using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public TwitchChat(ILogger<TwitchChat> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("connecting");

            await ws.ConnectAsync(new Uri(this.TWITCH_IRC_URL), stoppingToken);
            logger.LogInformation("connected");

            await SendMessage($"PASS {this.TOKEN}");
            await SendMessage($"NICK {this.NICKNAME}");
            await SendMessage($"JOIN #{this.CHANNEL}");
            await SendMessage("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");

            await Listen(stoppingToken);
        }

        private async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\r\n");
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Listen(CancellationToken stoppingToken)
        {
            var buffer = new byte[8192];

            while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                this.logger.LogInformation(message);

                if (message.Contains("PING :tmi.twitch.tv"))
                {
                    Console.WriteLine("receiving ping, sending pong");
                    await SendMessage("PONG :tmi.twitch.tv");
                }

                var match = Regex.Match(message, @"@.*;emotes=([^;]*);.*PRIVMSG #\w+ :(.*)");
                if (match.Success)
                {
                    string username = match.Groups[1].Value;
                    string text = match.Groups[2].Value;

                    string jsonOutput = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
                    //this.logger.LogInformation($"JSON Message: {jsonOutput}");
                }
            }
        }
    }
}