using System.Text.Json;
using api.Config;
using api.Models;
using Microsoft.Extensions.Options;

namespace api.infrastructure.repositories.twitch
{
    public class TwitchRepository
    {
        private readonly string twitch_url = "https://id.twitch.tv/oauth2";
        public readonly TwitchConfig config;
        private readonly HttpClient client;

        public TwitchRepository(HttpClient httpClient, IOptions<TwitchConfig> config)
        {
            client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.config = config.Value;
        }

        public async Task<TwitchToken> GetToken(string code)
        {
            config.GrantType = "authorization_code";

            var body = new Dictionary<string, string>
            {
                { "client_id", config.ClientId },
                { "client_secret", config.Secret },
                { "grant_type", config.GrantType },
                { "code", code },
                { "redirect_uri", config.RedirectUri }
            };
            var content = new FormUrlEncodedContent(body);
            var response = await client.PostAsync($"{twitch_url}/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            config.AccessToken = tokenData.Access_Token;
            config.RefreshToken = tokenData.Refresh_Token;

            return tokenData;
        }
        public async Task<TwitchToken> RefreshToken(string? refreshToken)
        {
            config.GrantType = "refresh_token";

            var body = new Dictionary<string, string>
            {
                { "client_id", config.ClientId },
                { "client_secret", config.Secret },
                { "grant_type", config.GrantType },
                { "refresh_token", !string.IsNullOrEmpty(refreshToken) ? refreshToken : config.RefreshToken }
            };
            var content = new FormUrlEncodedContent(body);
            var response = await this.client.PostAsync($"{this.twitch_url}/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            config.AccessToken = tokenData.Access_Token;
            config.RefreshToken = tokenData.Refresh_Token;

            return tokenData;
        }
        public async Task<TwitchToken> GetAuth(string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{this.twitch_url}/validate");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", accessToken);

            var response = await this.client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching information: {response.StatusCode} - {responseString}");
            }

            var tokenData = JsonSerializer.Deserialize<TwitchToken>(responseString);

            config.AccessToken = tokenData.Access_Token;
            config.RefreshToken = tokenData.Refresh_Token;

            return tokenData;
        }
    }
}