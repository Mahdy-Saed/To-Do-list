using Microsoft.EntityFrameworkCore;
using To_Do.Entity;

namespace To_Do.Data
{
    public class ToDoContext: DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ToDoTask> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.User_Id);
        }
    }
     
}
