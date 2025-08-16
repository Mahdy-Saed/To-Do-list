using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using To_Do.Data.Modle.Dto.TaskDto;
using To_Do.Entity;
using To_Do.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _taskServices;
        private readonly IMapper _mapper;
        public TaskController(ITaskServices taskServices,IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _taskServices = taskServices ?? throw new ArgumentNullException(nameof(taskServices));
        }



        // GET: api/<TaskController>
        [HttpGet("Tasks")]
        public async Task<IActionResult>  GetAllTasks()
        {
            var userid = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            if(userid == Guid.Empty) 
            {
                return Unauthorized("User ID is not valid.");
            }
            var tasks = await _taskServices.GetTasksByUserAsync(userid);
            if ( !tasks.Any())
            {
                return NotFound("No tasks found for the user.");
            }
            var taskDtos=_mapper.Map<IEnumerable<TaskResponceDto>>(tasks);
            return Ok(taskDtos);
        }
        
        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userid = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var task = await _taskServices.GetTaskByIdAsync(id, userid);
            if(task is null)
            {
                return NotFound($"Task with ID {id} not found for user {userid}.");
            }
            var taskDto = _mapper.Map<TaskResponceDto>(task);
            return Ok(taskDto);
        }

        // POST api/<TaskController>
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody]CreateTaskRequestDto taskRequestDto )
        {
            if (taskRequestDto is null)
            {
                return BadRequest("request must not be null");
            }
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            if (! Guid.TryParse(userIdString,out Guid userid))
            {
                return Unauthorized("User ID is not valid.");
            }
            var task = await _taskServices.createTaskAsync(userid, _mapper.Map<ToDoTask>(taskRequestDto));
            if(task is null)
            {
                return BadRequest("Task creation failed.");
            }
            task.User_Id = userid; // Ensure the task is associated with the user
            if (task is null) return BadRequest("Task creation failed.");
            var taskDto = _mapper.Map<TaskResponceDto>(task);
            return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
        }

        // PUT api/<TaskController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TaskController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
