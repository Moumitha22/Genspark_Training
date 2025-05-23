using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingApplication.Interfaces
{
    internal interface ISmsNotifier
    {
        void SendSMS(string message);
    }
}
