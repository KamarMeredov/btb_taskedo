using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO
{
    public class RegisterUserDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Login is required.")]
        [EmailAddress(ErrorMessage = "Login should be a valid email.")]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        [MinLength(8)]
        [MaxLength(20)]
        public string Password { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        [MinLength(8)]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
