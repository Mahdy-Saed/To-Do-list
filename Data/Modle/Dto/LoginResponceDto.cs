using To_Do.Authentication;

namespace To_Do.Data.Modle.Dto
{
    public class LoginResponceDto
    {
        // return the token and user information after login cause the frontend need to store token and user infromation
        public  TokenResponce? Token { get; set; }

        public UserDto? User { get; set; }


    }
}
