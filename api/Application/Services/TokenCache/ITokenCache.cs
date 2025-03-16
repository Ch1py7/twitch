using api.Domain.Entities;

namespace api.Application.Services.TokenCache
{
    public interface ITokenCache
    {
        Token Current { get; }
        void WriteToken(Token token);
    }
}
