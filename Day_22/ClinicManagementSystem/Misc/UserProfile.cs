using AutoMapper;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Misc
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DoctorAddRequestDto, User>() // to return User(src)
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<User, DoctorAddRequestDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));
   
            CreateMap<PatientAddRequestDto, User>() // to return User(src)
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());
           
            CreateMap<User, PatientAddRequestDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));
        }
    }
}