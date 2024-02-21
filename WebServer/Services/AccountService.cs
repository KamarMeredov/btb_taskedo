using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebServer.DTO.ReponseObjects;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebServer.DTO;
using WebServer.Data.Models;

namespace WebServer.Services
{
    public class AccountService : IAccountService
    {
        IConfiguration _configuration;
        UserManager<ApplicationUser> _userManager;
        public AccountService(IConfiguration configuration, UserManager<ApplicationUser> userManager) 
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<(LoginResponse response, int statusCode)> LoginAsync(LoginUserDTO user)
        {

            await Task.Delay(2000);
            var identityUser = await _userManager.FindByEmailAsync(user.Email);

            if (identityUser == null)
            {
                return (new LoginResponse()
                {
                    Email = user.Email,
                    Message = "Incorrect email or password.",
                }, StatusCodes.Status404NotFound);
            }

            var password = await _userManager.CheckPasswordAsync(identityUser, user.Password);

            if (!password)
            {
                return (new LoginResponse()
                {
                    Message = "Incorrect email or password.",
                    Email = user.Email,
                }, StatusCodes.Status401Unauthorized);
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

            return (new LoginResponse()
            {
                Email = user.Email,
                Message = "Login succeed.",
                UserName = identityUser.UserName,
                AccessToken = tokenString
            }, StatusCodes.Status200OK);
        }

        public async Task<(RegistrationResponse response, int statusCode)> RegisterAsync(RegisterUserDTO user)
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
                }, StatusCodes.Status201Created);
            }

            return (new RegistrationResponse()
            {
                Email = identityUser.Email,
                Message = result.Errors.Select(x => x.Description).FirstOrDefault(),
                UserName = user.Name,
            }, StatusCodes.Status409Conflict);
        }
    }
}
