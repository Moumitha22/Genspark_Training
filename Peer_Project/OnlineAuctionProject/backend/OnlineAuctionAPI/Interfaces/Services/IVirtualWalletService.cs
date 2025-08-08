using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IVirtualWalletService
{
    Task<User> AddVirtualWalletToUserAsync(Guid userId, VirtualWalletAddDto virtualWalletDto);

    Task<User> AddFundsToVirtualWalletAsync(Guid userId, decimal amount);
    Task<List<VirtualWalletHistory>> GetVirtualWalletHistoryByUserIdAsync(Guid userId);
    Task<VirtualWallet> GetVirtualWalletByUserIdAsync(Guid userId);
}
