using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;
using BlogPlatform.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogPlatform.Controllers
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
            if (!ModelState.IsValid)
            {
                return new JsonResult(new RegistrationResponse()
                {
                    Message = ModelState.FirstOrDefault(x => x.Value.ValidationState == ModelValidationState.Invalid)
                    .Value.Errors.FirstOrDefault().ErrorMessage
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var result = await _accountService.RegisterAsync(userDTO);
            return new JsonResult(result.response) { StatusCode = result.statusCode };
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            if( !ModelState.IsValid )
            {
                return new JsonResult((new RegistrationResponse()
                {
                    Message = ModelState.FirstOrDefault(x => x.Value.ValidationState == ModelValidationState.Invalid)
                    .Value.Errors.FirstOrDefault().ErrorMessage,
                }))
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var result = await _accountService.LoginAsync(userDTO);
            return new JsonResult(result.response) { StatusCode = result.statusCode };
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
