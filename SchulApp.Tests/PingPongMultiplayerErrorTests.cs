using SchulApp.Services;
using System.Net.Http;
using System.Net.WebSockets;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class PingPongMultiplayerErrorTests
    {
        [Fact]
        public void GetUserMessage_Timeout_ReturnsUsefulMessage()
        {
            string message = PingPongMultiplayerError.GetUserMessage(
                new TimeoutException("timeout")
            );

            Assert.Contains("zu lange", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("63636", message, StringComparison.Ordinal);
        }

        [Fact]
        public void GetUserMessage_HttpFailure_ReturnsHostGuidance()
        {
            string message = PingPongMultiplayerError.GetUserMessage(
                new HttpRequestException("connection refused")
            );

            Assert.Contains("nicht erreichbar", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Firewall", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetUserMessage_WrappedWebSocketFailure_IsUnwrapped()
        {
            Exception exception = new InvalidOperationException(
                "wrapper",
                new WebSocketException("socket failed")
            );

            string message = PingPongMultiplayerError.GetUserMessage(exception);

            Assert.Contains("nicht erreichbar", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetDisconnectMessage_RemotePlayerKnown_ExplainsPlayerLeft()
        {
            string message = PingPongMultiplayerError.GetDisconnectMessage(true);

            Assert.Contains("andere Spieler", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("gestoppt", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ConnectionTimeout_IsShortEnoughForInteractiveJoin()
        {
            Assert.InRange(
                PingPongMultiplayerError.ConnectionTimeoutSeconds,
                3,
                10
            );
        }
    }
}
