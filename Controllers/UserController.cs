using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.Threading.Tasks;
using To_Do.Data.Modle.Dto;
using To_Do.Entity;
using To_Do.Services;

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // create,read,update,delete users
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }


        [ApiExplorerSettings(GroupName = "1-Register")]
        [HttpPost("Register")] //api/User/Register
        [ProducesResponseType(201)]
        public async Task<IActionResult> Register([FromBody] RequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }
            var tokenResponce = await _userServices.CreateUser(request);

            if (tokenResponce is null)
            {
                return BadRequest("Create User Faild,Password or Email Invalid");
            }
            return CreatedAtAction(nameof(GetUser), new { id = tokenResponce.User_Id }, tokenResponce); // this will return the created user with the token


        }

      [ApiExplorerSettings(GroupName = "2-Login")]
        [HttpPost("login")] //api/User/login
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Login request cannot be null");
            }
            var tokenResponce = await _userServices.Login(loginDto);
            if (tokenResponce is null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(tokenResponce);
        }
        

        [ApiExplorerSettings(GroupName = "3-Users")]
        [HttpGet("Users")]  //api/User/Guid
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers() // can used with type that contain argument inside it like Task<T>
        {
            var users =  await  _userServices.GetAllUsers();

            List<UserResponceDto> userDtos = users.Select(u => new UserResponceDto()
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Role = u.Role,
                // Add other properties as needed
            }).ToList();
            return Ok(userDtos);
        }

        [ApiExplorerSettings(GroupName = "4-User")]
        [HttpGet("{id}")]  //api/User/Guid
      public  async Task<ActionResult<UserResponceDto>> GetUser(Guid id) // can used with type that contain argument inside it like Task<T>
        {
           var user = await _userServices.GetUserById(id);
            if (user is null) return NotFound("User not found");

            //must add mapper
        return Ok(user);
        }



        [ApiExplorerSettings(GroupName = "5-Update")]
        [HttpPut("{id}")] //api/User/Guid
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto UserDto)
        {
            if (UserDto == null)
            {
                return BadRequest("User cannot be null");
            }
            var user = new User()
            {
                Id = id,
                UserName =UserDto.UserName,
                Email = UserDto.Email,
                Role = UserDto.Role
                // Add other properties as needed
            };
            var updatedUser = await _userServices.UpdateUser(id, user);
            if (updatedUser == null)
            {
                return NotFound("User not found");
            }
            return Ok(updatedUser);
        }

        [ApiExplorerSettings(GroupName = "6-Patch")]
        [HttpPatch("{id}")] //api/User/Guid
        public async Task<IActionResult> UpdateSpcefic(Guid id,JsonPatchDocument<RequestDto> patchDocument)
        {
            var user = await _userServices.GetUserById(id);

            if (user is null) return NotFound("User not found");

            if (patchDocument is null)
            {
                return BadRequest("Request cannot be null");
            }
            RequestDto usetToDto = new RequestDto()
            {
                UserName = user.UserName,
                Email = user.Email
            };
            patchDocument.ApplyTo(usetToDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userServices.UpdateUser(id, new User
            {
                Id = id,
                UserName = usetToDto.UserName,
                Email = usetToDto.Email
            });
            return Ok(result);

        }

        [ApiExplorerSettings(GroupName = "7-Delete")]
        [HttpDelete("{id}")] //api/User/Guid
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var isDeleted = await _userServices.DeleteUser(id);
            if (!isDeleted)
            {
                return NotFound("User not found");
            }
            return NoContent(); // 204 No Content
        }




    }
}
