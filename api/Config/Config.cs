namespace api.Config
{
    public class TwitchConfig
    {
        public string ClientId { get; set; } = "";
        public string Secret { get; set; } = "";
        public string GrantType { get; set; } = "client_credentials";
        public string RedirectUri { get; set; } = "http://localhost:3000";
        public string Scope { get; set; } = "chat:read";
        public string TokenType { get; set; } = "bearer";
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
