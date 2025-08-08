using AutoMapper;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Mapping;

public class BidProfile : Profile
{
    public BidProfile()
    {
        CreateMap<BidItemAddDto, BidItem>()
            .ForMember(dest => dest.Bidder, opt => opt.Ignore())
            .ForMember(dest => dest.AuctionItem, opt => opt.Ignore());

        CreateMap<BidItem, BidItemAddDto>();

        CreateMap<BidItem, BidItemResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.BidderId))
            .ForMember(dest => dest.AuctionName, opt => opt.MapFrom(src => src.AuctionItem.Name));

        CreateMap<BidItemAddDto, BidItem>();
    }
}
