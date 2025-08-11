using System.ComponentModel.DataAnnotations;

namespace To_Do.Entity
{
    public class ToDoTask
    {
        public int  Id { get; set; }

        public Guid User_Id { get; set; }
        public User? User { get; set; }


        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public TaskStatus? Status { get; set; }

        public TaskPriority?  Priority { get; set; }


        public  DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get;  set; }

        public bool ReminderEnabled { get; set; } = false;

        public DateTime ReminderTime { get; set; }


        public enum TaskPriority
        {
            Low,
            Medium,
            High
        }
        public enum TaskStatus
        {
            Pending,
            InProgress,
            Completed,
            Cancelled
        }
    }
}
