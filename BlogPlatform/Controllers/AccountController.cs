using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(GetModelErrors())
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var result = await _accountService.RegisterAsync(userDTO);

            if (result.succeed)
            {
                return new JsonResult(result.response) { StatusCode = StatusCodes.Status201Created };
            }
            else
            {
                return new JsonResult(result.response) { StatusCode = StatusCodes.Status409Conflict };
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            if( !ModelState.IsValid )
            {
                return new JsonResult(GetModelErrors())
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var user = await _accountService.GetUserByEmail(userDTO.Email);
            if (user == null)
            {
                return new JsonResult(new LoginResponse()
                {
                    Email = userDTO.Email,
                    Message = "Incorrect email or password.",
                })
                { StatusCode = StatusCodes.Status401Unauthorized};
            }
            var truePassword = await _accountService.CheckUserPassword(user, userDTO.Password);
            
            if (!truePassword)
            {
                return new JsonResult(new LoginResponse()
                {
                    Email = userDTO.Email,
                    Message = "Incorrect email or password.",
                })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var result = await _accountService.LoginAsync(userDTO);
            return new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetMe()
        {
            // TODO add any other needed information for User
            return new JsonResult(new
            {
                Name = User.FindFirstValue(ClaimTypes.Name),
                Login = User.FindFirstValue(ClaimTypes.Email),
            });
        }


        private List<List<ModelError>> GetModelErrors()
        {
            return ModelState.Where(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                   .Select(x => x.Value!.Errors.ToList())
                   .ToList();
        }

    }
}
