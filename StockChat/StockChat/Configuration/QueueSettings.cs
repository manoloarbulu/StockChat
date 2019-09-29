using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockChat.Configuration
{
    public class QueueSettings
    {
        public string Host { get; set; }
        public string User { get; set;}
        public string Password { get; set;}
        public string QueueName { get; set;}
        public string QueueExchange { get; set;}
    }
}
