using RabbitMQ.Client;


ConnectionFactory connectionFactory = new ConnectionFactory()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Taghche-Rabbit-Client",
};

IConnection connection = connectionFactory.CreateConnection();
IModel cannel = connection.CreateModel();

string exchangeName = "Book-Exchange";
string routingKey = "book-modified";
string queue = "BookModified-Ack";

cannel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false, autoDelete: false);
cannel.QueueDeclare(queue, false, false, false);
cannel.QueueBind(queue, exchangeName, routingKey);



