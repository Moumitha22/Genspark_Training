using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class GooglePayPaymentProcessor : PaymentProcessor
    {
        public override bool ProcessPayment(double amount)
        {
            Console.WriteLine($"Processing Google Pay payment of Rs.{amount}.");
            return true;
        }
    }
}
