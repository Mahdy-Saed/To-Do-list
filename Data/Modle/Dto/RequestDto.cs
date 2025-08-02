using System.ComponentModel.DataAnnotations;
using To_Do.Validaion;

namespace To_Do.Data.Modle.Dto
{
    public class RequestDto
    {

        [Required(ErrorMessage = "User name can not be Empty")]
        [StringLength(50, ErrorMessage = "User name must be less than 50 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email can not be Empty")]

        [ValidateEmail(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public string? Password { get; set; }

    }
}
