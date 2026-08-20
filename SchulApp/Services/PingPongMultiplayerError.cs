using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace SchulApp.Services
{
    public static class PingPongMultiplayerError
    {
        public const int ConnectionTimeoutSeconds = 8;

        public static string GetUserMessage(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            Exception current = Unwrap(exception);

            if (current is TimeoutException || current is TaskCanceledException)
            {
                return "Die Multiplayer-Verbindung hat zu lange gedauert. Prüfe Host-IP, Port 63636 und Netzwerkverbindung.";
            }

            if (current is WebSocketException ||
                current is HttpRequestException ||
                current is SocketException)
            {
                return "Der Multiplayer-Host ist nicht erreichbar. Prüfe Host-IP, Port 63636, Netzwerk und Windows-Firewall.";
            }

            string message = current.Message.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                return "Beim PingPong-Multiplayer ist ein Verbindungsfehler aufgetreten.";
            }

            return message;
        }

        public static string GetDisconnectMessage(bool remotePlayerKnown)
        {
            return remotePlayerKnown
                ? "Der andere Spieler hat die Multiplayer-Verbindung verlassen. Das Spiel wurde gestoppt."
                : "Die Multiplayer-Verbindung zum Server wurde getrennt. Das Spiel wurde gestoppt.";
        }

        private static Exception Unwrap(Exception exception)
        {
            Exception current = exception;

            while (current.InnerException != null &&
                   (current is AggregateException || current is InvalidOperationException))
            {
                current = current.InnerException;
            }

            return current;
        }
    }
}
