namespace Chatbot_Dev.Entities
{ // Store per-user session info
    public class UserSession
    {
        public string State { get; set; } = "language";
        public string Language { get; set; }
        public SignUpData SignUpData { get; set; } = new();
        public string LoginEmail { get; set; }
        public string LoginCode { get; set; }
    }
}
