using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockChat.Service
{
    public class ConsumeQueueService: BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public ConsumeQueueService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsumeQueueService>();
            InitService();
        }

        private void InitService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("Stock.Exchange", ExchangeType.Topic);
            _channel.QueueDeclare(queue: "StockQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "StockQueue", exchange: "Stock.Exchange", routingKey: "StockQueue", arguments: null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += QueueServiceShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) => { 
                //Received message
                var content = Encoding.UTF8.GetString(ea.Body);

                //Process message
                HandleMessage(content);

                //Acknowledge the messages has been delivered
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;  
            consumer.Registered += OnConsumerRegistered;  
            consumer.Unregistered += OnConsumerUnregistered;  
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: "StockQueue", autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async void HandleMessage(string content)
        {
            var connection = new HubConnectionBuilder().WithUrl("https://localhost:44397/ChatRoom").Build();
            await connection.StartAsync();
            await connection.InvokeAsync("SendBotMessage", content);
            _logger.LogInformation(content);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)  {  }  
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) {  }  
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) {  }  
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) {  } 
        private void QueueServiceShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
