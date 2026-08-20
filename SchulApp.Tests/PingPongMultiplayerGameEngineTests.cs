using SchulApp.Services;
using System.Drawing;
using Xunit;

namespace SchulApp.Tests
{
    public class PingPongMultiplayerGameEngineTests
    {
        [Fact]
        public void Start_ResetsScoreAndCentersBall()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.Start(
                playfieldWidth: 1200,
                playfieldHeight: 645,
                ballWidth: 40,
                ballHeight: 40,
                directionX: 1
            );

            Assert.True(engine.IsRunning);
            Assert.Equal(0, engine.PlayerOneScore);
            Assert.Equal(0, engine.PlayerTwoScore);
            Assert.Equal(580, engine.BallLeft);
            Assert.Equal(302, engine.BallTop);
            Assert.Equal(1, engine.DirectionX);
        }

        [Fact]
        public void Start_NegativeDirectionStartsTowardPlayerOne()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.Start(300, 220, 40, 40, directionX: -1);

            Assert.Equal(-1, engine.DirectionX);
        }

        [Fact]
        public void Tick_BouncesFromLeftPaddle()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.Start(300, 220, 40, 40, directionX: -1);

            Rectangle left = new Rectangle(0, 0, 140, 220);
            Rectangle right = new Rectangle(266, 0, 34, 220);

            engine.Tick(left, right, 300, 220, 40, 40);

            Assert.Equal(1, engine.DirectionX);
            Assert.Equal(left.Right, engine.BallLeft);
        }

        [Fact]
        public void Tick_BouncesFromRightPaddle()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.Start(300, 220, 40, 40, directionX: 1);

            Rectangle left = new Rectangle(0, 0, 34, 220);
            Rectangle right = new Rectangle(266, 0, 34, 220);

            for (int i = 0; i < 30 && engine.DirectionX > 0; i++)
            {
                engine.Tick(left, right, 300, 220, 40, 40);
            }

            Assert.Equal(-1, engine.DirectionX);
            Assert.Equal(right.Left - 40, engine.BallLeft);
        }

        [Fact]
        public void Tick_AwardsPointToPlayerOneAndResetsBall()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.SetBallSpeed(30);
            engine.Start(100, 120, 40, 40, directionX: 1);

            Rectangle noLeftPaddle = new Rectangle(-200, -200, 1, 1);
            Rectangle noRightPaddle = new Rectangle(300, -200, 1, 1);

            PingPongGameTickResult result = PingPongGameTickResult.None;

            for (int i = 0; i < 10 && !result.ScoreChanged; i++)
            {
                result = engine.Tick(
                    noLeftPaddle,
                    noRightPaddle,
                    100,
                    120,
                    40,
                    40
                );
            }

            Assert.True(result.ScoreChanged);
            Assert.True(result.BallReset);
            Assert.False(result.GameEnded);
            Assert.Equal(1, engine.PlayerOneScore);
            Assert.Equal(0, engine.PlayerTwoScore);
            Assert.Equal(30, engine.BallLeft);
            Assert.Equal(40, engine.BallTop);
        }

        [Fact]
        public void Tick_AwardsPointToPlayerTwoAndResetsBall()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            engine.SetBallSpeed(30);
            engine.Start(100, 120, 40, 40, directionX: -1);

            Rectangle noLeftPaddle = new Rectangle(-200, -200, 1, 1);
            Rectangle noRightPaddle = new Rectangle(300, -200, 1, 1);

            PingPongGameTickResult result = PingPongGameTickResult.None;

            for (int i = 0; i < 10 && !result.ScoreChanged; i++)
            {
                result = engine.Tick(
                    noLeftPaddle,
                    noRightPaddle,
                    100,
                    120,
                    40,
                    40
                );
            }

            Assert.True(result.ScoreChanged);
            Assert.True(result.BallReset);
            Assert.False(result.GameEnded);
            Assert.Equal(0, engine.PlayerOneScore);
            Assert.Equal(1, engine.PlayerTwoScore);
            Assert.Equal(30, engine.BallLeft);
            Assert.Equal(40, engine.BallTop);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(-1, 2)]
        public void Tick_StopsGameAtFivePointsForEitherPlayer(
            int directionX,
            int expectedWinner)
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            Rectangle noLeftPaddle = new Rectangle(-200, -200, 1, 1);
            Rectangle noRightPaddle = new Rectangle(300, -200, 1, 1);
            PingPongGameTickResult result = PingPongGameTickResult.None;

            engine.SetBallSpeed(30);
            engine.Start(100, 120, 40, 40, directionX);

            while (engine.IsRunning)
            {
                result = engine.Tick(
                    noLeftPaddle,
                    noRightPaddle,
                    100,
                    120,
                    40,
                    40
                );
            }

            Assert.True(result.GameEnded);
            Assert.False(engine.IsRunning);
            Assert.Equal(expectedWinner, result.WinnerPlayer);

            if (expectedWinner == 1)
            {
                Assert.Equal(5, engine.PlayerOneScore);
                Assert.Equal(0, engine.PlayerTwoScore);
            }
            else
            {
                Assert.Equal(0, engine.PlayerOneScore);
                Assert.Equal(5, engine.PlayerTwoScore);
            }
        }

        [Fact]
        public void Tick_AfterGameEndDoesNotChangeState()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();
            Rectangle noLeftPaddle = new Rectangle(-200, -200, 1, 1);
            Rectangle noRightPaddle = new Rectangle(300, -200, 1, 1);

            engine.SetBallSpeed(30);
            engine.Start(100, 120, 40, 40, directionX: 1);

            while (engine.IsRunning)
            {
                engine.Tick(noLeftPaddle, noRightPaddle, 100, 120, 40, 40);
            }

            int left = engine.BallLeft;
            int top = engine.BallTop;
            int scoreOne = engine.PlayerOneScore;
            int scoreTwo = engine.PlayerTwoScore;

            PingPongGameTickResult result =
                engine.Tick(noLeftPaddle, noRightPaddle, 100, 120, 40, 40);

            Assert.False(result.ScoreChanged);
            Assert.Equal(left, engine.BallLeft);
            Assert.Equal(top, engine.BallTop);
            Assert.Equal(scoreOne, engine.PlayerOneScore);
            Assert.Equal(scoreTwo, engine.PlayerTwoScore);
        }
    }
}
