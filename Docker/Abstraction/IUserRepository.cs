using Docker.Dto;
using Docker.Models;

namespace Docker.Abstraction
{
    public interface IUserRepository
    {
        int AddUser(UserDto user);
        string CheckUser(LoginDto login);
    }
}