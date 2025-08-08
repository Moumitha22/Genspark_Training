using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Services
{
    public class EAgreementService : IEAgreementService
    {
        private readonly IEAgreementRepository _eAgreementRepository;
        private readonly IBidItemRepository _bidItemRepository;

        public EAgreementService(
            IEAgreementRepository eAgreementRepository,
            IBidItemRepository bidItemRepository
        )
        {
            _eAgreementRepository = eAgreementRepository;
            _bidItemRepository = bidItemRepository;
        }

        public async Task<IEnumerable<object>> GetByUserIdAsync(string userId)
        {
            Guid userGuid = Guid.Parse(userId);

            var agreements = await _eAgreementRepository.GetAllWithBidItemsAsync();

            var myAgreements = agreements
                .Where(a => a.Bidding != null && a.Bidding.BidderId == userGuid)
                .Select(agreement => new
                {
                    agreement.Id,
                    agreement.AuctionItemId,
                    agreement.BiddingId,
                    agreement.CreatedAt,
                    FileUrl = $"http://localhost:5230/api/v1/EAgreement/{agreement.Id}/download",
                })
                .ToList();

            return myAgreements;
        }

        public async Task<EAgreement?> GetByIdAsync(Guid id)
        {
            return await _eAgreementRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<EAgreement>> GetByBiddingIdAsync(Guid biddingId)
        {
            return await _eAgreementRepository.GetByBiddingIdAsync(biddingId);
        }
    }
}
