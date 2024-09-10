namespace Book_Api.Middlewares
{
    public class CacheConfig
    {
        public redisConfig redis { get; set; }
    }
    public class redisConfig
    {
        public int expirationTime { get; set; }
    }
}
