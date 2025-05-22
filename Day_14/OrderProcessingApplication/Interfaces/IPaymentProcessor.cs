using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApplication.Interfaces
{
    public interface IPaymentProcessor
    {
        bool ProcessPayment(double amount);
    }

}
