using AutoMapper;
using To_Do.Data.Dto;
using To_Do.Entity;

namespace To_Do.Mapper
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
             CreateMap<User,  UserResponceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            //CreateMap<User,UserTasksResponseDto>()
            //    .ForMember(dest=>dest.User,opt=>opt.MapFrom(src=>src))
            //    .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));

            CreateMap<UpdateRequest, User>();

            CreateMap<User, UpdateRequest>();

        }



    }
}
