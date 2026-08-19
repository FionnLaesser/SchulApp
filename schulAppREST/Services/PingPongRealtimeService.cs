using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;

namespace schulAppREST.Services
{
    public sealed class PingPongRealtimeService
    {
        private const int MaxMessageBytes = 16 * 1024;

        private static readonly HashSet<string> AllowedMessageTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "Lobby",
                "PlayerMovement",
                "BallState",
                "Score",
                "GameStart",
                "GameEnd"
            };

        private static readonly HashSet<string> AuthoritativeMessageTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "BallState",
                "Score",
                "GameStart",
                "GameEnd"
            };

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly ConcurrentDictionary<
            string,
            ConcurrentDictionary<int, ClientConnection>> sessions = new(
                StringComparer.OrdinalIgnoreCase
            );

        public async Task HandleConnectionAsync(
            string gameCode,
            int userId,
            int authoritativeUserId,
            WebSocket socket,
            CancellationToken cancellationToken)
        {
            string normalizedCode = NormalizeCode(gameCode);
            ConcurrentDictionary<int, ClientConnection> session =
                sessions.GetOrAdd(
                    normalizedCode,
                    _ => new ConcurrentDictionary<int, ClientConnection>()
                );

            ClientConnection connection = new ClientConnection(socket);

            if (session.TryGetValue(userId, out ClientConnection? existingConnection))
            {
                session[userId] = connection;
                await CloseConnectionQuietlyAsync(existingConnection);
            }
            else
            {
                session.TryAdd(userId, connection);
            }

            await BroadcastLobbyEventAsync(
                normalizedCode,
                "PlayerConnected",
                userId,
                cancellationToken
            );

            try
            {
                while (
                    socket.State == WebSocketState.Open &&
                    !cancellationToken.IsCancellationRequested)
                {
                    PingPongRealtimeMessage? message =
                        await ReceiveMessageAsync(socket, cancellationToken);

                    if (message == null)
                    {
                        break;
                    }

                    bool isAuthoritative = userId == authoritativeUserId;

                    if (!IsValidIncomingMessage(message, isAuthoritative))
                    {
                        continue;
                    }

                    message.GameCode = normalizedCode;
                    message.SenderUserId = userId;
                    message.TimestampUnixMilliseconds =
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    await BroadcastAsync(
                        normalizedCode,
                        message,
                        excludedUserId: userId,
                        cancellationToken
                    );
                }
            }
            catch (OperationCanceledException)
            {
                // The request or client connection was closed.
            }
            catch (WebSocketException)
            {
                // Network disconnects are handled by removing the client below.
            }
            finally
            {
                if (
                    session.TryGetValue(userId, out ClientConnection? current) &&
                    ReferenceEquals(current, connection))
                {
                    session.TryRemove(userId, out _);
                }

                await CloseConnectionQuietlyAsync(connection);

                if (session.IsEmpty)
                {
                    sessions.TryRemove(normalizedCode, out _);
                }
                else
                {
                    await BroadcastLobbyEventAsync(
                        normalizedCode,
                        "PlayerDisconnected",
                        userId,
                        CancellationToken.None
                    );
                }
            }
        }

        private async Task<PingPongRealtimeMessage?> ReceiveMessageAsync(
            WebSocket socket,
            CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];
            using MemoryStream stream = new MemoryStream();

            while (true)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return null;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                if (stream.Length + result.Count > MaxMessageBytes)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.MessageTooBig,
                        "Multiplayer message is too large.",
                        CancellationToken.None
                    );
                    return null;
                }

                stream.Write(buffer, 0, result.Count);

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            try
            {
                return JsonSerializer.Deserialize<PingPongRealtimeMessage>(
                    stream.ToArray(),
                    JsonOptions
                );
            }
            catch (JsonException)
            {
                return new PingPongRealtimeMessage();
            }
        }

        private static bool IsValidIncomingMessage(
            PingPongRealtimeMessage message,
            bool isAuthoritative)
        {
            if (
                string.IsNullOrWhiteSpace(message.Type) ||
                !AllowedMessageTypes.Contains(message.Type) ||
                message.Sequence < 0 ||
                message.Payload.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            // Lobby events are server-generated only.
            if (message.Type == "Lobby")
            {
                return false;
            }

            if (AuthoritativeMessageTypes.Contains(message.Type) &&
                !isAuthoritative)
            {
                return false;
            }

            return true;
        }

        private async Task BroadcastLobbyEventAsync(
            string gameCode,
            string eventName,
            int userId,
            CancellationToken cancellationToken)
        {
            PingPongRealtimeMessage message = new PingPongRealtimeMessage
            {
                Type = "Lobby",
                GameCode = gameCode,
                SenderUserId = 0,
                Sequence = 0,
                TimestampUnixMilliseconds =
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Payload = JsonSerializer.SerializeToElement(
                    new PingPongLobbyEventPayload
                    {
                        EventName = eventName,
                        UserId = userId
                    },
                    JsonOptions
                )
            };

            await BroadcastAsync(
                gameCode,
                message,
                excludedUserId: null,
                cancellationToken
            );
        }

        private async Task BroadcastAsync(
            string gameCode,
            PingPongRealtimeMessage message,
            int? excludedUserId,
            CancellationToken cancellationToken)
        {
            if (!sessions.TryGetValue(gameCode, out var session))
            {
                return;
            }

            byte[] data = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);

            foreach ((int userId, ClientConnection connection) in session)
            {
                if (excludedUserId.HasValue && userId == excludedUserId.Value)
                {
                    continue;
                }

                await SendQuietlyAsync(connection, data, cancellationToken);
            }
        }

        private static async Task SendQuietlyAsync(
            ClientConnection connection,
            byte[] data,
            CancellationToken cancellationToken)
        {
            if (connection.Socket.State != WebSocketState.Open)
            {
                return;
            }

            await connection.SendLock.WaitAsync(cancellationToken);

            try
            {
                if (connection.Socket.State != WebSocketState.Open)
                {
                    return;
                }

                await connection.Socket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken
                );
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
            finally
            {
                connection.SendLock.Release();
            }
        }

        private static async Task CloseConnectionQuietlyAsync(
            ClientConnection connection)
        {
            try
            {
                if (
                    connection.Socket.State == WebSocketState.Open ||
                    connection.Socket.State == WebSocketState.CloseReceived)
                {
                    await connection.Socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Multiplayer connection closed.",
                        CancellationToken.None
                    );
                }
            }
            catch (WebSocketException)
            {
            }
            finally
            {
                connection.SendLock.Dispose();
                connection.Socket.Dispose();
            }
        }

        private static string NormalizeCode(string gameCode)
        {
            return (gameCode ?? string.Empty).Trim().ToUpperInvariant();
        }

        private sealed class ClientConnection
        {
            public ClientConnection(WebSocket socket)
            {
                Socket = socket;
            }

            public WebSocket Socket { get; }
            public SemaphoreSlim SendLock { get; } = new SemaphoreSlim(1, 1);
        }

        private sealed class PingPongRealtimeMessage
        {
            public string Type { get; set; } = string.Empty;
            public string GameCode { get; set; } = string.Empty;
            public int SenderUserId { get; set; }
            public long Sequence { get; set; }
            public long TimestampUnixMilliseconds { get; set; }
            public JsonElement Payload { get; set; }
        }

        private sealed class PingPongLobbyEventPayload
        {
            public string EventName { get; set; } = string.Empty;
            public int UserId { get; set; }
        }
    }
}
