using To_Do.Data.Modle.Dto.TaskDto;

namespace To_Do.Data.Modle.Dto
{
    public class UserTasksResponseDto
    {
        public UserResponceDto? User { get; set; } 

       public  List<TaskResponceDto>? Tasks { get; set; }  


    }
}
