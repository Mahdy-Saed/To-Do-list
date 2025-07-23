using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Generators;
using To_Do.Authentication;
using To_Do.Authntication;
using To_Do.Data.Modle.Dto;
using To_Do.Data.Repositery;
using To_Do.Entity;

namespace To_Do.Services
{
    public interface IUserServices
    {
        Task<TokenResponce?> CreateUser(RequestDto requestDto);

        Task<User?> GetUserById(Guid id);


        Task<IEnumerable<User>> GetAllUsers(User user);
        Task<User?> UpdateUser(Guid id, User user);


        Task<bool> DeleteUser(Guid id);


    }
    public class UserServices : IUserServices
    {
        private readonly IUserRepositery _userRepositery;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerater _tokenGenerater;

        public UserServices(IUserRepositery userRepositery)
        {
            _userRepositery = userRepositery;
        }
        public async Task<TokenResponce?> CreateUser(RequestDto  requestDto)
        {
            if (requestDto == null) throw  new  ArgumentNullException(nameof(requestDto));

            if (string.IsNullOrEmpty(requestDto.Password)){            
            throw new ArgumentNullException(requestDto.Password,"Password can not be null or empty");
            }
            var passwordHash = _passwordHasher.Hash(requestDto.Password);
            var user = new User()
            {
                UserName = requestDto.UserName,
                Email = requestDto.Email,
                PasswordHash = passwordHash
            };
            TokenResponce tokenResponse = new TokenResponce()
            {
                // just for test 
                Access_Token = _tokenGenerater.CreateAccessToken(user), //To Do: replace with actual token generation logic
                Refresh_Token = _tokenGenerater.CreateRefreshToken(),
            };
            user.RefreshToken = tokenResponse.Refresh_Token;
            await _userRepositery.AddAsync(user);  // this will save the user in the database
            return tokenResponse;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = _userRepositery.GetByIdAsync(id);

            if (user == null) return false;

            await _userRepositery.DeleteAsync(id);  //inside the user repositery will save changes

            return true;  
        }

        public async Task<IEnumerable<User>> GetAllUsers(User user)
        { 

            return await _userRepositery.GetAllAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            var user = _userRepositery.GetByIdAsync(id);
            
                if(user is null) return  null;
                
                return await  user;

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
    }

}
