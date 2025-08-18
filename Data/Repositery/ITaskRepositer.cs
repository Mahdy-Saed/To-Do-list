using AutoMapper;
using Microsoft.EntityFrameworkCore;
using To_Do.Entity;

namespace To_Do.Data.Repositery
{
    public interface ITaskRepositer
    {
        // add , get, update, delete              //task mean return nothing but if write it like this Task<T> mean return this type of T
        Task AddAsync(ToDoTask Task);

         Task<IEnumerable<ToDoTask>> GetAllAsync(Guid userId);

        Task<ToDoTask?> GetByIdAsync(int id, Guid userId);

        Task UpdateAsync(ToDoTask Task,Guid userId);

        Task DeleteAsync(int id, Guid userId);

        Task DeleteAllAsync(Guid userId);
    }

    public class TaskRepositer : ITaskRepositer
    {
        private readonly ToDoContext _context;
        private readonly IMapper _mapper;   
        public TaskRepositer(ToDoContext context,IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task AddAsync(ToDoTask Task)
        {
            await _context.Tasks.AddAsync(Task);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ToDoTask>> GetAllAsync(Guid userId)
        {
            return await _context.Tasks.Where(u=>u.User_Id== userId).ToListAsync();
        }
        public async Task<ToDoTask?> GetByIdAsync(int id,Guid userId)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t=>t.Id==id && t.User_Id==userId);    //first or Default used to return only one record 

        }
        public async Task UpdateAsync(ToDoTask Task, Guid userId)
        {
            var taskExsit = await GetByIdAsync(Task.Id, userId);
            if (taskExsit is null)
            {
                throw new KeyNotFoundException($"Task with ID {Task.Id} not found for user {userId}.");
            }
            _mapper.Map(Task,taskExsit); // map the existing task to the new task
            _context.Tasks.Update(taskExsit);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id, Guid userId)
        {
            var taskExsit =   await GetByIdAsync(id,userId);
            if ( taskExsit is  null)  throw new KeyNotFoundException($"Task with ID {id} not found for user {userId}.");

            _context.Tasks.Remove(taskExsit);
                await _context.SaveChangesAsync();
            
        }
        public async Task DeleteAllAsync(Guid userId)
        {
            var tasks = await _context.Tasks.Where(t => t.User_Id == userId).ToListAsync();
            if(tasks == null || !tasks.Any())
            {
                throw new KeyNotFoundException($"No tasks found for user {userId}.");
            }
            _context.Tasks.RemoveRange(tasks);
            await _context.SaveChangesAsync();

        }
    }
}
