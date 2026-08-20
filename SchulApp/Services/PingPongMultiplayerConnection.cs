using SchulApp.Models;
using System.Net.WebSockets;
using System.Text.Json;

namespace SchulApp.Services
{
    public sealed class PingPongMultiplayerConnection : IAsyncDisposable
    {
        private const int MaxMessageBytes = 16 * 1024;

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);

        private ClientWebSocket? socket;
        private CancellationTokenSource? receiveCancellation;
        private Task? receiveTask;
        private SynchronizationContext? uiContext;
        private string gameCode = string.Empty;
        private int userId;
        private int disconnectedRaised;
        private int resultNotificationRaised;
        private int opponentDisconnectNotificationRaised;

        public event EventHandler<PingPongMultiplayerMessage>? MessageReceived;
        public event EventHandler? Disconnected;

        public bool IsConnected => socket?.State == WebSocketState.Open;

        public async Task ConnectAsync(
            string gameCode,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException(
                    "Die Multiplayer-Verbindung ist bereits geöffnet."
                );
            }

            if (string.IsNullOrWhiteSpace(gameCode))
            {
                throw new ArgumentException(
                    "Ein Game Code ist erforderlich.",
                    nameof(gameCode)
                );
            }

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            this.gameCode = gameCode.Trim().ToUpperInvariant();
            this.userId = userId;
            uiContext = SynchronizationContext.Current;
            disconnectedRaised = 0;
            resultNotificationRaised = 0;
            opponentDisconnectNotificationRaised = 0;

            socket?.Dispose();
            receiveCancellation?.Dispose();

            socket = new ClientWebSocket();
            receiveCancellation = new CancellationTokenSource();

            Uri endpoint = CreateWebSocketUri(this.gameCode, userId);

            using CancellationTokenSource connectionTimeout =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            connectionTimeout.CancelAfter(
                TimeSpan.FromSeconds(PingPongMultiplayerError.ConnectionTimeoutSeconds)
            );

            try
            {
                await socket.ConnectAsync(endpoint, connectionTimeout.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                socket.Dispose();
                socket = null;
                receiveCancellation.Dispose();
                receiveCancellation = null;

                throw new TimeoutException(
                    "Die Multiplayer-Verbindung hat zu lange gedauert. Prüfe Host-IP, Port 63636 und Netzwerkverbindung."
                );
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                socket.Dispose();
                socket = null;
                receiveCancellation.Dispose();
                receiveCancellation = null;

                throw new InvalidOperationException(
                    PingPongMultiplayerError.GetUserMessage(ex),
                    ex
                );
            }

            receiveTask = ReceiveLoopAsync(receiveCancellation.Token);
        }

        public async Task SendAsync<TPayload>(
            string messageType,
            TPayload payload,
            long sequence,
            CancellationToken cancellationToken = default)
        {
            if (!PingPongMultiplayerMessageTypes.IsSupported(messageType))
            {
                throw new ArgumentException(
                    "Unbekannter Multiplayer-Nachrichtentyp.",
                    nameof(messageType)
                );
            }

            if (sequence < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sequence));
            }

            ClientWebSocket currentSocket = socket ??
                throw new InvalidOperationException(
                    "Es besteht keine Multiplayer-Verbindung."
                );

            if (currentSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException(
                    "Die Multiplayer-Verbindung ist nicht geöffnet."
                );
            }

            PingPongMultiplayerMessage message =
                new PingPongMultiplayerMessage
                {
                    Type = messageType,
                    GameCode = gameCode,
                    SenderUserId = userId,
                    Sequence = sequence,
                    TimestampUnixMilliseconds =
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Payload = JsonSerializer.SerializeToElement(payload, JsonOptions)
                };

            if (message.Payload.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException(
                    "Multiplayer-Nutzdaten müssen als Objekt gesendet werden.",
                    nameof(payload)
                );
            }

            byte[] data = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);

            if (data.Length > MaxMessageBytes)
            {
                throw new InvalidOperationException(
                    "Die Multiplayer-Nachricht ist zu gross."
                );
            }

            await sendLock.WaitAsync(cancellationToken);

            try
            {
                await currentSocket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken
                );
            }
            finally
            {
                sendLock.Release();
            }

            if (messageType == PingPongMultiplayerMessageTypes.GameEnd)
            {
                NotifyGameResult(message.Payload);
            }
        }

        public async Task DisconnectAsync()
        {
            ClientWebSocket? currentSocket = socket;
            CancellationTokenSource? currentCancellation = receiveCancellation;
            Task? currentReceiveTask = receiveTask;

            receiveTask = null;
            receiveCancellation = null;
            socket = null;

            if (currentSocket == null)
            {
                return;
            }

            currentCancellation?.Cancel();

            try
            {
                if (currentSocket.State == WebSocketState.Open)
                {
                    await currentSocket.CloseOutputAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Client disconnected.",
                        CancellationToken.None
                    );
                }
            }
            catch (WebSocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            if (currentReceiveTask != null)
            {
                try
                {
                    await currentReceiveTask;
                }
                catch (OperationCanceledException)
                {
                }
            }

            currentCancellation?.Dispose();
            currentSocket.Dispose();
        }

        public static Uri CreateWebSocketUri(
            string gameCode,
            int userId,
            string? baseAddress = null)
        {
            string configuredBaseAddress = baseAddress ??
                PingPongMultiplayerEndpoint.CurrentBaseAddress;

            if (!configuredBaseAddress.EndsWith('/'))
            {
                configuredBaseAddress += "/";
            }

            Uri apiUri = new Uri(configuredBaseAddress, UriKind.Absolute);
            string websocketScheme = apiUri.Scheme switch
            {
                "https" => "wss",
                "http" => "ws",
                "wss" => "wss",
                "ws" => "ws",
                _ => throw new InvalidOperationException(
                    "Die Multiplayer-Basisadresse muss HTTP, HTTPS, WS oder WSS verwenden."
                )
            };

            string normalizedCode = (gameCode ?? string.Empty)
                .Trim()
                .ToUpperInvariant();

            UriBuilder builder = new UriBuilder(apiUri)
            {
                Scheme = websocketScheme,
                Path = apiUri.AbsolutePath.TrimEnd('/') +
                    "/ws/pingpong/" +
                    Uri.EscapeDataString(normalizedCode),
                Query = "userId=" + userId
            };

            return builder.Uri;
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            ClientWebSocket? currentSocket = socket;

            if (currentSocket == null)
            {
                return;
            }

            try
            {
                while (
                    currentSocket.State == WebSocketState.Open &&
                    !cancellationToken.IsCancellationRequested)
                {
                    PingPongMultiplayerMessage? message =
                        await ReceiveMessageAsync(currentSocket, cancellationToken);

                    if (message == null)
                    {
                        break;
                    }

                    if (!IsValidIncomingMessage(message))
                    {
                        continue;
                    }

                    MessageReceived?.Invoke(this, message);
                    HandleClientNotification(message);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                RaiseDisconnectedOnce();
            }
        }

        private async Task<PingPongMultiplayerMessage?> ReceiveMessageAsync(
            ClientWebSocket currentSocket,
            CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];
            using MemoryStream stream = new MemoryStream();

            while (true)
            {
                WebSocketReceiveResult result = await currentSocket.ReceiveAsync(
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
                return JsonSerializer.Deserialize<PingPongMultiplayerMessage>(
                    stream.ToArray(),
                    JsonOptions
                );
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private bool IsValidIncomingMessage(PingPongMultiplayerMessage message)
        {
            if (
                !PingPongMultiplayerMessageTypes.IsSupported(message.Type) ||
                !string.Equals(
                    message.GameCode,
                    gameCode,
                    StringComparison.OrdinalIgnoreCase
                ) ||
                message.Sequence < 0 ||
                message.Payload.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (message.Type == PingPongMultiplayerMessageTypes.Lobby)
            {
                return message.SenderUserId >= 0;
            }

            return message.SenderUserId > 0;
        }

        private void HandleClientNotification(PingPongMultiplayerMessage message)
        {
            if (message.Type == PingPongMultiplayerMessageTypes.GameEnd)
            {
                NotifyGameResult(message.Payload);
                return;
            }

            if (message.Type != PingPongMultiplayerMessageTypes.Lobby)
            {
                return;
            }

            PingPongLobbyEventPayload? payload =
                TryDeserializePayload<PingPongLobbyEventPayload>(message.Payload);

            if (payload == null ||
                !string.Equals(
                    payload.EventName,
                    "PlayerDisconnected",
                    StringComparison.Ordinal) ||
                payload.UserId <= 0 ||
                payload.UserId == userId)
            {
                return;
            }

            if (Interlocked.Exchange(ref opponentDisconnectNotificationRaised, 1) != 0)
            {
                return;
            }

            RaiseDisconnectedOnce();
            receiveCancellation?.Cancel();
            socket?.Abort();

            PostToUi(PingPongMultiplayerResultForm.ShowOpponentDisconnected);
        }

        private void NotifyGameResult(JsonElement payloadElement)
        {
            PingPongGameEndPayload? payload =
                TryDeserializePayload<PingPongGameEndPayload>(payloadElement);

            if (payload == null ||
                payload.WinnerUserId <= 0 ||
                payload.PlayerOneScore < 0 ||
                payload.PlayerTwoScore < 0)
            {
                return;
            }

            if (Interlocked.Exchange(ref resultNotificationRaised, 1) != 0)
            {
                return;
            }

            PostToUi(() =>
                PingPongMultiplayerResultForm.ShowGameResult(
                    userId,
                    payload.WinnerUserId,
                    payload.PlayerOneScore,
                    payload.PlayerTwoScore
                )
            );
        }

        private static TPayload? TryDeserializePayload<TPayload>(JsonElement payload)
            where TPayload : class
        {
            try
            {
                return payload.Deserialize<TPayload>(JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private void PostToUi(Action action)
        {
            SynchronizationContext? context = uiContext;

            if (context == null)
            {
                return;
            }

            context.Post(
                _ =>
                {
                    try
                    {
                        action();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                },
                null
            );
        }

        private void RaiseDisconnectedOnce()
        {
            if (Interlocked.Exchange(ref disconnectedRaised, 1) == 0)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
            sendLock.Dispose();
        }

        private sealed class PingPongLobbyEventPayload
        {
            public string EventName { get; set; } = string.Empty;
            public int UserId { get; set; }
        }
    }
}
