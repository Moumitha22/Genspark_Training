using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuctionDeleteRequestController : ControllerBase
{
    private readonly IAuctionDeleteRequestService _deleteRequestService;
    private readonly ILogger<AuctionDeleteRequestController> _logger;

    public AuctionDeleteRequestController(
        IAuctionDeleteRequestService deleteRequestService,
        ILogger<AuctionDeleteRequestController> logger
    )
    {
        _deleteRequestService = deleteRequestService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<ActionResult> SubmitAuctionDeleteRequest(
        [FromBody] AuctionDeleteRequestDto dto
    )
    {
        await _deleteRequestService.SubmitAuctionDeleteRequestAsync(dto);
        _logger.LogInformation(
            "Auction delete request submitted for auction {AuctionId}",
            dto.AuctionItemId
        );
        return Ok(
            ApiResponseHelper.CreateSuccess<string>(
                null,
                "Auction delete request submitted successfully"
            )
        );
    }

    [HttpPatch("{auctionId}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ApproveAuctionDeleteRequest(Guid auctionId)
    {
        await _deleteRequestService.ApproveAuctionDeleteRequestAsync(auctionId);
        _logger.LogInformation(
            "Auction delete request approved for auction {AuctionId}",
            auctionId
        );
        return Ok(
            ApiResponseHelper.CreateSuccess<string>(
                null,
                "Auction delete request approved successfully"
            )
        );
    }

    [HttpPatch("reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RejectAuctionDeleteRequest(
        [FromBody] AuctionDeleteRequestRejectDto dto
    )
    {
        await _deleteRequestService.RejectAuctionDeleteRequestAsync(dto);
        _logger.LogInformation(
            "Auction delete request rejected for auction {AuctionId}",
            dto.AuctionItemId
        );
        return Ok(
            ApiResponseHelper.CreateSuccess<string>(
                null,
                "Auction delete request rejected successfully"
            )
        );
    }

    [HttpGet("{auctionId}")]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<ActionResult<AuctionDeleteRequest>> GetAuctionDeleteRequestByAuctionId(
        Guid auctionId
    )
    {
        var result = await _deleteRequestService.GetAuctionDeleteRequestByAuctionIdAsync(auctionId);
        if (result == null)
        {
            _logger.LogWarning("No delete request found for auction {AuctionId}", auctionId);
            return NotFound(
                ApiResponseHelper.CreateNotFound<string>(
                    "No delete request found for this auction."
                )
            );
        }
        _logger.LogInformation("Delete request retrieved for auction {AuctionId}", auctionId);
        return Ok(ApiResponseHelper.CreateSuccess(result, "Delete request retrieved successfully"));
    }
}
