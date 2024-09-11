using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Book_Api.Middlewares
{
    public class RabbitMiddleware
    {
        private readonly RequestDelegate _next;
        private ConnectionFactory _connFactory;

        public RabbitMiddleware(RequestDelegate next,ConnectionFactory connectionFactory)
        {
            _next = next;
            _connFactory = connectionFactory;
            

        }

        public async Task InvokeAsync(HttpContext context)
        {

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
                Console.WriteLine(message);
                cannel.BasicAck(args.DeliveryTag, false);

            };
            cannel.BasicConsume(queue, autoAck: false, consumer);

            await _next(context);

            string t = "5";

        }









    }
}
