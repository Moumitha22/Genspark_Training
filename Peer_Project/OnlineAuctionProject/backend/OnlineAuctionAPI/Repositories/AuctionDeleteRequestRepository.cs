using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Repositories;

public class AuctionDeleteRequestRepository
    : Repository<Guid, AuctionDeleteRequest>,
        IAuctionDeleteRequestRepository
{
    public AuctionDeleteRequestRepository(AuctionContext context)
        : base(context) { }

    public async Task AuctionDeleteRequestAsync(AuctionDeleteRequestDto dto)
    {
        try
        {
            if (dto == null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Auction delete request cannot be null."
                );
            }

            var auctionItem = await _auctionContext.AuctionItems.FirstOrDefaultAsync(a =>
                a.Id == dto.AuctionItemId
            );
            if (auctionItem == null)
            {
                throw new NotFoundException($"Auction item with ID {dto.AuctionItemId} not found.");
            }

            var deleteRequest = new AuctionDeleteRequest
            {
                Id = Guid.NewGuid(),
                AuctionItemId = dto.AuctionItemId,
                RequestDate = DateTime.UtcNow,
                Reason = dto.Reason,
                UserId = dto.UserId,
                IsApproved = false,
                ApprovalDate = null,
            };

            _auctionContext.AuctionDeleteRequests.Add(deleteRequest);
            await _auctionContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error creating auction delete request.", ex);
        }
    }

    public async Task RejectAuctionDeleteRequestAsync(AuctionDeleteRequestRejectDto requestId)
    {
        try
        {
            if (requestId == null)
            {
                throw new ArgumentNullException(nameof(requestId), "Request cannot be null.");
            }

            var deleteRequest = await _auctionContext.AuctionDeleteRequests.FirstOrDefaultAsync(r =>
                r.AuctionItemId == requestId.AuctionItemId
            );

            if (deleteRequest == null)
            {
                throw new NotFoundException(
                    $"Delete request for auction item {requestId.AuctionItemId} not found."
                );
            }

            deleteRequest.IsApproved = false;
            deleteRequest.Remarks = requestId.Remarks;
            deleteRequest.ApprovalDate = DateTime.UtcNow;

            _auctionContext.AuctionDeleteRequests.Update(deleteRequest);
            await _auctionContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error rejecting auction delete request.", ex);
        }
    }

    public async Task<AuctionDeleteRequest> GetAuctionDeleteRequestByAuctionIdAsync(Guid auctionId)
    {
        try
        {
            if (auctionId == Guid.Empty)
            {
                throw new ArgumentException("Auction ID cannot be empty.", nameof(auctionId));
            }

            var deleteRequest = await _auctionContext.AuctionDeleteRequests.FirstOrDefaultAsync(r =>
                r.AuctionItemId == auctionId
            );

            if (deleteRequest == null)
            {
                throw new NotFoundException(
                    $"Delete request for auction item {auctionId} not found."
                );
            }

            return deleteRequest;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException(
                "Error retrieving auction delete request by auction ID.",
                ex
            );
        }
    }
}
