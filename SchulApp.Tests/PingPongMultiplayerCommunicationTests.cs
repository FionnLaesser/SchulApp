using SchulApp.Models;
using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class PingPongMultiplayerCommunicationTests
    {
        [Theory]
        [InlineData(PingPongMultiplayerMessageTypes.Lobby)]
        [InlineData(PingPongMultiplayerMessageTypes.PlayerMovement)]
        [InlineData(PingPongMultiplayerMessageTypes.BallState)]
        [InlineData(PingPongMultiplayerMessageTypes.Score)]
        [InlineData(PingPongMultiplayerMessageTypes.GameStart)]
        [InlineData(PingPongMultiplayerMessageTypes.GameEnd)]
        [InlineData(PingPongMultiplayerMessageTypes.PauseRequest)]
        [InlineData(PingPongMultiplayerMessageTypes.PauseState)]
        public void IsSupported_KnownMessageType_ReturnsTrue(string messageType)
        {
            Assert.True(PingPongMultiplayerMessageTypes.IsSupported(messageType));
        }

        [Theory]
        [InlineData("Unknown")]
        [InlineData("")]
        [InlineData(null)]
        public void IsSupported_UnknownMessageType_ReturnsFalse(string? messageType)
        {
            Assert.False(PingPongMultiplayerMessageTypes.IsSupported(messageType));
        }

        [Fact]
        public void CreateWebSocketUri_DefaultLocalAddress_UsesHttpWebSocketPort()
        {
            Uri uri = PingPongMultiplayerConnection.CreateWebSocketUri(
                " ab12cd ",
                42,
                "http://localhost:63636/"
            );

            Assert.Equal("ws", uri.Scheme);
            Assert.Equal("localhost", uri.Host);
            Assert.Equal(63636, uri.Port);
            Assert.Equal("/ws/pingpong/AB12CD", uri.AbsolutePath);
            Assert.Equal("?userId=42", uri.Query);
        }

        [Fact]
        public void CreateWebSocketUri_LanAddress_UsesRemoteHost()
        {
            Uri uri = PingPongMultiplayerConnection.CreateWebSocketUri(
                "LAN123",
                11,
                "http://192.168.1.42:63636/"
            );

            Assert.Equal("ws", uri.Scheme);
            Assert.Equal("192.168.1.42", uri.Host);
            Assert.Equal(63636, uri.Port);
            Assert.Equal("/ws/pingpong/LAN123", uri.AbsolutePath);
            Assert.Equal("?userId=11", uri.Query);
        }

        [Fact]
        public void CreateWebSocketUri_HttpsAddress_UsesSecureWebSocketScheme()
        {
            Uri uri = PingPongMultiplayerConnection.CreateWebSocketUri(
                "ABC123",
                7,
                "https://school.example:7443/api/"
            );

            Assert.Equal("wss", uri.Scheme);
            Assert.Equal("school.example", uri.Host);
            Assert.Equal(7443, uri.Port);
            Assert.Equal("/api/ws/pingpong/ABC123", uri.AbsolutePath);
            Assert.Equal("?userId=7", uri.Query);
        }

        [Fact]
        public void CreateWebSocketUri_UnsupportedScheme_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                PingPongMultiplayerConnection.CreateWebSocketUri(
                    "ABC123",
                    7,
                    "ftp://school.example:7443/"
                )
            );
        }

        [Theory]
        [InlineData("localhost", "http://localhost:63636/")]
        [InlineData("192.168.1.42", "http://192.168.1.42:63636/")]
        [InlineData("192.168.1.42:7000", "http://192.168.1.42:7000/")]
        [InlineData("http://school-pc:63636", "http://school-pc:63636/")]
        public void TryNormalizeHost_ValidInput_ReturnsExpectedAddress(
            string input,
            string expected)
        {
            bool valid = PingPongMultiplayerEndpoint.TryNormalizeHost(
                input,
                out string normalized
            );

            Assert.True(valid);
            Assert.Equal(expected, normalized);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ftp://192.168.1.42")]
        [InlineData("http://")]
        public void TryNormalizeHost_InvalidInput_ReturnsFalse(string input)
        {
            Assert.False(
                PingPongMultiplayerEndpoint.TryNormalizeHost(input, out _)
            );
        }
    }
}
