using AutoMapper;
using To_Do.Data.Repositery;
using To_Do.Entity;

namespace To_Do.Services
{
    public interface ITaskServices
    {
        Task<ToDoTask?> createTaskAsync(Guid userId, ToDoTask task);
        Task<ToDoTask?> GetTaskByIdAsync(int id, Guid userId);
        Task<IEnumerable<ToDoTask>> GetTasksByUserAsync(Guid userId);
        Task<bool> UpdateTaskAsync(ToDoTask task, Guid userId);
        Task<bool> DeleteTaskAsync(int id, Guid userId);
        Task<bool> DeleteAllTasksAsync(Guid userId);
    }

    public class TaskServices : ITaskServices
    {
        private readonly ITaskRepositer  _taskRepositer;
         public TaskServices(ITaskRepositer taskRepositer,IMapper mapper)
        {
            _taskRepositer =taskRepositer ?? throw new ArgumentNullException(nameof(taskRepositer));
         }
        public async Task<ToDoTask?> createTaskAsync(Guid userId, ToDoTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            task.User_Id = userId;
            task.CreateAt = DateTime.UtcNow; // set the creation time to now
            await _taskRepositer.AddAsync(task);
            return task;
        }
        public async Task<ToDoTask?> GetTaskByIdAsync(int id, Guid userId)
        {
            return await _taskRepositer.GetByIdAsync(id, userId);
        }
        public async Task<IEnumerable<ToDoTask>> GetTasksByUserAsync(Guid userId)
        {
            return await _taskRepositer.GetAllAsync(userId);
        }
        public async Task<bool> UpdateTaskAsync(ToDoTask task, Guid userId)
        {

            if (task == null)
                throw new ArgumentNullException(nameof(task));

            try
            {
                await _taskRepositer.UpdateAsync(task, userId);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false; // أو ترجع exception حسب أسلوبك
            }
        }
        public async Task<bool> DeleteTaskAsync(int id, Guid userId)
        {
            try
            {
                await _taskRepositer.DeleteAsync(id, userId);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false; // أو ترجع exception حسب أسلوبك
            }
        }

        public async Task<bool> DeleteAllTasksAsync(Guid userId)
        {
            try
            {
                await _taskRepositer.DeleteAllAsync(userId);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false; // أو ترجع exception حسب أسلوبك
            }
        }
    }

}
