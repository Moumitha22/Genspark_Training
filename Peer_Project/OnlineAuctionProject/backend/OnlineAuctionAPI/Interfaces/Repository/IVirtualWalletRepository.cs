using System;
using System.Threading.Tasks;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces
{
    public interface IVirtualWalletRepository : IRepository<Guid, VirtualWallet>
    {
        Task AddVirtualWalletAsync(Guid userId, VirtualWalletAddDto dto);
        Task AddFundsToWalletAndHistoryAsync(Guid userId, decimal amount);
        Task<List<VirtualWalletHistory>> GetVirtualWalletHistoryByUserIdAsync(Guid userId);
        Task AddHistoryAsync(VirtualWalletHistory history);
        Task<VirtualWallet?> GetByUserIdAsync(Guid userId);
        // Task UpdateAsync(VirtualWallet wallet);
    }
}
