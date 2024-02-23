using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;

namespace BlogPlatform.Services
{
    public interface IAccountService
    {
        Task<(LoginResponse response, int statusCode)> LoginAsync(LoginUserDTO user);
        Task<(RegistrationResponse response, int statusCode)> RegisterAsync(RegisterUserDTO user);
    }
}
