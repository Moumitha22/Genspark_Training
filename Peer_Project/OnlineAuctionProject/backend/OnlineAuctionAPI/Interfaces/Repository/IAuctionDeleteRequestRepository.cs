namespace OnlineAuctionAPI.Interfaces;

using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

public interface IAuctionDeleteRequestRepository : IRepository<Guid, AuctionDeleteRequest>
{
    Task AuctionDeleteRequestAsync(AuctionDeleteRequestDto dto);

    Task RejectAuctionDeleteRequestAsync(AuctionDeleteRequestRejectDto dto);

    Task<AuctionDeleteRequest> GetAuctionDeleteRequestByAuctionIdAsync(Guid id);
}
