using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Generators;
using To_Do.Authentication;
using To_Do.Authntication;
using To_Do.Data.Dto;
 using To_Do.Data.Repositery;
using To_Do.Entity;

namespace To_Do.Services
{
    public interface IUserServices
    {
        Task<TokenResponce?> CreateUser(RequestDto requestDto);

        Task<User?> GetUserById(Guid id);

        Task<IEnumerable<User>> GetAllUsers();

        Task<User?> GetUserTask(Guid id); // this will return the user with his tasks
        Task<LoginResponceDto?>Login(LoginRequestDto loginRequestDto);

        Task<User?> UpdateUser(Guid id, User user);


        Task<bool> DeleteUser(Guid id);

        Task<bool> DeleteAllUser();


        Task<bool> ChangePasswordAsync(Guid userID,string CurrentPassword,string NewPassword);


    }
    public class UserServices : IUserServices
    {
        private readonly IUserRepositery _userRepositery;
        private readonly   IPasswordHasher _passwordHasher;
        private readonly ITokenGenerater _tokenGenerater;

        
        public UserServices(IUserRepositery userRepositery,IPasswordHasher passwordHaser,ITokenGenerater tokenGenerater)
        {
            _passwordHasher = passwordHaser ?? throw new ArgumentNullException(nameof(passwordHaser));
            _tokenGenerater = tokenGenerater ?? throw new ArgumentNullException(nameof(tokenGenerater));
            _userRepositery = userRepositery;
        }
        public async Task<TokenResponce?> CreateUser(RequestDto  requestDto)
        {
            if (requestDto == null) throw  new  ArgumentNullException(nameof(requestDto));

            if (string.IsNullOrEmpty(requestDto.Password)){            
            throw new ArgumentNullException(requestDto.Password,"Password can not be null or empty");
            }
            var existingUser = await _userRepositery.GetByEmail(requestDto.Email!);  // checking if the user is already registered
            if(existingUser != null)
            {
                return null; // or throw an exception if you prefer
            }

            var passwordHash = _passwordHasher.Hash(requestDto.Password);
            var user = new User()
            {
                UserName = requestDto.UserName,
                Email = requestDto.Email,
                PasswordHash = passwordHash
            };
            var   RefreshToken = _tokenGenerater.CreateRefreshToken();  
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // set the expiry time for the refresh token
            await _userRepositery.AddAsync(user);
            TokenResponce tokenResponse = new TokenResponce()
            {
                // just for test 
                User_Id= user.Id,
                Access_Token = _tokenGenerater.CreateAccessToken(user), //To Do: replace with actual token generation logic
                Refresh_Token = _tokenGenerater.CreateRefreshToken(),
            };
            user.RefreshToken = tokenResponse.Refresh_Token;
             // this will save the user in the database
            return tokenResponse;
        }

        public async Task<bool> DeleteAllUser()
        {
            await _userRepositery.DeleteAllAsync();
            return true;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await  _userRepositery.GetByIdAsync(id);

            if (user == null) return false;

            await _userRepositery.DeleteAsync(id);  //inside the user repositery will save changes

            return true;  
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        { 
            return await _userRepositery.GetAllAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            var user =await  _userRepositery.GetByIdAsync(id);
            
                if(user is null) return  null;
                
                return    user;

        }




        public async Task<LoginResponceDto?> Login(LoginRequestDto loginRequestDto)
        {
            User? user = await _userRepositery.GetByEmail(loginRequestDto.Email!);
            if (user is null)  return null;


            var passwordVerificationResult = _passwordHasher.verify(loginRequestDto.Password!, user.PasswordHash!);

            if (!passwordVerificationResult) return null;

            LoginResponceDto Result = new LoginResponceDto()
            {
                Token = new TokenResponce()
                {
                    User_Id= user.Id,
                    Refresh_Token = _tokenGenerater.CreateRefreshToken(),
                    Access_Token = _tokenGenerater.CreateAccessToken(user),
                },
                User = new UserResponceDto()
                {
                    Id = user.Id,
                    Username= user.UserName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
            await _userRepositery.UpdateAsync(user); // update the user with the new refresh token
            return Result; // return the login response with the token and user information
        }




        public async Task<User?> UpdateUser(Guid id, User userRequest)
        {
          var user = await _userRepositery.GetByIdAsync(id);
            if (user is  null || userRequest is null ) return null;

            user.UserName = userRequest.UserName;
            user.Email = userRequest.Email;
            user.PasswordHash = userRequest.PasswordHash;
             
            await _userRepositery.UpdateAsync(user);
            return user;  //return the updated user
        }

        public async  Task<bool> ChangePasswordAsync(Guid userID, string CurrentPassword,string NewPassword)
        {
            var user = await _userRepositery.GetByIdAsync(userID);
            if(user is null)
            {
                return false; // User not found
            }   


            if (!_passwordHasher.verify(CurrentPassword, user?.PasswordHash!))
            {
                return false;
            }

            user.PasswordHash = _passwordHasher.Hash(NewPassword!);
            await _userRepositery.UpdateAsync(user);
            return true;
        }

        public async Task<User?> GetUserTask(Guid id)
        {
            var user = await _userRepositery.GetByIdAsync(id);
            if (user is null) return null;

            var userWithTasks = await _userRepositery.GetUserTask(id);
            if (userWithTasks is null) return null;


            return userWithTasks;
        }
    }

}
