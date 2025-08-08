using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IAuctionDeleteRequestService
{
    Task SubmitAuctionDeleteRequestAsync(AuctionDeleteRequestDto dto);
    Task RejectAuctionDeleteRequestAsync(AuctionDeleteRequestRejectDto dto);
    Task<AuctionDeleteRequest> GetAuctionDeleteRequestByAuctionIdAsync(Guid id);
    Task ApproveAuctionDeleteRequestAsync(Guid auctionId);
}
