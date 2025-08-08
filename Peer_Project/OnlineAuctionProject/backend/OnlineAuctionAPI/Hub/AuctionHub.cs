using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OnlineAuctionAPI.Hubs;

[Authorize]
public class AuctionHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var user = Context.User;

        var userRole = user?.FindFirst(ClaimTypes.Role)?.Value;
        var userId = user?.FindFirst("UserId")?.Value;

        if (string.Equals(userRole, "Bidder", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(connectionId, $"Bidder_{userId}");
            }

            await Groups.AddToGroupAsync(connectionId, "Bidders");
        }

        if (
            string.Equals(userRole, "Seller", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(userId)
        )
        {
            await Groups.AddToGroupAsync(connectionId, $"Seller_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public async Task SendAuctionItemAdded(object result)
    {
        await Clients.Group("Bidders").SendAsync("AuctionItemAdded", result);
    }

    public async Task SendWinningIdUpdatedToWinnerAndSeller(
        string winnerId,
        string sellerId,
        object result
    )
    {
        if (!string.IsNullOrEmpty(winnerId))
        {
            await Clients.Group($"Bidder_{winnerId}").SendAsync("WinningIdUpdated", result);
        }

        if (!string.IsNullOrEmpty(sellerId))
        {
            await Clients.Group($"Seller_{sellerId}").SendAsync("WinningIdUpdated", result);
        }
    }
}
