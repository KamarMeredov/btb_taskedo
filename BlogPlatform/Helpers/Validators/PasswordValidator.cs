using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BlogPlatform.Helpers.Validators
{
    public class PasswordValidator : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var text = value as string;
            if (text == null)
            {
                return false;
            }

            if (!text.Any(x => char.IsDigit(x)))
            {
                ErrorMessage = "Password must contain at least one digit.";
                return false;
            }

            if (!text.Any(x => char.IsUpper(x)))
            {
                ErrorMessage = "Password must contain at least one upper case letter.";
                return false;
            }

            if (!text.Any(x => char.IsLower(x)))
            {
                ErrorMessage = "Password must contain at least one lower case letter.";
            }

            if (text.All(x => char.IsLetterOrDigit(x)))
            {
                ErrorMessage = "Password must contain at least one non alphanumeric letter.";
                return false;
            }

            return true;
        }
    }
}
