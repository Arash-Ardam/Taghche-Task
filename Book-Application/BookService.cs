using Book_Infra;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net.Http.Json;
using System.Text;

namespace Book_Application
{
    public class BookService
    {
        private readonly IOptions<CacheConfig> _options;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _httpclient;
        private readonly IDistributedCache _memCache;
        private readonly IDatabase _redisCache;

        public BookService(IHttpClientFactory httpClientFactory, IDistributedCache memCache, IConnectionMultiplexer muxer, IOptions<CacheConfig> options)
        {
            _options = options;
            _factory = httpClientFactory;
            _httpclient = _factory.CreateClient("Book-Client");
            _memCache = memCache;
            _redisCache = muxer.GetDatabase();
            
        }



        public async Task<BookDto> GetBookAsync(short id)
        {
            try
            {
                var cacheData = GetBookFromCache(id);

                if (cacheData == null)
                {
                    var response = await GetBookFromApiAsync(id);

                    string key = cacheKeyGenerator.GenerateKeyFromId(id);

                    await WriteDataToMemCache(key, response.ToString());

                    await WriteDatatoRedisCache(key, response.ToString());

                    return response;
                }

                return cacheData;
               
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<BookDto?> GetBookFromApiAsync(short id) 
        {
            string route = $"{_httpclient.BaseAddress}/{id}";

            var response = await _httpclient.GetAsync(route);

            var content = await response.Content.ReadFromJsonAsync<BookDto>();

            return content;
        }

        private BookDto? GetBookFromCache(short id)
        {
            var key = cacheKeyGenerator.GenerateKeyFromId(id);

            var memCacheData = _memCache.Get(key);
            if (memCacheData != null)
            {
                var result = JsonConvert.DeserializeObject<BookDto>(Encoding.UTF8.GetString(memCacheData));

                return result;

            }

            var redisCacheData = _redisCache.StringGetAsync(new RedisKey(key));
            if (redisCacheData.Result.HasValue)
            {
                var result = JsonConvert.DeserializeObject<BookDto>(Encoding.UTF8.GetString(redisCacheData.Result));

                WriteDataToMemCache(key, redisCacheData.Result);

                return result;
            }

            return null;
        }


        private async Task WriteDatatoRedisCache(string key , string data)
        {
            var setTask = _redisCache.StringSetAsync(key, new RedisValue(data));

            var expireTask = _redisCache.KeyExpireAsync(key, TimeSpan.FromSeconds(_options.Value.redis.expirationTime));

            await Task.WhenAll(setTask, expireTask);
        }

        private async Task WriteDataToMemCache(string key,string data)
        {
            await _memCache.SetAsync(
                key, Encoding.UTF8.GetBytes(data),
                options: new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Value.inMemoryCache.expirationTime)
                });
        }


    }
}
