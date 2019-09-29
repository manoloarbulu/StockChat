using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StockChat.Models
{
    public class StockModel
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string Symbol { get; set; }

        [DataMember(Order = 1)]
        public string Date { get; set; }

        [DataMember(Order = 2)]
        public string Time { get; set; }

        [DataMember(Order = 3)]
        public string Open { get; set; }

        [DataMember(Order = 4)]
        public string High { get; set; }

        [DataMember(Order = 5)]
        public string Low { get; set; }

        [DataMember(Order = 6)]
        public string Close { get; set; }

        [DataMember(Order = 7)]
        public string Volume { get; set; }
    }
}
