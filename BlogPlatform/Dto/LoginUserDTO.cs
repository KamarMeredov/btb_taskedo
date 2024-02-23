using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.DTO
{
    public class LoginUserDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Login is required.")]
        [EmailAddress(ErrorMessage = "Login should be a valid email.")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
