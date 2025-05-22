using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class CreditCardPaymentProcessor : PaymentProcessor
    {
        public override bool ProcessPayment(double amount)
        {
            Console.WriteLine($"Processing credit card payment of Rs.{amount}.");
            return true; 
        }
    }
}
