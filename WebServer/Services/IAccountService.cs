using WebServer.DTO;
using WebServer.DTO.ReponseObjects;

namespace WebServer.Services
{
    public interface IAccountService
    {
        Task<(LoginResponse response, int statusCode)> LoginAsync(LoginUserDTO user);
        Task<(RegistrationResponse response, int statusCode)> RegisterAsync(RegisterUserDTO user);
    }
}
