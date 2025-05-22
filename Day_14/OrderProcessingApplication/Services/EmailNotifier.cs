using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class EmailNotifier : INotifier
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"\nNotification Sent: {message}");
        }
    }
}
