using To_Do.Data.Dto.TaskDto;

namespace To_Do.Data.Dto
{
    public class UserTasksResponseDto
    {
        public UserResponceDto? User { get; set; } 

       public  List<TaskResponceDto>? Tasks { get; set; }  


    }
}
