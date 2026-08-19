namespace SchulApp.Models
{
    public sealed class PingPongPlayerMovementPayload
    {
        public double NormalizedTop { get; set; }
    }

    public static class PingPongPlayerMovementPosition
    {
        public static double Normalize(int top, int maxTop)
        {
            if (maxTop <= 0)
            {
                return 0;
            }

            int boundedTop = Math.Clamp(top, 0, maxTop);
            return (double)boundedTop / maxTop;
        }

        public static bool TryDenormalize(
            double normalizedTop,
            int maxTop,
            out int top)
        {
            top = 0;

            if (
                maxTop < 0 ||
                double.IsNaN(normalizedTop) ||
                double.IsInfinity(normalizedTop) ||
                normalizedTop < 0 ||
                normalizedTop > 1)
            {
                return false;
            }

            top = (int)Math.Round(normalizedTop * maxTop);
            top = Math.Clamp(top, 0, maxTop);
            return true;
        }
    }
}
