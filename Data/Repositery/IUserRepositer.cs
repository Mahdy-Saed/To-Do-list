using Microsoft.EntityFrameworkCore;
using To_Do.Data;
using To_Do.Data.Repositery;
using To_Do.Entity;

namespace To_Do.Data.Repositery
{
    public interface IUserRepositery
    {

        // add , get, update, delete              //task mean return nothing but if write it like this Task<T> mean return this type of T
        Task AddAsync(User user);

        Task<User?> GetByEmail(string email);
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);


        Task UpdateAsync( User user);   

        Task DeleteAsync(Guid id);

    }

    public class UserRepositery : IUserRepositery
    {

        private readonly ToDoContext _context;

        public UserRepositery(ToDoContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await  _context.Users.ToListAsync();     
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);    //find used only primary key and its faster cuz search in memo
                                                        /// first or firstOrDefault use condition
        } 
        public async Task UpdateAsync(User user)
        {
             _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
         public async Task DeleteAsync(Guid id)
        {
           var user = await GetByIdAsync(id);
             if(user is not null)
            {
                  _context.Users.Remove(user);
                await _context.SaveChangesAsync();    // anything with async mean its Task like this SaveChangesAsync should put await before it
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            //return only one record that have this Email
           return await _context.Users.SingleOrDefaultAsync(u=>u.Email == email);  
        }
    }

}


 
 
 
 
 