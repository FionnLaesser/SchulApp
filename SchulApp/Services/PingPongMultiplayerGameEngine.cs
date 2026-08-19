namespace SchulApp.Services
{
    public sealed class PingPongMultiplayerGameEngine
    {
        public const int MaxScore = 5;

        private int ballSpeedX = 6;
        private int ballSpeedY = 6;

        public int BallLeft { get; private set; }
        public int BallTop { get; private set; }
        public int PlayerOneScore { get; private set; }
        public int PlayerTwoScore { get; private set; }
        public bool IsRunning { get; private set; }

        public int DirectionX => Math.Sign(ballSpeedX);
        public int DirectionY => Math.Sign(ballSpeedY);

        public void SetBallSpeed(int speed)
        {
            int safeSpeed = Math.Clamp(Math.Abs(speed), 1, 30);
            ballSpeedX = DirectionX == 0
                ? safeSpeed
                : safeSpeed * DirectionX;
            ballSpeedY = DirectionY == 0
                ? safeSpeed
                : safeSpeed * DirectionY;
        }

        public void Start(
            int playfieldWidth,
            int playfieldHeight,
            int ballWidth,
            int ballHeight,
            int? directionX = null)
        {
            PlayerOneScore = 0;
            PlayerTwoScore = 0;
            IsRunning = true;

            int direction = directionX.HasValue
                ? NormalizeDirection(directionX.Value)
                : Random.Shared.Next(0, 2) == 0 ? -1 : 1;

            ResetBall(
                playfieldWidth,
                playfieldHeight,
                ballWidth,
                ballHeight,
                direction
            );
        }

        public PingPongGameTickResult Tick(
            Rectangle playerOnePaddle,
            Rectangle playerTwoPaddle,
            int playfieldWidth,
            int playfieldHeight,
            int ballWidth,
            int ballHeight)
        {
            if (!IsRunning)
            {
                return PingPongGameTickResult.None;
            }

            BallLeft += ballSpeedX;
            BallTop += ballSpeedY;

            if (BallTop <= 0)
            {
                BallTop = 0;
                ballSpeedY = Math.Abs(ballSpeedY);
            }

            if (BallTop + ballHeight >= playfieldHeight)
            {
                BallTop = Math.Max(0, playfieldHeight - ballHeight);
                ballSpeedY = -Math.Abs(ballSpeedY);
            }

            Rectangle ballBounds = new Rectangle(
                BallLeft,
                BallTop,
                ballWidth,
                ballHeight
            );

            if (ballBounds.IntersectsWith(playerOnePaddle) && ballSpeedX < 0)
            {
                BallLeft = playerOnePaddle.Right;
                ballSpeedX = Math.Abs(ballSpeedX);
            }
            else if (ballBounds.IntersectsWith(playerTwoPaddle) && ballSpeedX > 0)
            {
                BallLeft = playerTwoPaddle.Left - ballWidth;
                ballSpeedX = -Math.Abs(ballSpeedX);
            }

            if (BallLeft + ballWidth < 0)
            {
                PlayerTwoScore++;
                return FinishPoint(
                    playfieldWidth,
                    playfieldHeight,
                    ballWidth,
                    ballHeight,
                    -1,
                    winnerPlayer: 2
                );
            }

            if (BallLeft > playfieldWidth)
            {
                PlayerOneScore++;
                return FinishPoint(
                    playfieldWidth,
                    playfieldHeight,
                    ballWidth,
                    ballHeight,
                    1,
                    winnerPlayer: 1
                );
            }

            return PingPongGameTickResult.None;
        }

        private PingPongGameTickResult FinishPoint(
            int playfieldWidth,
            int playfieldHeight,
            int ballWidth,
            int ballHeight,
            int nextDirection,
            int winnerPlayer)
        {
            bool gameEnded =
                PlayerOneScore >= MaxScore || PlayerTwoScore >= MaxScore;

            if (gameEnded)
            {
                IsRunning = false;
                return new PingPongGameTickResult(
                    scoreChanged: true,
                    ballReset: false,
                    gameEnded: true,
                    winnerPlayer: winnerPlayer
                );
            }

            ResetBall(
                playfieldWidth,
                playfieldHeight,
                ballWidth,
                ballHeight,
                nextDirection
            );

            return new PingPongGameTickResult(
                scoreChanged: true,
                ballReset: true,
                gameEnded: false,
                winnerPlayer: 0
            );
        }

        private void ResetBall(
            int playfieldWidth,
            int playfieldHeight,
            int ballWidth,
            int ballHeight,
            int directionX)
        {
            BallLeft = Math.Max(0, (playfieldWidth - ballWidth) / 2);
            BallTop = Math.Max(0, (playfieldHeight - ballHeight) / 2);

            int horizontalSpeed = Math.Max(1, Math.Abs(ballSpeedX));
            int verticalSpeed = Math.Max(1, Math.Abs(ballSpeedY));

            ballSpeedX = horizontalSpeed * NormalizeDirection(directionX);
            ballSpeedY = Random.Shared.Next(0, 2) == 0
                ? -verticalSpeed
                : verticalSpeed;
        }

        private static int NormalizeDirection(int direction)
        {
            return direction < 0 ? -1 : 1;
        }
    }

    public readonly struct PingPongGameTickResult
    {
        public static PingPongGameTickResult None => new PingPongGameTickResult(
            scoreChanged: false,
            ballReset: false,
            gameEnded: false,
            winnerPlayer: 0
        );

        public PingPongGameTickResult(
            bool scoreChanged,
            bool ballReset,
            bool gameEnded,
            int winnerPlayer)
        {
            ScoreChanged = scoreChanged;
            BallReset = ballReset;
            GameEnded = gameEnded;
            WinnerPlayer = winnerPlayer;
        }

        public bool ScoreChanged { get; }
        public bool BallReset { get; }
        public bool GameEnded { get; }
        public int WinnerPlayer { get; }
    }
}
