using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockChat.Configuration;

namespace StockChat.Service
{
    public class ConsumeQueueService: BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly QueueSettings _settings;

        public ConsumeQueueService(ILoggerFactory loggerFactory, IOptions<QueueSettings> settings)
        {
            _logger = loggerFactory.CreateLogger<ConsumeQueueService>();
            _settings = settings.Value;
            InitService();
        }

        private void InitService()
        {
            var factory = new ConnectionFactory() {
                HostName = _settings.Host,
                UserName = _settings.User,
                Password = _settings.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_settings.QueueExchange, ExchangeType.Topic);
            _channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _settings.QueueName, exchange: _settings.QueueExchange, routingKey: _settings.QueueName, arguments: null);
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

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);
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
