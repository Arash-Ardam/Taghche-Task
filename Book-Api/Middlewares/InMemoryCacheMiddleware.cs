using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
            var key = context.Request.Path.ToString();

            var cacheContent = _memCache.Get(key);

            if (cacheContent != null)
            {

                context.Response.ContentType = "application/json";

                var result = Encoding.UTF8.GetString(cacheContent);

                await context.Response.WriteAsync(result);
                return;
            }

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(context.Response.Body);
                var contextResult = await streamReader.ReadToEndAsync();

                if (contextResult != null)
                {
                    await _memCache.SetAsync(key, Encoding.UTF8.GetBytes(contextResult),
                        options: new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Value.inMemoryCache.expirationTime)
                        });
                }
        }

    }

    //internal class MemoryCacheObject
    //{
    //    public
    //}
}
