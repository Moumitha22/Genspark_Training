using AutoMapper;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegisterRequestDto, User>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role, true))
            )
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.Auctions, opt => opt.Ignore())
            .ForMember(dest => dest.Auctions, opt => opt.Ignore())
            .ForMember(dest => dest.Bids, opt => opt.Ignore());

        CreateMap<User, UserRegisterRequestDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<UserUpdateRequestDto, User>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role, true))
            )
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.StatusId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Auctions, opt => opt.Ignore())
            .ForMember(dest => dest.Bids, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
