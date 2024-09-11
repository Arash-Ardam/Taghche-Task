
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Book_Api.RabbitBackgroundService
{
    public class RabbitBackgroundService : BackgroundService
    {
        private ConnectionFactory _connFactory;
        private IDistributedCache _memCache;
        private IDatabase _redis;

        public RabbitBackgroundService(ConnectionFactory connectionFactory,IDistributedCache memoryCache,IConnectionMultiplexer muxer)
        {
            _connFactory = connectionFactory;
            _memCache = memoryCache;
            _redis = muxer.GetDatabase();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string result = string.Empty;

            _connFactory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            _connFactory.ClientProvidedName = "Taghche-Rabbit-RecieverClient";

            IConnection connection = _connFactory.CreateConnection();

            string exchangeName = "Book-Exchange";
            string routingKey = "book-modified";
            string queue = "BookModified-Ack";

            IModel cannel = connection.CreateModel();
            cannel.QueueBind(queue, exchangeName, routingKey);

            var consumer = new EventingBasicConsumer(cannel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                result = message;

                if (message.Contains("modified"))
                {
                    object t;
                    var bookId = Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers["BookId"]);
                    var key = $"BookId:{bookId}";

                    _memCache.Remove(key);
                    _redis.KeyDelete(key);
                }

                cannel.BasicAck(args.DeliveryTag, false);

            };
            cannel.BasicConsume(queue, autoAck: false, consumer);

            return Task.CompletedTask;

        }

    }
}
