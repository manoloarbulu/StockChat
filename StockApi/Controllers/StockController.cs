using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockApi.Model;

namespace StockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        // GET api/stock/apple.us
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            //Consume Stock Endpoint in async way
            ProcessStockRequest(id);
            return Ok();
        }

        private static async void ProcessStockRequest(string id)
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/CSV"));
                client.DefaultRequestHeaders.Add("User-Agent", "Test Stock Bot");

                //Consumes Endpoint and gets stock data
                var streamTask = await client.GetStringAsync("https://stooq.com/q/l/?s="+id+"&f=sd2t2ohlcv&h&e=csv");

                //Parses received data to proper data members
                var stockData = new StockModel(streamTask);
                
                //Serialize StockData and send Message to Queue
            }
        }
    }
}
