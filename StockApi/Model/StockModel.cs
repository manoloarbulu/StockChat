using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StockApi.Model
{
    [DataContract]
    public class StockModel
    {
        [DataMember(IsRequired = true)]
        public string Symbol { get; set; }

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Open { get; set; }

        [DataMember]
        public string High { get; set; }

        [DataMember]
        public string Low { get; set; }

        [DataMember]
        public string Close { get; set; }

        [DataMember]
        public string Volume { get; set; }

        public StockModel(string receivedData)
        { }
    }
}
