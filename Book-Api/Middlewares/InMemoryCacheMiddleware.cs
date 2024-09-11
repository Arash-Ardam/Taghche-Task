using Book_Infra;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;

namespace Book_Api.Middlewares
{
    public class InMemoryCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<CacheConfig> _options;
        private readonly IDistributedCache _memCache;

        public InMemoryCacheMiddleware(RequestDelegate next, IDistributedCache memoryCache, IOptions<CacheConfig> options)
        {
            _next = next;
            _options = options;
            _memCache = memoryCache;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var key = cacheKeyGenerator.GenerateKeyFromHttpContext(context);

            var cacheContent = _memCache.Get(key);

            if (cacheContent != null)
            {
                

                context.Response.ContentType = "application/json";

                var result = Encoding.UTF8.GetString(cacheContent);

                var exrtact = JsonConvert.DeserializeObject<MemoryCacheObject>(result);

                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = DateTimeOffset.UtcNow - exrtact.dateModified,
                    
                };

                await context.Response.WriteAsync(exrtact.value);
                return;
            }

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(context.Response.Body);
                var contextResult = await streamReader.ReadToEndAsync();

                if (contextResult != null)
                {
                    MemoryCacheObject entry = new MemoryCacheObject()
                    {
                        value = contextResult,
                        dateModified = DateTime.UtcNow
                    };
                    
                    var jsonSerializedEntry =JsonConvert.SerializeObject(entry);
                    
                    await _memCache.SetAsync(key, Encoding.UTF8.GetBytes(jsonSerializedEntry),
                        options: new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Value.inMemoryCache.expirationTime)
                        });
                }
        }

    }

    internal class MemoryCacheObject
    {
        public string value { get; set; }
        public DateTimeOffset dateModified { get; set; }
    }
}
