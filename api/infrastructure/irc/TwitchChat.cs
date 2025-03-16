using api.Application.Services;
using api.Infrastructure.Persistence.TokensRepository;
using api.Infrastructure.Repositories.Twitch;

namespace api.Infrastructure.Irc
{
    public class TwitchChat
    {
        private readonly IrcService ircService;
        private static readonly int MaxAttempts = 3;

        public TwitchChat(ILogger<TwitchChat> logger, ITokensRepository tokens, TwitchRepository twitchRepository)
        {
            ircService = new IrcService(logger, tokens, twitchRepository);
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