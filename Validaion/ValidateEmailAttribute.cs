using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
 namespace To_Do.Validaion
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class ValidateEmailAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var email = value as string; // this will try to cast the value to a string or null it it fails

            var EmailRegex= @"[^@\s]+@[^@\s]+\.[^@\s]+";

            if(email == null  )   return new ValidationResult("Email can not be empty");
            

            if ( !Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase)) 
            {
                return new ValidationResult("Invalid Email format");

            }
            return ValidationResult.Success; // if the email is valid, return success

        }


    }
}
