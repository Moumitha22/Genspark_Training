using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Service
{
    public class AuctionCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AuctionHub> _hubContext;

        public AuctionCheckService(
            IServiceProvider serviceProvider,
            IHubContext<AuctionHub> hubContext
        )
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var auctionItemRepository =
                        scope.ServiceProvider.GetRequiredService<IAuctionItemRepository>();
                    var bidItemRepository =
                        scope.ServiceProvider.GetRequiredService<IBidItemRepository>();
                    var eAgreementRepository =
                        scope.ServiceProvider.GetRequiredService<IEAgreementRepository>();
                    var virtualWalletRepository =
                        scope.ServiceProvider.GetRequiredService<IVirtualWalletRepository>();
                    var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.UtcNow;

                    var toGoLiveAuctions =
                        await auctionItemRepository.GetAllUpcomingAndShouldBeLiveAsync(now);

                    foreach (var auction in toGoLiveAuctions)
                    {
                        var previousStatus = auction.Status;
                        if (previousStatus != AuctionStatus.Live)
                        {
                            auction.Status = AuctionStatus.Live;
                            await auctionItemRepository.Update(auction.Id, auction);

                            var result = new
                            {
                                id = auction.Id,
                                name = auction.Name,
                                status = auction.Status.ToString(),
                                winnerId = (Guid?)null,
                                startTime = auction.StartTime,
                                endTime = auction.EndTime,
                                sellerId = auction.SellerId,
                                createdAt = auction.CreatedAt,
                                updatedAt = auction.UpdatedAt,
                            };
                            await _hubContext
                                .Clients.Group("Bidders")
                                .SendAsync("AuctionStatusUpdated", result);
                        }
                    }

                    var endedAuctions = await auctionItemRepository.GetAllEndedAndNotCompletedAsync(
                        now
                    );

                    foreach (var auction in endedAuctions)
                    {
                        if (auction.Status == AuctionStatus.Cancelled)
                        {
                            continue;
                        }
                        var previousStatus = auction.Status;
                        var highestBid = await bidItemRepository.GetHighestBidAsync(auction.Id);

                        if (highestBid != null && highestBid.Amount >= auction.ReservePrice)
                        {
                            auction.WinnerId = highestBid.Id;
                            var result = new
                            {
                                id = auction.Id,
                                name = auction.Name,
                                winnerId = auction.WinnerId,
                                bidderId = highestBid.BidderId,
                                sellerId = auction.SellerId,
                                createdAt = auction.CreatedAt,
                                amount = highestBid.Amount,
                            };

                            await _hubContext
                                .Clients.Group($"Bidder_{highestBid.BidderId}")
                                .SendAsync("WinningIdUpdated", result);

                            await _hubContext
                                .Clients.Group($"Seller_{auction.SellerId}")
                                .SendAsync("WinningIdUpdated", result);

                            var winnerWallet = await virtualWalletRepository.GetByUserIdAsync(
                                highestBid.BidderId
                            );
                            if (winnerWallet != null && winnerWallet.Balance >= highestBid.Amount)
                            {
                                winnerWallet.Balance -= highestBid.Amount;
                                winnerWallet.UpdatedAt = DateTime.UtcNow;
                                await virtualWalletRepository.Update(winnerWallet.Id, winnerWallet);
                                var history = new VirtualWalletHistory
                                {
                                    VirtualWalletId = winnerWallet.Id,
                                    Amount = -highestBid.Amount,
                                    Description = $"Deducted for winning auction {auction.Id}",
                                    TransactionDate = DateTime.UtcNow,
                                };
                                await virtualWalletRepository.AddHistoryAsync(history);
                            }
                            else if (winnerWallet == null)
                            {
                                throw new NullValueException("Winner's virtual wallet not found.");
                            }
                            else if (winnerWallet.Balance < highestBid.Amount)
                            {
                                throw new InvalidDataException(
                                    "Insufficient balance in winner's virtual wallet."
                                );
                            }
                            else
                            {
                                continue;
                            }

                            byte[] pdfBytes = PdfHelper.GenerateAgreementPdf(auction, highestBid);

                            var agreement = new EAgreement
                            {
                                Id = Guid.NewGuid(),
                                AuctionItemId = auction.Id,
                                BiddingId = highestBid.Id,
                                File = pdfBytes,
                                CreatedAt = DateTime.UtcNow,
                            };
                            await eAgreementRepository.Add(agreement);
                            auction.Status = AuctionStatus.Completed;
                            var recipient =
                                await _userService.GetUserByIdAsync(highestBid.BidderId)
                                ?? throw new NotFoundException(
                                    $"Recipient with ID {highestBid.BidderId} not found."
                                );
                            var pdfFilePath = PdfHelper.SavePdfToFile(
                                pdfBytes,
                                $"agreement_{agreement.Id} - {recipient.Username} .pdf"
                            );

                            try
                            {
                                await _emailService.SendEmailAsync(
                                    recipient.Email,
                                    "Auction Won",
                                    $"Congratulations! You have won the auction for {auction.Name}.",
                                    pdfFilePath
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Failed to send email to {recipient.Email}: {ex.Message}"
                                );
                            }
                        }
                        else
                        {
                            auction.WinnerId = null;
                            auction.Status = AuctionStatus.Closed;
                        }

                        if (auction.Status != previousStatus)
                        {
                            await auctionItemRepository.Update(auction.Id, auction);

                            var result = new
                            {
                                id = auction.Id,
                                name = auction.Name,
                                status = auction.Status.ToString(),
                                winnerId = auction.WinnerId,
                                startTime = auction.StartTime,
                                endTime = auction.EndTime,
                                sellerId = auction.SellerId,
                                createdAt = auction.CreatedAt,
                                updatedAt = auction.UpdatedAt,
                            };
                            await _hubContext
                                .Clients.Group("Bidders")
                                .SendAsync("AuctionStatusUpdated", result);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
