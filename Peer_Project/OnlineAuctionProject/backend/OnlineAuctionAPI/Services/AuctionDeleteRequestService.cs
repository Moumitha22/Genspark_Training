using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Services;

public class AuctionDeleteRequestService : IAuctionDeleteRequestService
{
    private readonly IAuctionDeleteRequestRepository _deleteRequestRepository;

    public AuctionDeleteRequestService(IAuctionDeleteRequestRepository deleteRequestRepository)
    {
        _deleteRequestRepository = deleteRequestRepository;
    }

    public async Task SubmitAuctionDeleteRequestAsync(AuctionDeleteRequestDto dto)
    {
        await _deleteRequestRepository.AuctionDeleteRequestAsync(dto);
    }

    public async Task RejectAuctionDeleteRequestAsync(AuctionDeleteRequestRejectDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Auction delete request cannot be null.");
        }
        await _deleteRequestRepository.RejectAuctionDeleteRequestAsync(dto);
    }

    public async Task<AuctionDeleteRequest> GetAuctionDeleteRequestByAuctionIdAsync(Guid id)
    {
        return await _deleteRequestRepository.GetAuctionDeleteRequestByAuctionIdAsync(id);
    }

    public async Task ApproveAuctionDeleteRequestAsync(Guid auctionId)
    {
        if (auctionId == Guid.Empty)
            throw new ArgumentException("Auction ID cannot be empty.", nameof(auctionId));

        var deleteRequest = await _deleteRequestRepository.GetAuctionDeleteRequestByAuctionIdAsync(
            auctionId
        );
        if (deleteRequest == null)
            throw new NotFoundException($"Delete request for auction ID {auctionId} not found");

        deleteRequest.IsApproved = true;
        deleteRequest.ApprovalDate = DateTime.UtcNow;

        await _deleteRequestRepository.Update(deleteRequest.Id, deleteRequest);
    }
}
