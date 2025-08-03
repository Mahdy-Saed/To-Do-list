using To_Do.Data.Modle.Dto.TaskDto;

namespace To_Do.Data.Modle.Dto
{
    public class UserWithTasksDto
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }

        public string? Email { get; set; }

         List<TaskResponceDto> Tasks { get; set; }


    }
}
