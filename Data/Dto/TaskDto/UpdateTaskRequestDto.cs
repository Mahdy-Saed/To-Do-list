using System.ComponentModel.DataAnnotations;

namespace To_Do.Data.Dto.TaskDto
{
    public class UpdateTaskRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public bool ReminderEnabled { get; set; }

        public DateTime? ReminderTime { get; set; }

    }
}
