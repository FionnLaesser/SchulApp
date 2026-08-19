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
        public void Tick_AwardsPointAndResetsBall()
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
        public void Tick_StopsGameAtFivePoints()
        {
            PingPongMultiplayerGameEngine engine =
                new PingPongMultiplayerGameEngine();

            Rectangle noLeftPaddle = new Rectangle(-200, -200, 1, 1);
            Rectangle noRightPaddle = new Rectangle(300, -200, 1, 1);
            PingPongGameTickResult result = PingPongGameTickResult.None;

            engine.SetBallSpeed(30);
            engine.Start(100, 120, 40, 40, directionX: 1);

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
            Assert.Equal(1, result.WinnerPlayer);
            Assert.Equal(5, engine.PlayerOneScore);
            Assert.Equal(0, engine.PlayerTwoScore);
        }
    }
}
