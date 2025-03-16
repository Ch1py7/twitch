using api.Domain.Entities;

namespace api.Infrastructure.Persistence.TokensRepository
{
    public class TokensRepository : ITokensRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TokensRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private AppDbContext GetDbContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public Token GetToken()
        {
            using var context = GetDbContext();
            return context.Tokens.OrderByDescending(t => t.ObtainedAt).FirstOrDefault();
        }

        public Token? GetTokenByRefreshToken(string RefreshToken)
        {
            using var context = GetDbContext();
            return context.Tokens.Find(RefreshToken);
        }

        public void UpdateToken(Token UpdatedToken)
        {
            using var context = GetDbContext();
            var token = context.Tokens.FirstOrDefault(t => t.RefreshToken == UpdatedToken.RefreshToken);

            if (token != null)
            {
                token.AccessToken = UpdatedToken.AccessToken;
                token.ExpiresIn = UpdatedToken.ExpiresIn;
                token.ObtainedAt = UpdatedToken.ObtainedAt;

                context.SaveChanges();
            }
        }

        public void CreateToken(Token Token)
        {
            using var context = GetDbContext();
            context.Tokens.Add(Token);
            context.SaveChanges();
        }

        public void Save()
        {
            using var context = GetDbContext();
            context.SaveChanges();
        }
    }
}
