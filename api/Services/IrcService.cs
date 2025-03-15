using System.Net.WebSockets;
using System.Text;
using api.infrastructure.irc;
using api.infrastructure.parser;
using api.infrastructure.repositories.twitch;
using api.Models;

namespace api.Services
{
    public class IrcService
    {
        private readonly ClientWebSocket ws;
        private TwitchRepository twitchRepository;
        private readonly ILogger<TwitchChat> logger;

        public IrcService(ClientWebSocket ws, TwitchRepository twitchRepository, ILogger<TwitchChat> logger)
        {
            this.ws = ws;
            this.twitchRepository = twitchRepository;
            this.logger = logger;
        }

        public async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\r\n");
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task Listen(CancellationToken stoppingToken)
        {
            var buffer = new byte[8192];

            while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (message == ":tmi.twitch.tv NOTICE * :Login authentication failed")
                {
                    await twitchRepository.RefreshToken("");
                }
                if (message == "PING :tmi.twitch.tv\r\n")
                {
                    logger.LogInformation("receiving ping, sending pong");
                    await SendMessage("PONG :tmi.twitch.tv");
                }
                if (message != ":tmi.twitch.tv 001 bulbsum :Welcome, GLHF!\r\n:tmi.twitch.tv 002 bulbsum :Your host is tmi.twitch.tv\r\n:tmi.twitch.tv 003 bulbsum :This server is rather new\r\n:tmi.twitch.tv 004 bulbsum :-\r\n:tmi.twitch.tv 375 bulbsum :-\r\n:tmi.twitch.tv 372 bulbsum :You are in a maze of twisty passages, all alike.\r\n:tmi.twitch.tv 376 bulbsum :>\r\n")
                {
                    MessageParser messageParser = new MessageParser(message);

                    switch (messageParser.messageType)
                    {
                        case ("PRIVMSG"):
                            TwitchMessage parsedMessage = messageParser.ParseTwitchMessage();
                            logger.LogInformation($"{parsedMessage.UserInfo.Message}");
                            break;
                    }
                }
            }
        }
    }
}
