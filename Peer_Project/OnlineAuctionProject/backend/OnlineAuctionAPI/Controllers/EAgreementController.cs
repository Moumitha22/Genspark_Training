using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EAgreementController : ControllerBase
    {
        private readonly IEAgreementService _eAgreementService;
        private readonly IEmailService _emailService;
        private readonly ILogger<EAgreementController> _logger;

        public EAgreementController(
            IEAgreementService eAgreementService,
            IEmailService emailService,
            ILogger<EAgreementController> logger
        )
        {
            _eAgreementService = eAgreementService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("myAgreements")]
        [Authorize(Roles = "Bidder")]
        public async Task<IActionResult> GetMyAgreements()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest(
                    ApiResponseHelper.CreateBadRequest<string>("Please provide a valid userId.")
                );
            }

            var result = await _eAgreementService.GetByUserIdAsync(userId.ToString());
            return Ok(
                ApiResponseHelper.CreateSuccess<object>(
                    result,
                    "Agreements retrieved successfully."
                )
            );
        }

        [HttpGet("{id}/download")]
        [Authorize(Roles = "Bidder")]
        public async Task<IActionResult> DownloadAgreementFile(Guid id)
        {
            var agreement = await _eAgreementService.GetByIdAsync(id);
            if (agreement == null || agreement.File == null)
            {
                return NotFound("Agreement or file not found.");
            }

            var fileName = $"agreement_{agreement.Id}.pdf";
            var contentType = "application/pdf";

            return File(agreement.File, contentType, fileName);
        }

        [HttpGet("{biddingId}")]
        [Authorize(Roles = "Bidder")]
        public async Task<IActionResult> GetAgreementsByBiddingId(Guid biddingId)
        {
            var agreements = await _eAgreementService.GetByBiddingIdAsync(biddingId);
            if (agreements == null || !agreements.Any())
            {
                return NotFound(
                    ApiResponseHelper.CreateNotFound<string>(
                        "No agreements found for the provided bidding ID."
                    )
                );
            }
            return Ok(
                ApiResponseHelper.CreateSuccess<IEnumerable<EAgreement>>(
                    agreements,
                    "Agreements retrieved successfully."
                )
            );
        }
    }
}
