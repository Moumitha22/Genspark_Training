using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class EmailNotifier : IEmailNotifier
    {
        public void SendEmail(string message)
        {
            Console.WriteLine($"\nNotification Sent via Email: {message}");
        }
    }
}
