using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BlogPlatform.DTO.ReponseObjects;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BlogPlatform.DTO;
using BlogPlatform.Data.Models;

namespace BlogPlatform.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(IConfiguration configuration, UserManager<ApplicationUser> userManager) 
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckUserPassword(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<LoginResponse> LoginAsync(LoginUserDTO user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);

            if (identityUser == null)
            {
                throw new ArgumentNullException(nameof(identityUser));
            }

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var lifetime = int.Parse(_configuration["Jwt:Lifetime"]);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id.ToString()),
                new Claim(ClaimTypes.Email, identityUser.Email),
            };
            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    now,
                    now.AddMinutes(lifetime),
                    new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new LoginResponse()
            {
                Email = user.Email,
                Message = "Login succeed.",
                UserName = identityUser.UserName,
                AccessToken = tokenString
            };
        }

        public async Task<(RegistrationResponse response, bool succeed)> RegisterAsync(RegisterUserDTO user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var identityUser = new ApplicationUser()
            {
                UserName = user.Name,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (result.Succeeded)
            {
                return (new RegistrationResponse()
                {
                    Email = identityUser.Email,
                    Message = "Registration succeed.",
                    UserName = user.Name,
                }, true);
            }

            return (new RegistrationResponse()
            {
                Email = identityUser.Email,
                Message = result.Errors.Select(x => x.Description).FirstOrDefault(),
                UserName = user.Name,
            }, false);
        }
    }
}
