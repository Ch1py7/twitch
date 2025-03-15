using System.Net.WebSockets;
using api.Config;
using api.infrastructure.repositories.twitch;
using api.Services;
using Microsoft.Extensions.Options;

namespace api.infrastructure.irc
{
    public class TwitchChat : BackgroundService
    {
        private readonly string TWITCH_IRC_URL = "wss://irc-ws.chat.twitch.tv:443";
        private readonly string NICKNAME = "bulbsum";
        private readonly string CHANNEL = "bulbsum";

        private readonly ClientWebSocket ws = new();
        private readonly ILogger<TwitchChat> logger;

        private readonly TwitchConfig config;
        private readonly IrcService ircService;

        public TwitchChat(ILogger<TwitchChat> logger, TwitchRepository twitchRepository, IOptions<TwitchConfig> config)
        {
            this.config = config.Value;
            this.logger = logger;

            ircService = new IrcService(ws, twitchRepository, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (string.IsNullOrEmpty(config.AccessToken) || string.IsNullOrEmpty(config.RefreshToken))
            {
                logger.LogWarning("Tokens missing, retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }

            logger.LogInformation("Connecting");
            await ws.ConnectAsync(new Uri(TWITCH_IRC_URL), stoppingToken);
            logger.LogInformation("Connected");

            await ircService.SendMessage($"PASS oauth:{config.AccessToken}");
            await ircService.SendMessage($"NICK {NICKNAME}");
            await ircService.SendMessage($"JOIN #{CHANNEL}");
            await ircService.SendMessage("CAP REQ :twitch.tv/membership twitch.tv/tags twitch.tv/commands");

            await ircService.Listen(stoppingToken);
        }
    }
}