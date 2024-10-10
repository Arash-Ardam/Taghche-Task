namespace Book_Api.RabbitBackgroundService
{
    public class RabbitConfig
    {
        public string Url { get; set; } = string.Empty;
        public string ClientProvidedName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;

    }
}
