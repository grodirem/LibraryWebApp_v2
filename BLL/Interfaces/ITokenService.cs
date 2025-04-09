using DAL.Models;

namespace BLL.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user, IList<string> roles);
    string GenerateRefreshToken();
}
