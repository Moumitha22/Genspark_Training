using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Services;

public class VirtualWalletService : IVirtualWalletService
{
    private readonly IVirtualWalletRepository _virtualWalletRepository;
    private readonly IUserRepository _userRepository;

    public VirtualWalletService(
        IVirtualWalletRepository virtualWalletRepository,
        IUserRepository userRepository
    )
    {
        _virtualWalletRepository = virtualWalletRepository;
        _userRepository = userRepository;
    }

    public async Task<User> AddFundsToVirtualWalletAsync(Guid userId, decimal amount)
    {
        await _virtualWalletRepository.AddFundsToWalletAndHistoryAsync(userId, amount);
        var updatedUser = await _userRepository.GetByIdWithVirtualWalletAsync(userId);
        return updatedUser!;
    }

    public async Task<User> AddVirtualWalletToUserAsync(
        Guid userId,
        VirtualWalletAddDto virtualWalletDto
    )
    {
        var user = await _userRepository.GetByIdWithVirtualWalletAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with Id {userId} not found");

        if (user.VirtualWallet != null)
            throw new InvalidOperationException("User already has a virtual wallet.");

        var balance = (user.VirtualWallet?.Balance ?? 0) + virtualWalletDto.Balance;
        if (balance > 5_000_000)
            throw new InvalidDataException("Exceed the amount holding limit");

        if (virtualWalletDto.Balance <= 0)
            throw new InvalidDataException("Balance must be greater than zero");

        await _virtualWalletRepository.AddVirtualWalletAsync(userId, virtualWalletDto);

        var updatedUser = await _userRepository.GetByIdWithVirtualWalletAsync(userId);
        return updatedUser!;
    }

    public async Task<VirtualWallet> GetVirtualWalletByUserIdAsync(Guid userId)
    {
        var wallet = await _virtualWalletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            throw new NotFoundException($"Virtual wallet for user {userId} not found");
        return wallet;
    }

    public async Task<List<VirtualWalletHistory>> GetVirtualWalletHistoryByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        return await _virtualWalletRepository.GetVirtualWalletHistoryByUserIdAsync(userId);
    }
}
