using System.ComponentModel.DataAnnotations;
using To_Do.Validaion;

namespace To_Do.Data.Dto
{
    public class LoginRequestDto
    {
        [Required]
        [ValidateEmail(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required]
        [MinLength(8,ErrorMessage ="password can not be less than 8 ")]
        public string? Password { get; set; }


    }
}
