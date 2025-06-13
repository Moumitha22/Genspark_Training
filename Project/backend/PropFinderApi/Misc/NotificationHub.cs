using Microsoft.AspNetCore.SignalR;

namespace PropFinderApi.Misc
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

    }
}