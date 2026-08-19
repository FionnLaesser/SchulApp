using SchulApp.Models;
using System.Text.Json;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class PingPongMultiplayerStateTests
    {
        [Theory]
        [InlineData(0, 100, 0.0)]
        [InlineData(50, 100, 0.5)]
        [InlineData(100, 100, 1.0)]
        [InlineData(-10, 100, 0.0)]
        [InlineData(150, 100, 1.0)]
        public void BallPosition_Normalize_IsBounded(
            int value,
            int maxValue,
            double expected)
        {
            Assert.Equal(
                expected,
                PingPongBallStatePosition.Normalize(value, maxValue),
                precision: 6
            );
        }

        [Theory]
        [InlineData(0.0, 100, 0)]
        [InlineData(0.5, 100, 50)]
        [InlineData(1.0, 100, 100)]
        public void BallPosition_TryDenormalize_ValidValueRestoresPosition(
            double normalized,
            int maxValue,
            int expected)
        {
            bool valid = PingPongBallStatePosition.TryDenormalize(
                normalized,
                maxValue,
                out int value
            );

            Assert.True(valid);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(1.01)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void BallPosition_TryDenormalize_InvalidValueIsRejected(
            double normalized)
        {
            Assert.False(
                PingPongBallStatePosition.TryDenormalize(
                    normalized,
                    100,
                    out _
                )
            );
        }

        [Fact]
        public void BallStatePayload_RoundTripPreservesSynchronizedState()
        {
            PingPongBallStatePayload original = new PingPongBallStatePayload
            {
                NormalizedLeft = 0.25,
                NormalizedTop = 0.75,
                DirectionX = -1,
                DirectionY = 1
            };

            string json = JsonSerializer.Serialize(original);
            PingPongBallStatePayload? restored =
                JsonSerializer.Deserialize<PingPongBallStatePayload>(json);

            Assert.NotNull(restored);
            Assert.Equal(original.NormalizedLeft, restored.NormalizedLeft);
            Assert.Equal(original.NormalizedTop, restored.NormalizedTop);
            Assert.Equal(original.DirectionX, restored.DirectionX);
            Assert.Equal(original.DirectionY, restored.DirectionY);
        }

        [Fact]
        public void ScorePayload_RoundTripPreservesBothScores()
        {
            PingPongScorePayload original = new PingPongScorePayload
            {
                PlayerOneScore = 4,
                PlayerTwoScore = 3
            };

            string json = JsonSerializer.Serialize(original);
            PingPongScorePayload? restored =
                JsonSerializer.Deserialize<PingPongScorePayload>(json);

            Assert.NotNull(restored);
            Assert.Equal(4, restored.PlayerOneScore);
            Assert.Equal(3, restored.PlayerTwoScore);
        }

        [Fact]
        public void GameEndPayload_RoundTripPreservesWinnerAndFinalScore()
        {
            PingPongGameEndPayload original = new PingPongGameEndPayload
            {
                WinnerUserId = 42,
                PlayerOneScore = 5,
                PlayerTwoScore = 2
            };

            string json = JsonSerializer.Serialize(original);
            PingPongGameEndPayload? restored =
                JsonSerializer.Deserialize<PingPongGameEndPayload>(json);

            Assert.NotNull(restored);
            Assert.Equal(42, restored.WinnerUserId);
            Assert.Equal(5, restored.PlayerOneScore);
            Assert.Equal(2, restored.PlayerTwoScore);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PauseStatePayload_RoundTripPreservesPauseState(bool isPaused)
        {
            PingPongPauseStatePayload original =
                new PingPongPauseStatePayload { IsPaused = isPaused };

            string json = JsonSerializer.Serialize(original);
            PingPongPauseStatePayload? restored =
                JsonSerializer.Deserialize<PingPongPauseStatePayload>(json);

            Assert.NotNull(restored);
            Assert.Equal(isPaused, restored.IsPaused);
        }
    }
}
