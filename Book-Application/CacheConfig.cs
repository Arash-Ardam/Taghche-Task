using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Application
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
        public int expirationTime { get; set; }
    }
}
