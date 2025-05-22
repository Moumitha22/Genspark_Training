using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApplication.Exceptions
{
    public class PaymentFailedException : Exception
    {
        private string _message = "Collection is empty";

        public PaymentFailedException(string msg)
        {
            _message = msg;
        }

        public override string Message => _message;
    }
}
