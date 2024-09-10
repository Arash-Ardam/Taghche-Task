using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Book_Api.Middlewares
{
    public class RedisMiddleware
    {
        private readonly IOptions<CacheConfig> _options;
        private readonly RequestDelegate _next;
        private readonly IDatabase _redis;

        public RedisMiddleware(RequestDelegate next, IConnectionMultiplexer muxer,IOptions<CacheConfig> options)
        {
            _options = options;
            _next = next;
            _redis = muxer.GetDatabase();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != HttpMethod.Get.ToString())
            {
                await _next(context);
            }

            var key = new RedisKey(context.Request.Path);
            var result = _redis.StringGetAsync(key);
            
            if (result.Result.HasValue) 
            {
                var liveExpirationTime = DateTime.Now - await _redis.KeyExpireTimeAsync(key);

                context.Response.ContentType = "application/json";
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = liveExpirationTime
                };

                await context.Response.WriteAsync(result.Result.ToString());
                return;
            }

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(context.Response.Body);
                var contextResult = await streamReader.ReadToEndAsync();
                if (contextResult != null)
                {
                    var expirationTime = TimeSpan.FromSeconds(_options.Value.redis.expirationTime);
                    var setTask = _redis.StringSetAsync(key, new RedisValue(contextResult));
                    var expireTask = _redis.KeyExpireAsync(key, expirationTime);

                    await Task.WhenAll(setTask, expireTask);
                }
        }

    }
}
