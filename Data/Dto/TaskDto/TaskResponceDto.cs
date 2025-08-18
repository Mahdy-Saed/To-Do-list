namespace To_Do.Data.Dto.TaskDto
{
    public class TaskResponceDto
    {
    
            public int Id { get; set; }
            public Guid UserId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }

            public string? Status { get; set; } // we convert it to string in the controller and mapper
            public string? Priority { get; set; }

            public DateTime CreateAt { get; set; }
            public DateTime? DueDate { get; set; }

            public bool ReminderEnabled { get; set; }
            public DateTime? ReminderTime { get; set; }
       



    }
}
