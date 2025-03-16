using api.Application.Services;
using api.Application.Services.TokenCache;
using api.Infrastructure.Repositories.Twitch;

namespace api.Infrastructure.Irc
{
    public class TwitchChat
    {
        private readonly IrcService ircService;
        private static readonly int MaxAttempts = 3;

        public TwitchChat(ILogger<TwitchChat> logger, TwitchRepository twitchRepository, ITokenCache tokenCache)
        {
            ircService = new IrcService(twitchRepository, logger, tokenCache);
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            await ircService.ConnectWebSocket(stoppingToken);

            for (int i = 0; i < MaxAttempts; i++)
            {
                await ircService.Listen(stoppingToken);
            }
        }
    }
}