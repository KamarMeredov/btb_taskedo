using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebServer.DTO;
using WebServer.DTO.ReponseObjects;
using WebServer.Services;
using System.Security.Claims;

namespace WebServer.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        IConfiguration _configuration;
        IAccountService _accountService;
        public AccountController(IConfiguration configuration, IAccountService accountService)
        {
            _configuration = configuration;
            _accountService = accountService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new JsonResult(new RegistrationResponse()
                    {
                        Message = ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage
                    })
                    { StatusCode = StatusCodes.Status400BadRequest};
                }

                var result = await _accountService.RegisterAsync(userDTO);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex)
            {
                return new JsonResult(new RegistrationResponse()
                {
                    Message = "Something went wrong on server",
                })
                { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new JsonResult((new RegistrationResponse()
                    {
                        Message = ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage,
                    }))
                    { StatusCode = StatusCodes.Status400BadRequest};
                }

                var result = await _accountService.LoginAsync(userDTO);
                return new JsonResult(result.response) { StatusCode = result.statusCode };
            }
            catch (Exception ex)
            {
                return new JsonResult(new RegistrationResponse()
                {
                    Message = "Something went wrong on server",
                })
                { StatusCode = StatusCodes.Status500InternalServerError };
                
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            return new JsonResult(new
            {
                Name = User.FindFirstValue(ClaimTypes.Name),
                Login = User.FindFirstValue(ClaimTypes.Email),
            });
        }

    }
}
