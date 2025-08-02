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

        [HttpPost("Register")] //api/User/Register
        [ProducesResponseType(201)]
        public async Task<IActionResult> Register([FromBody]RequestDto requestDto)
        {
            if(requestDto == null)
            {
                return BadRequest("Request cannot be null");
            }
            var tokenResponce = await _userServices.CreateUser(requestDto);

            if(tokenResponce is null)
            {
                return BadRequest("Create User Faild");
            }
            return  CreatedAtAction(nameof(GetUser), new { id =tokenResponce.User_Id}, tokenResponce); // this will return the created user with the token


        }


        



        [HttpGet("{id}")]  //api/User/Guid
      public  async Task<ActionResult<UserDto>> GetUser(Guid id) // can used with type that contain argument inside it like Task<T>
        {
           var user = await _userServices.GetUserById(id);
            if (user is null) return BadRequest("User not found");

            //must add mapper
        return Ok(user);
        }

        [HttpGet("Users")]  //api/User/Guid
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers() // can used with type that contain argument inside it like Task<T>
        {
            var users =  await  _userServices.GetAllUsers();

            List<UserDto> userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role,
                // Add other properties as needed
            }).ToList();
            return Ok(userDtos);
        }



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
                UserName = UserDto.UserName,
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


        [HttpPatch("{id}")] //api/User/Guid

        public async Task<IActionResult> UpdateSpcefic(Guid id,JsonPatchDocument<RequestDto > patchDocument)
        {
            var user = await _userServices.GetUserById(id);

            if (user is null) return NotFound("User not found");

            if (patchDocument is null)
            {
                return BadRequest("Request cannot be null");
            }
            RequestDto usetToDto = new RequestDto
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






    }
}
