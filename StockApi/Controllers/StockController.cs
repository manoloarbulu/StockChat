using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StockApi.Configuration;
using StockApi.Model;

namespace StockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private QueueSettings Settings { get; set; }

        public StockController(IOptions<QueueSettings> settings)
        {
            Settings = settings.Value;
        }

        // GET api/stock/apple.us
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
                //throw new ArgumentException("message", nameof(id));
            }
            //Consume Stock Endpoint in async way
            ProcessStockRequest(id, Settings);
            return Ok();
        }

        private static async void ProcessStockRequest(string id, QueueSettings settings)
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/CSV"));
                client.DefaultRequestHeaders.Add("User-Agent", "Test Stock API Consumer");

                //Consumes Endpoint and gets stock data
                var streamTask = await client.GetStringAsync("https://stooq.com/q/l/?s="+id+"&f=sd2t2ohlcv&h&e=csv");

                //Parses received data to proper data members
                var stockData = new StockModel(streamTask);
                
                //Send Message to the Queue
                SendMessageToQueue(stockData, settings);
            }
        }

        private static void SendMessageToQueue(StockModel stock, QueueSettings settings)
        {
            //Queue location and Queue name should be in configuration
            var serializer = new DataContractJsonSerializer(typeof(StockModel));

            var factory = new ConnectionFactory() {
                HostName = settings.Host,
                UserName = settings.User,
                Password = settings.Password
            };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    using(var writer = new MemoryStream())
                    {
                        serializer.WriteObject(writer, stock);
                        var message = writer.ToArray();

                        channel.BasicPublish(exchange: "",
                            routingKey: settings.QueueName,
                            basicProperties: null,
                            body: message);
                    }
                }
            }
        }
    }
}
