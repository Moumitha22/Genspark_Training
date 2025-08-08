using AutoMapper;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Mapping
{
    public class AuctionItemProfile : Profile
    {
        public AuctionItemProfile()
        {
            CreateMap<AuctionItemAddDto, AuctionItem>()
                .ForMember(dest => dest.Seller, opt => opt.Ignore())
                .ForMember(dest => dest.WinningBid, opt => opt.Ignore())
                .ForMember(dest => dest.Bids, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerId, opt => opt.Ignore())
                .ForMember(dest => dest.FileAttachments, opt => opt.Ignore());

            CreateMap<AuctionItem, AuctionItemAddDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.FileAttachments, opt => opt.Ignore());

            CreateMap<AuctionItem, AuctionItemResponseDto>()
                .ForMember(
                    dest => dest.SellerName,
                    opt =>
                        opt.MapFrom(src => src.Seller != null ? src.Seller.Username : string.Empty)
                )
                .ForMember(
                    dest => dest.Files,
                    opt =>
                        opt.MapFrom(
                            (src, dest, destMember, context) =>
                            {
                                if (src.FileAttachments == null)
                                    return new List<FileDataDto>();

                                var httpContext =
                                    context.Items != null
                                    && context.Items.TryGetValue("HttpContext", out var httpCtxObj)
                                        ? httpCtxObj as HttpContext
                                        : null;

                                return src
                                    .FileAttachments.Select(file =>
                                    {
                                        var baseUrl =
                                            httpContext != null
                                                ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}"
                                                : string.Empty;

                                        return new FileDataDto
                                        {
                                            Name = file.Name,
                                            ContentType = file.ContentType,
                                            DownloadUrl = string.IsNullOrEmpty(baseUrl)
                                                ? string.Empty
                                                : $"{baseUrl}/api/v1/AuctionItem/download/{file.AuctionItemId}/{Uri.EscapeDataString(file.Name)}",
                                        };
                                    })
                                    .ToList();
                            }
                        )
                )
                .ForMember(
                    dest => dest.DeleteRequest,
                    opt => opt.MapFrom(src => src.DeleteRequest)
                );

            CreateMap<FileData, FileDataDto>()
                .ForMember(dest => dest.DownloadUrl, opt => opt.Ignore());
        }
    }
}
