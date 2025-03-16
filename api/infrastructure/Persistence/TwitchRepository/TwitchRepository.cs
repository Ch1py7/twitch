using System.Text.Json;
using api.Application;
using api.Domain.Entities;
using api.Infrastructure.Persistence.TokensRepository;
using Microsoft.Extensions.Options;

namespace api.Infrastructure.Persistence.TwitchRepository
{
    public class TwitchRepository
    {
        private readonly string twitch_url = "https://id.twitch.tv/oauth2";
        private readonly HttpClient client;
        private readonly TwitchConfig config;
        private readonly ITokensRepository tokens_repository;

        public TwitchRepository(HttpClient httpClient, IOptions<TwitchConfig> config, ITokensRepository tokens)
        {
            this.config = config.Value;
            client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            tokens_repository = tokens;
        }

        public async Task<TwitchToken> GetToken(string code)
        {
            var body = new Dictionary<string, string>
            {
                { "client_id", config.ClientId },
                { "client_secret", config.Secret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", "http://localhost:3000" }
            };
            var content = new FormUrlEncodedContent(body);
            var response = await client.PostAsync($"{twitch_url}/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            UpdateToken(tokenData);

            return tokenData;
        }
        public async Task<TwitchToken> RefreshToken(string? refreshToken = "")
        {
            var dbToken = tokens_repository.GetToken().RefreshToken;
            var body = new Dictionary<string, string>
            {
                { "client_id", config.ClientId },
                { "client_secret", config.Secret },
                { "grant_type", "refresh_token" },
                { "refresh_token", !string.IsNullOrEmpty(refreshToken) ? refreshToken : dbToken }
            };
            var content = new FormUrlEncodedContent(body);
            var response = await client.PostAsync($"{twitch_url}/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            UpdateToken(tokenData);

            return tokenData;
        }
        public async Task<TwitchToken> GetAuth(string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{twitch_url}/validate");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", accessToken);

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching information: {response.StatusCode} - {responseString}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            UpdateToken(tokenData);

            return tokenData;
        }

        private void UpdateToken(TwitchToken tokenData)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Token newToken = new Token
            {
                AccessToken = tokenData.AccessToken,
                RefreshToken = tokenData.RefreshToken,
                ExpiresIn = tokenData.ExpiresIn,
                ObtainedAt = now,
            };

            if (tokens_repository != null)
            {
                tokens_repository.UpdateToken(newToken);
            }
            else
            {
                tokens_repository.CreateToken(newToken);
            }
        }
    }
}