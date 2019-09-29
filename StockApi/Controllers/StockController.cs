using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
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
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("message", nameof(id));
            }
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
                
                //Send Message to the Queue
                SendMessageToQueue(stockData);
            }
        }

        private static void SendMessageToQueue(StockModel stock)
        {
            //Queue location and Queue name should be in configuration
            var serializer = new DataContractSerializer(typeof(StockModel));
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "StockQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    using(var writer = new MemoryStream())
                    {
                        serializer.WriteObject(writer, stock);
                        var message = writer.ToArray();

                        channel.BasicPublish(exchange: "",
                            routingKey: "StockQueue",
                            basicProperties: null,
                            body: message);
                    }
                }
            }
        }
    }
}
