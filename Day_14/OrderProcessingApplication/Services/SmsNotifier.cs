using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Services
{
    public class SmsNotifier : ISmsNotifier
    {
        public void SendSMS(string message)
        {
            Console.WriteLine($"\nNotification Sent via SMS: {message}");
        }
    }
}
