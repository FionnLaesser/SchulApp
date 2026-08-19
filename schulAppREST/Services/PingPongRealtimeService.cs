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
                "GameEnd",
                "PauseRequest",
                "PauseState"
            };

        private static readonly HashSet<string> AuthoritativeMessageTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "BallState",
                "Score",
                "GameStart",
                "GameEnd",
                "PauseState"
            };

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly ConcurrentDictionary<string, RealtimeSession> sessions =
            new(StringComparer.OrdinalIgnoreCase);

        public async Task HandleConnectionAsync(
            string gameCode,
            int userId,
            int authoritativeUserId,
            WebSocket socket,
            CancellationToken cancellationToken)
        {
            string normalizedCode = NormalizeCode(gameCode);
            RealtimeSession session = sessions.GetOrAdd(
                normalizedCode,
                _ => new RealtimeSession()
            );

            ClientConnection connection = new ClientConnection(socket);

            if (session.Connections.TryGetValue(
                userId,
                out ClientConnection? existingConnection))
            {
                session.Connections[userId] = connection;
                await CloseConnectionQuietlyAsync(existingConnection);
            }
            else
            {
                session.Connections.TryAdd(userId, connection);
            }

            await BroadcastLobbyEventAsync(
                normalizedCode,
                session,
                "PlayerConnected",
                userId,
                cancellationToken
            );

            await ReplayLatestGameStateAsync(
                session,
                connection,
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

                    StoreAuthoritativeState(session, message);

                    await BroadcastAsync(
                        session,
                        message,
                        excludedUserId: userId,
                        cancellationToken
                    );
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
            finally
            {
                if (
                    session.Connections.TryGetValue(
                        userId,
                        out ClientConnection? current) &&
                    ReferenceEquals(current, connection))
                {
                    session.Connections.TryRemove(userId, out _);
                }

                await CloseConnectionQuietlyAsync(connection);

                if (session.Connections.IsEmpty)
                {
                    sessions.TryRemove(normalizedCode, out _);
                }
                else
                {
                    await BroadcastLobbyEventAsync(
                        normalizedCode,
                        session,
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

        private static void StoreAuthoritativeState(
            RealtimeSession session,
            PingPongRealtimeMessage message)
        {
            switch (message.Type)
            {
                case "GameStart":
                    session.LatestGameStart = message;
                    session.LatestGameEnd = null;
                    session.LatestBallState = null;
                    session.LatestScore = null;
                    session.LatestPauseState = null;
                    break;

                case "BallState":
                    session.LatestBallState = message;
                    break;

                case "Score":
                    session.LatestScore = message;
                    break;

                case "PauseState":
                    session.LatestPauseState = message;
                    break;

                case "GameEnd":
                    session.LatestGameEnd = message;
                    break;
            }
        }

        private static async Task ReplayLatestGameStateAsync(
            RealtimeSession session,
            ClientConnection connection,
            CancellationToken cancellationToken)
        {
            PingPongRealtimeMessage?[] messages =
            {
                session.LatestGameStart,
                session.LatestScore,
                session.LatestBallState,
                session.LatestPauseState,
                session.LatestGameEnd
            };

            foreach (PingPongRealtimeMessage? message in messages)
            {
                if (message == null)
                {
                    continue;
                }

                byte[] data = JsonSerializer.SerializeToUtf8Bytes(
                    message,
                    JsonOptions
                );

                await SendQuietlyAsync(
                    connection,
                    data,
                    cancellationToken
                );
            }
        }

        private async Task BroadcastLobbyEventAsync(
            string gameCode,
            RealtimeSession session,
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
                session,
                message,
                excludedUserId: null,
                cancellationToken
            );
        }

        private static async Task BroadcastAsync(
            RealtimeSession session,
            PingPongRealtimeMessage message,
            int? excludedUserId,
            CancellationToken cancellationToken)
        {
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);

            foreach ((int userId, ClientConnection connection) in
                     session.Connections)
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

        private sealed class RealtimeSession
        {
            public ConcurrentDictionary<int, ClientConnection> Connections { get; } =
                new ConcurrentDictionary<int, ClientConnection>();

            public PingPongRealtimeMessage? LatestGameStart { get; set; }
            public PingPongRealtimeMessage? LatestBallState { get; set; }
            public PingPongRealtimeMessage? LatestScore { get; set; }
            public PingPongRealtimeMessage? LatestPauseState { get; set; }
            public PingPongRealtimeMessage? LatestGameEnd { get; set; }
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
