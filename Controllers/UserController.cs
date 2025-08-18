using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using To_Do.Data.Dto;
using To_Do.Data.Dto.TaskDto;
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
        private readonly IMapper _mapper;
        public UserController(IUserServices userServices, IMapper mapper)
        {
            _userServices = userServices;
            _mapper = mapper;
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

        // this will allow only users with the role of user to access this endpoint
        [ApiExplorerSettings(GroupName = "3-Users")]
        [Authorize(Roles = "User")] // this will allow only users with the role of user to access this endpoint
        [HttpGet("Users")]  //api/User/Users
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers() // can used with type that contain argument inside it like Task<T>
        {
            var users = await _userServices.GetAllUsers();

            var usersResponce = _mapper.Map<List<UserResponceDto>>(users);

            return Ok(usersResponce);
        }

        [ApiExplorerSettings(GroupName = "4-User")]
        [HttpGet("{id}")]  //api/User/Guid
        public async Task<ActionResult<UserResponceDto>> GetUser(Guid id) // can used with type that contain argument inside it like Task<T>
        {
            var user = await _userServices.GetUserById(id);
            if (user is null) return NotFound("User not found");

            var userDto = _mapper.Map<UserResponceDto>(user); // map the user to UserResponceDto
                                                              //must add mapper
            return Ok(userDto);
        }



        [HttpGet("{id}/User-Task")]
        public async Task<IActionResult> GetUserTask(Guid id)
        {
            var user = await _userServices.GetUserTask(id);
            if (user is null) return NotFound("User not found");
            var userDto = _mapper.Map<UserResponceDto>(user);
            var userTask = _mapper.Map<List<TaskResponceDto>>(user.Tasks);

            var response = new UserTasksResponseDto() // we can do it in mapper by specify the member for it
            {
                User = userDto,
                Tasks = userTask
            }; // map the user to UserResponceDto
               //must add mapper
            return Ok(response);
        }



        [ApiExplorerSettings(GroupName = "5-Update")]
        [HttpPut("{id}")] //api/User/Guid
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateRequest UpdateRequest)
        {
            if (UpdateRequest == null)
            {
                return BadRequest("User cannot be null");
            }
            var userExists = await _userServices.GetUserById(id);
            if (userExists == null)
            {
                return NotFound("User not found");
            }
            _mapper.Map(UpdateRequest, userExists);
            var updatedUser = await _userServices.UpdateUser(id, userExists);
            if (updatedUser == null)
            {
                return NotFound("User not found");
            }
            var responce = _mapper.Map<UserResponceDto>(updatedUser);

            return Ok(responce);
        }

        [ApiExplorerSettings(GroupName = "6-Patch")]
        [HttpPatch("{id}")] //api/User/Guid
        public async Task<IActionResult> UpdateSpcefic(Guid id, JsonPatchDocument<UpdateRequest> patchDocument)
        {
            var userEntity = await _userServices.GetUserById(id);

            if (userEntity is null) return NotFound("User not found");

            if (patchDocument is null)
            {
                return BadRequest("Request cannot be null");
            }
            var userDto = _mapper.Map<UpdateRequest>(userEntity); // map the user to UpdateRequest
            patchDocument.ApplyTo(userDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(userDto, userEntity);

            var Updated = await _userServices.UpdateUser(id, userEntity);

            var result = _mapper.Map<UserResponceDto>(Updated);

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
            return NoContent(); // 204 No Content mean every thing is ok and the user is deleted.
        }

        [ApiExplorerSettings(GroupName = "8-Delete")]
        [HttpDelete("AllUsers")] //api/User/Guid
        public async Task<IActionResult> DeleteAllUsers()
        {
            var users = await _userServices.GetAllUsers();
            if (users is null || !users.Any())
            {
                return NotFound("No users found to delete");
            }
            await _userServices.DeleteAllUser();
            return NoContent(); // 204 No Content mean every thing is ok and all users are deleted.
        }


        [ApiExplorerSettings(GroupName = "9-changePassword")]
        [Authorize(Roles = "User")]
        [HttpPatch("ChangePassword")] //api/User/ChangePassword
        public async Task<IActionResult> ChangePassWord([FromBody] ChangePasswordDto passwordDto)
        {
            if (passwordDto == null)
                return BadRequest("Change password request cannot be null");

            var userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userid, out Guid userId))
                return Unauthorized("Invalid user ID in token");

            var changed = await _userServices.ChangePasswordAsync(userId, passwordDto.oldPassword!, passwordDto.newPassword!);

            if (!changed)
                return BadRequest("Old password is incorrect or user not found");

            return Ok("Password changed successfully");
        }
    }
}
