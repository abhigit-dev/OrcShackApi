using OrcShackApi.Core.Models;

namespace OrcShackApi.Web.Jwt
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
