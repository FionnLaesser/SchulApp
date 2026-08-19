using System.Text.Json.Serialization;

namespace SchulApp.Models
{
    public sealed class PingPongBallStatePayload
    {
        [JsonPropertyName("normalizedLeft")]
        public double NormalizedLeft { get; set; }

        [JsonPropertyName("normalizedTop")]
        public double NormalizedTop { get; set; }

        [JsonPropertyName("directionX")]
        public int DirectionX { get; set; }

        [JsonPropertyName("directionY")]
        public int DirectionY { get; set; }
    }

    public sealed class PingPongScorePayload
    {
        [JsonPropertyName("playerOneScore")]
        public int PlayerOneScore { get; set; }

        [JsonPropertyName("playerTwoScore")]
        public int PlayerTwoScore { get; set; }
    }

    public sealed class PingPongGameStartPayload
    {
        [JsonPropertyName("playerOneScore")]
        public int PlayerOneScore { get; set; }

        [JsonPropertyName("playerTwoScore")]
        public int PlayerTwoScore { get; set; }
    }

    public sealed class PingPongGameEndPayload
    {
        [JsonPropertyName("winnerUserId")]
        public int WinnerUserId { get; set; }

        [JsonPropertyName("playerOneScore")]
        public int PlayerOneScore { get; set; }

        [JsonPropertyName("playerTwoScore")]
        public int PlayerTwoScore { get; set; }
    }

    public static class PingPongBallStatePosition
    {
        public static double Normalize(int value, int maxValue)
        {
            if (maxValue <= 0)
            {
                return 0;
            }

            int boundedValue = Math.Clamp(value, 0, maxValue);
            return (double)boundedValue / maxValue;
        }

        public static bool TryDenormalize(
            double normalizedValue,
            int maxValue,
            out int value)
        {
            value = 0;

            if (maxValue < 0 ||
                double.IsNaN(normalizedValue) ||
                double.IsInfinity(normalizedValue) ||
                normalizedValue < 0 ||
                normalizedValue > 1)
            {
                return false;
            }

            value = (int)Math.Round(normalizedValue * maxValue);
            value = Math.Clamp(value, 0, maxValue);
            return true;
        }
    }
}
