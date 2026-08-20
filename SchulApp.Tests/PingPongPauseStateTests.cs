using SchulApp.Models;
using System.Text.Json;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class PingPongPauseStateTests
    {
        [Theory]
        [InlineData(PingPongMultiplayerMessageTypes.PauseRequest)]
        [InlineData(PingPongMultiplayerMessageTypes.PauseState)]
        public void IsSupported_PauseMessageTypes_ReturnsTrue(string messageType)
        {
            Assert.True(PingPongMultiplayerMessageTypes.IsSupported(messageType));
        }

        [Fact]
        public void PauseRequestPayload_UsesNetworkPropertyName()
        {
            string json = JsonSerializer.Serialize(
                new PingPongPauseRequestPayload { IsPaused = true }
            );

            Assert.Contains("\"isPaused\":true", json, StringComparison.Ordinal);
        }

        [Fact]
        public void PauseStatePayload_DeserializesPauseValue()
        {
            PingPongPauseStatePayload? payload =
                JsonSerializer.Deserialize<PingPongPauseStatePayload>(
                    "{\"isPaused\":true}"
                );

            Assert.NotNull(payload);
            Assert.True(payload.IsPaused);
        }
    }
}
