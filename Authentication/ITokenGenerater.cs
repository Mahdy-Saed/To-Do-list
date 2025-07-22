using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using To_Do.Entity;

namespace To_Do.Authntication
{
    public interface ITokenGenerater
    {
        // in genereal all the functions in interface its Public 
          string CreateAccessToken(User User);

          string CreateRefreshToken();

          bool ValidateRefreshToken(string RefreshToken);


    }

    public class TokenGenerater(IConfiguration configuration) : ITokenGenerater  // ioption is also good option rather than Iconfiguraion
                                                                                 // and can generate specific configurations.
    {
        public string CreateAccessToken(User User)
        {
           
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier,User.Id.ToString()),
                new Claim(ClaimTypes.Name,User.UserName),
                new Claim(ClaimTypes.Email,User.Email),
                new Claim(ClaimTypes.Role,User.Role),
            };

            byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("jwt:key").Value !); // GET THE KEY FROM THE SETTING

            var securityKey = new SymmetricSecurityKey(key);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);  //now we generate the credntials


            var tokenDescription = new JwtSecurityToken(
                    issuer: "YourApp",
                    audience: "ClientApp",
                     claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                    
                    );


            return new JwtSecurityTokenHandler().WriteToken(tokenDescription); // this will return the token as string.
        }

        public string CreateRefreshToken()
        {
            byte[] arayNumber = new byte[32]; // 32 byte its 256 bits
                                              // 

            using(var random=RandomNumberGenerator.Create())
            {
                random.GetBytes(arayNumber); // this will fill the array with random bytes
            }

            return Convert.ToBase64String(arayNumber); // convert the byte array to base64 string


        }

        public bool ValidateRefreshToken(string RefreshToken)
        {
            throw new NotImplementedException();
        }
    }


}
