using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StockApi.Model
{
    [DataContract]
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

        public StockModel(string receivedData)
        {
            if (string.IsNullOrWhiteSpace(receivedData))
                return;

            string[] data = Regex.Replace(receivedData, @"\n", string.Empty).
                Split('\r', StringSplitOptions.RemoveEmptyEntries)[1].
                Split(',', StringSplitOptions.RemoveEmptyEntries);

            for(int i=0; i<data.Length; i++)
            {
                switch(i)
                {
                    case 0:
                        Symbol = data[i];
                        break;
                    case 1:
                        Date = data[i];
                        break;
                    case 2:
                        Time = data[i];
                        break;
                    case 3:
                        Open = data[i];
                        break;
                    case 4:
                        High = data[i];
                        break;
                    case 5:
                        Low = data[i];
                        break;
                    case 6:
                        Close = data[i];
                        break;
                    case 7:
                        Volume = data[i];
                        break;
                }
            }
        }
    }
}
