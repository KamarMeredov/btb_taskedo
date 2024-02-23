using BlogPlatform.Data.Models;
using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;

namespace BlogPlatform.Services
{
    public interface IAccountService
    {
        Task<LoginResponse> LoginAsync(LoginUserDTO user);
        Task<(RegistrationResponse response, bool succeed)> RegisterAsync(RegisterUserDTO user);

        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> CheckUserPassword(ApplicationUser user, string password);
    }
}
