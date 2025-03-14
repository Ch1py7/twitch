using System.Net.WebSockets;
using api.infrastructure.config;
using api.infrastructure.repositories.twitch;
using api.Services;
using Microsoft.Extensions.Options;

namespace api.infrastructure.irc
{
    public class TwitchChat : BackgroundService
    {
        private readonly string TWITCH_IRC_URL = "wss://irc-ws.chat.twitch.tv:443";
        private readonly string TOKEN = "oauth:";
        private readonly string NICKNAME = "bulbsum";
        private readonly string CHANNEL = "bulbsum";

        private readonly ClientWebSocket ws = new();
        private readonly ILogger<TwitchChat> logger;

        private readonly Config config;
        private readonly IrcService ircService;

        public TwitchChat(ILogger<TwitchChat> logger, TwitchRepository twitchRepository, IOptions<Config> config)
        {
            this.config = config.Value;
            this.logger = logger;

            this.ircService = new IrcService(this.ws, twitchRepository);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (string.IsNullOrEmpty(this.config.AccessToken) || string.IsNullOrEmpty(this.config.RefreshToken))
            {
                await Task.Delay(5000, stoppingToken);
            }

            logger.LogInformation("Available tokens. initialazing bot...");

            logger.LogInformation("connecting");

            await ws.ConnectAsync(new Uri(this.TWITCH_IRC_URL), stoppingToken);
            logger.LogInformation("connected");

            await this.ircService.SendMessage($"PASS {this.TOKEN}{this.config.AccessToken}");
            await this.ircService.SendMessage($"NICK {this.NICKNAME}");
            await this.ircService.SendMessage($"JOIN #{this.CHANNEL}");
            await this.ircService.SendMessage("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");

            await this.ircService.Listen(stoppingToken);
        }
    }
}