using AutoMapper;
using To_Do.Data.Modle.Dto;
using To_Do.Entity;

namespace To_Do.Mapper
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponceDto>();

            CreateMap<LoginRequestDto, User>();

            CreateMap<RequestDto, User>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

            CreateMap<UserDto, User>();

            CreateMap<User, UserResponceDto>();
                 


        }



    }
}
