namespace Book_Api.Middlewares
{
    public class CacheConfig
    {
        public redisConfig redis { get; set; }
        public InMemoryCache inMemoryCache { get; set; }
    }
    public class redisConfig
    {
        public int expirationTime { get; set; }
    }
    public class InMemoryCache
    {
        public int expirationTime { get; set;}
    }
}
