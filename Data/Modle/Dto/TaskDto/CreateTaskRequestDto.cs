using System.ComponentModel.DataAnnotations;

namespace To_Do.Data.Modle.Dto.TaskDto
{
      public class CreateTaskRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? Status { get; set; }  // string بدل enum

        public string? Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public bool ReminderEnabled { get; set; }

        public DateTime? ReminderTime { get; set; }
    }
}
