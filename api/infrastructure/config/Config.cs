namespace api.infrastructure.config
{
    public class Config
    {
        public string ClientId { get; } = System.Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Machine) ?? "";
        public string Secret { get; } = System.Environment.GetEnvironmentVariable("Secret", EnvironmentVariableTarget.Machine) ?? "";
        public string GrantType { get; set; } = "client_credentials";
        public string RedirectUri { get; set; } = "http://localhost:3000";
        public string Scope { get; set; } = "chat:read"; // %3A
        public string TokenType { get; set; } = "bearer";
    }
}