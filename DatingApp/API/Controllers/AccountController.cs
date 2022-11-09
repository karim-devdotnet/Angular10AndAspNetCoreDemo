using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _DataContext;

        public AccountController(DataContext dataContext)
        {
            _DataContext = dataContext;
        }


        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest($"Username {registerDto.UserName} is taken.");

            using (var hmac = new HMACSHA512())
            {
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };

                _DataContext.Users.Add(appUser);
                await _DataContext.SaveChangesAsync();

                return appUser;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            var user = await _DataContext.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("invalid username.");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password.");
            }

            return user;
        }

        private Task<bool> UserExists(string userName)
        {
            return _DataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}
