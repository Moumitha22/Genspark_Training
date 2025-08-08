using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Migrations;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Repositories
{
    public class VirtualWalletRepository : Repository<Guid, VirtualWallet>, IVirtualWalletRepository
    {
        public VirtualWalletRepository(AuctionContext context)
            : base(context) { }

        public async Task AddVirtualWalletAsync(Guid userId, VirtualWalletAddDto dto)
        {
            using var transaction = await _auctionContext.Database.BeginTransactionAsync();
            try
            {
                var virtualWallet = new VirtualWallet
                {
                    UserId = userId,
                    Balance = dto.Balance,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _auctionContext.VirtualWallets.AddAsync(virtualWallet);

                var history = new VirtualWalletHistory
                {
                    Id = Guid.NewGuid(),
                    VirtualWalletId = virtualWallet.Id,
                    Amount = dto.Balance,
                    TransactionDate = DateTime.UtcNow,
                };

                await _auctionContext.VirtualWalletHistories.AddAsync(history);

                await _auctionContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RepositoryOperationException("Add virtual wallet and history", ex);
            }
        }

        public async Task AddFundsToWalletAndHistoryAsync(Guid userId, decimal amount)
        {
            using var transaction = await _auctionContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _auctionContext
                    .Users.Include(u => u.VirtualWallet)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.VirtualWallet == null)
                    throw new Exception("User or wallet not found");

                user.VirtualWallet.Balance += amount;
                user.VirtualWallet.UpdatedAt = DateTime.UtcNow;

                var history = new VirtualWalletHistory
                {
                    Id = Guid.NewGuid(),
                    VirtualWalletId = user.VirtualWallet.Id,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                };

                await _auctionContext.VirtualWalletHistories.AddAsync(history);
                await _auctionContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RepositoryOperationException("Add funds to wallet and history", ex);
            }
        }

        public async Task AddHistoryAsync(VirtualWalletHistory history)
        {
            try
            {
                _auctionContext.VirtualWalletHistories.Add(history);
                await _auctionContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Adding virtual wallet history", ex);
            }
        }

        public async Task<VirtualWallet?> GetByUserIdAsync(Guid userId)
        {
            try
            {
                return await _auctionContext.VirtualWallets.FirstOrDefaultAsync(w =>
                    w.UserId == userId
                );
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Getting virtual wallet by userId", ex);
            }
        }

        public async Task<List<VirtualWalletHistory>> GetVirtualWalletHistoryByUserIdAsync(
            Guid userId
        )
        {
            try
            {
                var user = await _auctionContext
                    .Users.Include(u => u.VirtualWallet)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.VirtualWallet == null)
                    throw new Exception("User or wallet not found");

                var history = await _auctionContext
                    .VirtualWalletHistories.Where(h => h.VirtualWalletId == user.VirtualWallet.Id)
                    .OrderByDescending(h => h.TransactionDate)
                    .Select(h => new VirtualWalletHistory
                    {
                        Id = h.Id,
                        VirtualWalletId = h.VirtualWalletId,
                        Amount = h.Amount,
                        Description = h.Description,
                        TransactionDate = h.TransactionDate,
                    })
                    .ToListAsync();

                return history;
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Get virtual wallet history by userId", ex);
            }
        }

        // public async Task UpdateAsync(VirtualWallet wallet)
        // {
        //     try
        //     {
        //         _auctionContext.VirtualWallets.Update(wallet);
        //         await _auctionContext.SaveChangesAsync();
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new RepositoryOperationException("Updating virtual wallet", ex);
        //     }
        // }
    }
}
