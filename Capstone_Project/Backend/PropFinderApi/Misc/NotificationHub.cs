using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PropFinderApi.Misc
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");

            var user = Context.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "Buyer")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Buyers");
                }
                else if (role == "Lister" && !string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId); // Private group for lister
                }
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Buyer")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Buyers");
            }
            else if (role == "Lister" && !string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

    }

}