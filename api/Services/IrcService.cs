using System.Net.WebSockets;
using System.Text;
using api.infrastructure.repositories.twitch;
using api.Models;
using api.infrastructure.parser;

namespace api.Services
{
    public class IrcService
    {
        private byte[] buffer = new byte[4096];
        private readonly ClientWebSocket ws;
        private TwitchRepository twitchRepository;

        public IrcService(ClientWebSocket ws, TwitchRepository twitchRepository)
        {
            this.ws = ws;
            this.twitchRepository = twitchRepository;
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
                MessageParser parsedMessages = new MessageParser(message);

                if (message.Contains(":tmi.twitch.tv NOTICE * :Login authentication failed"))
                {
                    await this.twitchRepository.RefreshToken("");
                }
                else
                {
                    if (message.Contains("PING :tmi.twitch.tv"))
                    {
                        Console.WriteLine("receiving ping, sending pong");
                        await SendMessage("PONG :tmi.twitch.tv");
                    }
                    else
                    {
                        if (!message.Contains("Welcome") && !message.Contains("JOIN") && !message.Contains("CAP * ACK") && !message.Contains("NAMES"))
                        {
                            TwitchMessage parsedToTwitch = parsedMessages.ToTwitchMessage();
                            Console.WriteLine(parsedToTwitch.Channel);
                            Console.WriteLine(parsedToTwitch.UserInfo.UserName);
                            Console.WriteLine(parsedToTwitch.UserInfo.Color);
                        }
                    }

                }
            }
        }
    }
}
