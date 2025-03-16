using System.Net.WebSockets;
using System.Text;
using api.Domain.Entities;
using api.Infrastructure.Irc;
using api.Infrastructure.Parser;
using api.Infrastructure.Persistence.TokensRepository;
using api.Infrastructure.Persistence.TwitchRepository;

namespace api.Application.Services
{
    public class IrcService
    {
        private ClientWebSocket ws = new();
        //private TwitchRepository twitchRepository;
        private readonly string CHANNEL = "bulbsum";
        private readonly ILogger<TwitchChat> logger;
        private readonly string NICKNAME = "bulbsum";
        private readonly string TWITCH_IRC_URL = "wss://irc-ws.chat.twitch.tv:443";
        private readonly ITokensRepository tokens_repository;
        private readonly TwitchRepository twitchRepository;

        public IrcService(ILogger<TwitchChat> logger, ITokensRepository tokens, TwitchRepository twitchRepository)
        {
            this.twitchRepository = twitchRepository;
            this.logger = logger;
            tokens_repository = tokens;
        }

        public async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\r\n");
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task Listen(CancellationToken stoppingToken)
        {
            var buffer = new byte[8192];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (ws.State != WebSocketState.Open)
                    {
                        await ReconnectWebSocket();
                        await AuthenticateAndJoin();
                        logger.LogInformation("Reauthenticated and joined channel.");
                    }

                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (message == ":tmi.twitch.tv NOTICE * :Login authentication failed\r\n")
                    {
                        logger.LogWarning("Authentication failed, refreshing token.");
                        await twitchRepository.RefreshToken();
                        await ReconnectWebSocket();
                        await AuthenticateAndJoin();
                    }
                    if (message == "PING :tmi.twitch.tv\r\n")
                    {
                        logger.LogInformation("receiving ping, sending pong");
                        await SendMessage("PONG :tmi.twitch.tv");
                    }
                    else if (message != ":tmi.twitch.tv 001 bulbsum :Welcome, GLHF!\r\n:tmi.twitch.tv 002 bulbsum :Your host is tmi.twitch.tv\r\n:tmi.twitch.tv 003 bulbsum :This server is rather new\r\n:tmi.twitch.tv 004 bulbsum :-\r\n:tmi.twitch.tv 375 bulbsum :-\r\n:tmi.twitch.tv 372 bulbsum :You are in a maze of twisty passages, all alike.\r\n:tmi.twitch.tv 376 bulbsum :>\r\n")
                    {
                        MessageParser messageParser = new(message);

                        switch (messageParser.messageType)
                        {
                            case "PRIVMSG":
                                TwitchMessage parsedMessage = messageParser.ParseTwitchMessage();
                                logger.LogInformation($"{parsedMessage.UserInfo.Message}");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in Listen: {ex.Message}");

                }
            }
        }

        public async Task ConnectWebSocket(CancellationToken stoppingToken)
        {
            logger.LogInformation("Connecting");
            await ws.ConnectAsync(new Uri(TWITCH_IRC_URL), stoppingToken);
            await AuthenticateAndJoin();

            logger.LogInformation("Connected");
        }

        private async Task ReconnectWebSocket()
        {
            if (ws != null)
            {
                try
                {
                    if (ws.State == WebSocketState.Open || ws.State == WebSocketState.Connecting)
                    {
                        logger.LogInformation("Closing existing WebSocket connection.");
                        await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Error closing WebSocket: {ex.Message}");
                }
                finally
                {
                    ws.Dispose();
                    ws = new ClientWebSocket();
                }
            }

            logger.LogInformation("Waiting before reconnecting...");
            await Task.Delay(2000);

            logger.LogInformation("Reconnecting WebSocket...");
            await ws.ConnectAsync(new Uri(TWITCH_IRC_URL), CancellationToken.None);
        }

        private async Task AuthenticateAndJoin()
        {
            await SendMessage($"PASS oauth:{tokens_repository.GetToken().AccessToken}");
            await SendMessage($"NICK {NICKNAME}");
            await SendMessage($"JOIN #{CHANNEL}");
            await SendMessage("CAP REQ :twitch.tv/membership twitch.tv/tags twitch.tv/commands");
        }

    }
}
