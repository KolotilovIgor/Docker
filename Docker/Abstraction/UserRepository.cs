using Docker.Dto;
using Docker.Models;
using Docker.DB;
using System.Text;
using System.Security.Cryptography;

namespace Docker.Abstraction
{
    public class UserRepository : IUserRepository
    {
        private readonly ITokenService _tokenService;

        public UserRepository(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public int AddUser(UserDto user)
        {
            using (var context = new UserContext())
            {
                if (context.Users.Any(x => x.Name == user.Name))
                    throw new Exception("User is already exist!");
                if (user.Role == UserRoleDto.Admin)
                    if (context.Users.Any(x => x.RoleId == RoleId.Admin))
                        throw new Exception("Admin is already exist!");

                var entity = new User { Name = user.Name, RoleId = (RoleId)user.Role };

                entity.Salt = new byte[16];
                new Random().NextBytes(entity.Salt);
                var data = Encoding.UTF8.GetBytes(user.Password).Concat(entity.Salt).ToArray();

                entity.Password = new SHA512Managed().ComputeHash(data);
                context.Users.Add(entity);
                context.SaveChanges();

                return entity.Id;
            }
        }

        public string CheckUser(LoginDto login)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Name == login.Name);

                if (user == null) throw new Exception("No user like this!");

                var data = Encoding.UTF8.GetBytes(login.Password).Concat(user.Salt).ToArray();
                var hash = new SHA512Managed().ComputeHash(data);

                if (user.Password.SequenceEqual(hash))
                    return _tokenService.GenerateToken(user);

                throw new Exception("Wrong password!");
            }
        }
    }
}
