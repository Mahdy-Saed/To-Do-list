using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using To_Do.Data.Dto.TaskDto;
using To_Do.Entity;
using To_Do.Services;

namespace To_Do.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _taskServices;
        private readonly IMapper _mapper;

        public TaskController(ITaskServices taskServices, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _taskServices = taskServices ?? throw new ArgumentNullException(nameof(taskServices));
        }

        // private function to check and extrace userID
        private bool TryGetValidUserId(out Guid userId, out IActionResult errorResult)
        {
            if (!this.TryGetUserId(out userId))
            {
                errorResult = Unauthorized("Authorization-Error: User ID is not valid.");
                return false;
            }
            errorResult = null;
            return true;
        }

        [HttpGet("Tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var tasks = await _taskServices.GetTasksByUserAsync(userid);
            if (!tasks.Any())
            {
                return NotFound("No tasks found for the user.");
            }
            var taskDtos = _mapper.Map<IEnumerable<TaskResponceDto>>(tasks);
            return Ok(taskDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var task = await _taskServices.GetTaskByIdAsync(id, userid);
            if (task is null)
            {
                return NotFound($"Task with ID {id} not found for user {userid}.");
            }
            var taskDto = _mapper.Map<TaskResponceDto>(task);
            return Ok(taskDto);
        }

        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDto taskRequestDto)
        {
            if (taskRequestDto is null)
            {
                return BadRequest("Request must not be null.");
            }

            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var task = await _taskServices.createTaskAsync(userid, _mapper.Map<ToDoTask>(taskRequestDto));
            if (task is null)
            {
                return BadRequest("Task creation failed.");
            }

            task.User_Id = userid; // تأكد ربط المهمة بالمستخدم
            var taskDto = _mapper.Map<TaskResponceDto>(task);
            return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequestDto value)
        {
            if (value is null)
            {
                return BadRequest("Request body cannot be null.");
            }

            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var existingTask = await _taskServices.GetTaskByIdAsync(id, userid);
            if (existingTask is null)
            {
                return NotFound($"Task with ID {id} not found for user {userid}.");
            }

            _mapper.Map(value, existingTask);
            var isUpdated = await _taskServices.UpdateTaskAsync(existingTask, userid);
            if (!isUpdated)
            {
                return BadRequest("Task update failed.");
            }

            var updatedTaskDto = _mapper.Map<TaskResponceDto>(existingTask);
            return Ok(updatedTaskDto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartTask(int id, [FromBody] JsonPatchDocument<UpdateTaskRequestDto> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch document cannot be null.");
            }

            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var existingTask = await _taskServices.GetTaskByIdAsync(id, userid);
            if (existingTask is null)
            {
                return NotFound($"Task with ID {id} not found for user {userid}.");
            }

            var taskToPatch = _mapper.Map<UpdateTaskRequestDto>(existingTask);
            patchDoc.ApplyTo(taskToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(taskToPatch, existingTask);
            var isUpdated = await _taskServices.UpdateTaskAsync(existingTask, userid);
            if (!isUpdated)
            {
                return BadRequest("Task update failed.");
            }

            var updatedTaskDto = _mapper.Map<TaskResponceDto>(existingTask);
            return Ok(updatedTaskDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            var task = await _taskServices.GetTaskByIdAsync(id, userid);
            if (task is null)
            {
                return NotFound($"Task with ID {id} not found for user {userid}.");
            }

            var isDeleted = await _taskServices.DeleteTaskAsync(id, userid);
            if (!isDeleted)
            {
                return BadRequest("Task deletion failed.");
            }
            return NoContent();
        }

        [HttpDelete("DeleteAllTasks")]
        public async Task<IActionResult> DeleteAllTasks()
        {
            if (!TryGetValidUserId(out var userid, out var error))
                return error;

            try
            {
                await _taskServices.DeleteAllTasksAsync(userid);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
