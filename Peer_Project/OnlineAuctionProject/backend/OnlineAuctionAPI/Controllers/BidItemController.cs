using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class BidItemController : ControllerBase
{
    private readonly IBidItemService _bidItemService;
    private readonly ILogger<BidItemController> _logger;

    public BidItemController(IBidItemService bidItemService, ILogger<BidItemController> logger)
    {
        _bidItemService = bidItemService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Bidder")]
    public async Task<ActionResult> AddBidItem([FromBody] BidItemAddDto bidDto)
    {
        var result = await _bidItemService.PlaceBidAsync(bidDto);
        _logger.LogInformation(
            "Bid placed successfully for auction item {AuctionItemId} by user {UserId}",
            bidDto.AuctionItemId,
            bidDto.BidderId
        );

        return Ok(ApiResponseHelper.CreateSuccess(result, "Bid placed successfully"));
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAllBids([FromQuery] BidFilterDto filterDto)
    {
        var result = await _bidItemService.GetAllBidAsync(filterDto);
        _logger.LogInformation(
            "Bids retrieved successfully"
        );
        return Ok(ApiResponseHelper.CreateSuccess(result, "Bids retrieved successfully"));
    }

    [HttpGet("{auctionId}")]
    [Authorize(Roles = "Seller,Bidder, Admin")]
    public async Task<ActionResult> GetBidsByAuctionId(Guid auctionId)
    {
        var result = await _bidItemService.GetBidsByAuctionIdAsync(auctionId, User);
        _logger.LogInformation(
            "Bids retrieved successfully for auction item {AuctionItemId}",
            auctionId
        );
        return Ok(ApiResponseHelper.CreateSuccess(result, "Bids retrieved successfully"));
    }

    [HttpGet("ByBidder")]
    [Authorize(Roles = "Bidder")]
    public async Task<ActionResult<IEnumerable<BidItemResponseDto>>> GetBidsByBidderId([FromQuery] BidFilterDto filterDto)
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid or missing userId in token");
                return BadRequest(
                    ApiResponseHelper.CreateBadRequest<string>("Please provide a valid userId.")
                );
            }

            var bidderItems = await _bidItemService.GetBidsByUserIdAsync(userId, filterDto);
            if (bidderItems == null)
            {
                _logger.LogWarning("No bidding data found");
                return NotFound(ApiResponseHelper.CreateNotFound<string>("No bidding Items found"));
            }
            return Ok(ApiResponseHelper.CreateSuccess(bidderItems, "Auctions for seller id"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bids for user");
            return StatusCode(
                500,
                ApiResponseHelper.CreateFailure<string>("An unexpected error occurred.")
            );
        }
    }

    [HttpGet("HighestBid/{auctionItemId}")]
    [Authorize(Roles = "Seller,Bidder")]
    public async Task<ActionResult> GetHighestBids(Guid auctionItemId)
    {
        var result = await _bidItemService.GetHighestBidAsync(auctionItemId);
        _logger.LogInformation(
            "Highest bid retrieved successfully for auction item {AuctionItemId}",
            auctionItemId
        );
        return Ok(ApiResponseHelper.CreateSuccess(result, "Highest bid retrieved successfully"));
    }

    [HttpDelete("{bidId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBid(Guid bidId)
    {
        var result = await _bidItemService.DeleteBidAsync(bidId);
        _logger.LogInformation("Bid with ID {BidId} deleted successfully", bidId);
        return Ok(ApiResponseHelper.CreateSuccess(result, "Bid deleted successfully"));
    }

    [HttpPatch("{bidId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateBidStatus(Guid bidId, [FromBody] BidStatusUpdateRequestDto dto)
    {
        var result = await _bidItemService.UpdateBidStatusAsync(bidId, dto.Status);
        _logger.LogInformation("Bid status with ID {BidId} updated successfully", bidId);
        return Ok(ApiResponseHelper.CreateSuccess(result, "Bid updated successfully"));
    }

    [HttpGet("ByBiddingId/{biddingId}")]
    [Authorize(Roles = "Seller,Bidder, Admin")]
    public async Task<ActionResult<IEnumerable<BidItemResponseDto>>> GetBidsByBiddingId(
        Guid biddingId
    )
    {
        try
        {
            if (biddingId == Guid.Empty)
            {
                _logger.LogWarning("Bidding ID cannot be empty");
                return BadRequest(
                    ApiResponseHelper.CreateBadRequest<string>("Bidding ID cannot be empty.")
                );
            }
            var bids = await _bidItemService.GetBidsByBiddingIdAsync(biddingId);
            if (bids == null)
            {
                _logger.LogWarning("No bids found for bidding ID {BiddingId}", biddingId);
                return NotFound(
                    ApiResponseHelper.CreateNotFound<string>(
                        $"No bids found for bidding ID {biddingId}"
                    )
                );
            }
            _logger.LogInformation(
                "Bids retrieved successfully for bidding ID {BiddingId}",
                biddingId
            );
            return Ok(ApiResponseHelper.CreateSuccess(bids, "Bids retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bids for bidding ID {BiddingId}", biddingId);
            return StatusCode(
                500,
                ApiResponseHelper.CreateFailure<string>("An unexpected error occurred.")
            );
        }
    }
}
