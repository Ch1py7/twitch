using System.Text.Json;
using api.Domain.Entities;

namespace api.Application.Services.TokenCache
{
    public class TokenCache : ITokenCache
    {
        private static readonly string FileCachePath = @"D:\tokens.json";
        public Token Current { get; private set; }

        public TokenCache()
        {
            Current = new Token { AccessToken = null, RefreshToken = null, ObtainedAt = 0, ExpiresIn = 0 };

            if (File.Exists(FileCachePath))
            {
                ReadToken();
            };
        }

        private void ReadToken()
        {
            var file = File.ReadAllText(FileCachePath);
            var fileData = JsonSerializer.Deserialize<Token>(file);
            if (fileData != null)
            {
                Current = fileData;
            }
        }

        public void WriteToken(Token token)
        {
            Current = token;
            var dataAsString = JsonSerializer.Serialize(Current);
            File.WriteAllText(FileCachePath, dataAsString);
        }
    }
}
