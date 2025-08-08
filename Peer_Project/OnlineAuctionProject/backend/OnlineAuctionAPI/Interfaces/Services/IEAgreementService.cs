using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Services
{
    public interface IEAgreementService
    {
        Task<IEnumerable<object>> GetByUserIdAsync(string userId);
        Task<EAgreement?> GetByIdAsync(Guid id);
        Task<IEnumerable<EAgreement>> GetByBiddingIdAsync(Guid biddingId);
    }
}
