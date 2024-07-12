using Docker.Models;

namespace Docker.Abstraction
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
