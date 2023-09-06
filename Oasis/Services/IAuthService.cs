using Oasis.DataOfToken;
using TryJWT.Models;

namespace Oasis.Services
{
    public interface IAuthService
    {
        Task<AuthModel> Registeration(RegisterModel model);
        Task<AuthModel> GetToken(TokenRequestModel model);
    }
}
