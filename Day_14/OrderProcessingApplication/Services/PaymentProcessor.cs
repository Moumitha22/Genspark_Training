using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        public virtual bool ProcessPayment(double amount)
        {
            if (amount > 0)
            {
                Console.WriteLine($"\nPayment of Rs.{amount} processed successfully.");
                return true;
            }
            return false;
        }
    }
}
