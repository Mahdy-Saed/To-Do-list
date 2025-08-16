using AutoMapper;
using To_Do.Data.Modle.Dto.TaskDto;
using To_Do.Entity;

namespace To_Do.Mapper
{
    public class ToDoTaskProfile : Profile
    {
        public ToDoTaskProfile() {

            CreateMap<ToDoTask, ToDoTask>();


            CreateMap<CreateTaskRequestDto, ToDoTask>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ReminderTime, opt => opt.Condition(src => src.ReminderEnabled));

            CreateMap<UpdateTaskRequestDto, ToDoTask>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseStatus(src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)))
            .ForMember(dest => dest.ReminderTime, opt => opt.Condition(src => src.ReminderEnabled));

            CreateMap<ToDoTask, TaskResponceDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.ToString() : null))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.HasValue ? src.Priority.ToString() : null))
            .ForMember(dest => dest.ReminderTime, opt => opt.Condition(src => src.ReminderEnabled));
        }

    

    private static ToDoTask.TaskStatus ParseStatus(string? status)
        {
            if (Enum.TryParse<ToDoTask.TaskStatus>(status, true, out var result))
                return result;
            return ToDoTask.TaskStatus.Pending; // قيمة افتراضية
        }

        private static ToDoTask.TaskPriority ParsePriority(string? priority)
        {
            if (Enum.TryParse<ToDoTask.TaskPriority>(priority, true, out var result))
                return result;
            return ToDoTask.TaskPriority.Low; // قيمة افتراضية
        }



    }
}
