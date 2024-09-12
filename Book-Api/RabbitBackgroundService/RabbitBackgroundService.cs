
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Newtonsoft.Json;
using Book_Infra;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace Book_Api.RabbitBackgroundService
{
    public class RabbitBackgroundService : BackgroundService
    {
        private Policy _policy;
        private ConnectionFactory _connFactory;
        private IDistributedCache _memCache;
        private IDatabase _redis;
        private IConnection _connection;
        public RabbitBackgroundService(ConnectionFactory connectionFactory,IDistributedCache memoryCache,IConnectionMultiplexer muxer)
        {
            _connFactory = connectionFactory;
            _memCache = memoryCache;
            _redis = muxer.GetDatabase();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string result = string.Empty;
            _connFactory.Uri = new Uri("amqp://guest:guest@localhost:5672/");
            _connFactory.ClientProvidedName = "Taghche-Rabbit-RecieverClient";
            _connFactory.AutomaticRecoveryEnabled = true;


            Policy.Handle<BrokerUnreachableException>()
                .WaitAndRetry(5, x => TimeSpan.FromSeconds(15))
                .Execute(() => {
                  _connection =  _connFactory.CreateConnection();
                });


            string exchangeName = "Book-Exchange";
            string routingKey = "book-modified";
            string queue = "BookModified-Ack";

            IModel cannel = _connection.CreateModel();
            cannel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false, autoDelete: false);
            cannel.QueueDeclare(queue, false, false, false);
            cannel.QueueBind(queue, exchangeName, routingKey);

            var consumer = new EventingBasicConsumer(cannel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();

                string messageString = Encoding.UTF8.GetString(body);

                BookMessage message = JsonConvert.DeserializeObject<BookMessage>(messageString);

                if (message != null)
                {
                    var key = cacheKeyGenerator.GenerateKeyFromId(message.BookId);

                    _memCache.Remove(key);
                    if (_redis != null)
                    {
                        _redis.KeyDelete(key);
                    }

                    Console.WriteLine($"consumer message : {message.Message}");
                }
                cannel.BasicAck(args.DeliveryTag, false);

            };
            cannel.BasicConsume(queue, autoAck: false, consumer);

            return Task.CompletedTask;

        }


    }

    internal class BookMessage
    {
        public int BookId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
