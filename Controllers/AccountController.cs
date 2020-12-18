using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto userInfo)
        {
            if (await UsernameExists(userInfo.Username)) return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();
            AppUser user = new AppUser
            {
                UserName = userInfo.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userInfo.Password)),
                PasswordSalt = hmac.Key
            };

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();
            return new UserDto 
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UsernameExists(string username)
        {
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto UserInfo)
        {
            // check if we have this username
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == UserInfo.Username);
            if (user == null) return Unauthorized("Usename is invalid");
            // check for password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            byte[] ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(UserInfo.Password));
            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect Password");
            }
            return new UserDto 
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }
    }
}