using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Interfaces
{
    public interface IEAgreementRepository : IRepository<Guid, EAgreement>
    {
        Task<EAgreement?> GetByIdAsync(Guid id);
        Task<IEnumerable<EAgreement>> GetAllWithBidItemsAsync();
        Task<IEnumerable<EAgreement>> GetByBiddingIdAsync(Guid biddingId);
    }
}
