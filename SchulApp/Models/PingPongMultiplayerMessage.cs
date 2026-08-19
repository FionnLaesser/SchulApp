using System.Text.Json;

namespace SchulApp.Models
{
    public sealed class PingPongMultiplayerMessage
    {
        public string Type { get; set; } = string.Empty;
        public string GameCode { get; set; } = string.Empty;
        public int SenderUserId { get; set; }
        public long Sequence { get; set; }
        public long TimestampUnixMilliseconds { get; set; }
        public JsonElement Payload { get; set; }
    }

    public static class PingPongMultiplayerMessageTypes
    {
        public const string Lobby = "Lobby";
        public const string PlayerMovement = "PlayerMovement";
        public const string BallState = "BallState";
        public const string Score = "Score";
        public const string GameStart = "GameStart";
        public const string GameEnd = "GameEnd";
        public const string PauseRequest = "PauseRequest";
        public const string PauseState = "PauseState";

        public static bool IsSupported(string? messageType)
        {
            return messageType == Lobby ||
                messageType == PlayerMovement ||
                messageType == BallState ||
                messageType == Score ||
                messageType == GameStart ||
                messageType == GameEnd ||
                messageType == PauseRequest ||
                messageType == PauseState;
        }
    }
}
