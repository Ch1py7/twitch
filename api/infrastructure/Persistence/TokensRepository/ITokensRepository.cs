using api.Domain.Entities;
namespace api.Infrastructure.Persistence.TokensRepository
{
    public interface ITokensRepository
    {
        public Token GetToken();
        public Token? GetTokenByRefreshToken(string RefreshToken);
        public void UpdateToken(Token UpdatedToken);
        public void CreateToken(Token Token);
        public void Save();
    }
}
