using SchulApp.Models;
using System.Text.Json;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class PingPongPlayerMovementTests
    {
        [Theory]
        [InlineData(0, 300, 0.0)]
        [InlineData(150, 300, 0.5)]
        [InlineData(300, 300, 1.0)]
        [InlineData(-20, 300, 0.0)]
        [InlineData(350, 300, 1.0)]
        public void Normalize_ProducesBoundedRelativePosition(
            int top,
            int maxTop,
            double expected)
        {
            double normalized = PingPongPlayerMovementPosition.Normalize(
                top,
                maxTop
            );

            Assert.Equal(expected, normalized, 6);
        }

        [Theory]
        [InlineData(0.0, 300, 0)]
        [InlineData(0.5, 300, 150)]
        [InlineData(1.0, 300, 300)]
        public void TryDenormalize_ValidPosition_ReturnsPaddleTop(
            double normalizedTop,
            int maxTop,
            int expectedTop)
        {
            bool valid = PingPongPlayerMovementPosition.TryDenormalize(
                normalizedTop,
                maxTop,
                out int top
            );

            Assert.True(valid);
            Assert.Equal(expectedTop, top);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(1.01)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void TryDenormalize_InvalidPosition_IsRejected(double normalizedTop)
        {
            bool valid = PingPongPlayerMovementPosition.TryDenormalize(
                normalizedTop,
                300,
                out _
            );

            Assert.False(valid);
        }

        [Fact]
        public void Payload_CamelCaseJson_DeserializesNormalizedTop()
        {
            using JsonDocument document = JsonDocument.Parse(
                "{\"normalizedTop\":0.625}"
            );

            PingPongPlayerMovementPayload? payload =
                document.RootElement.Deserialize<PingPongPlayerMovementPayload>();

            Assert.NotNull(payload);
            Assert.Equal(0.625, payload!.NormalizedTop, 6);
        }
    }
}
