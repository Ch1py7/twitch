using api.Models;

namespace api.Services.TokenCache
{
    public interface ITokenCache
    {
        Token Current { get; }
        void WriteToken(Token token);
    }
}
