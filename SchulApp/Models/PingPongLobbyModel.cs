namespace SchulApp.Models
{
    public sealed class PingPongLobbyModel
    {
        public string Code { get; set; } = string.Empty;
        public int HostUserId { get; set; }
        public string HostUsername { get; set; } = string.Empty;
        public int? GuestUserId { get; set; }
        public string? GuestUsername { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string Status { get; set; } = "Waiting";

        public bool IsReady =>
            GuestUserId.HasValue &&
            string.Equals(Status, "Ready", StringComparison.OrdinalIgnoreCase);
    }
}
