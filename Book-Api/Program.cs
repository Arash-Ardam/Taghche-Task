
using Book_Api.Controllers;
using Book_Api.Middlewares;
using Book_Application;
using Book_Infra;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Book_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHttpClient("Book-Client", httpclient => httpclient.BaseAddress = new Uri(builder.Configuration.GetSection("Book-Api:api-address").Value));
            builder.Services.Configure<CacheConfig>(builder.Configuration.GetSection(nameof(CacheConfig)));
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6000"));
            builder.Services.AddHostedService<RabbitBackgroundService.RabbitBackgroundService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<ConnectionFactory>();
            builder.Services.AddSingleton<cacheKeyGenerator>();
            builder.Services.AddScoped<BookService>();
            builder.Services.AddScoped<BookController>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.UseAuthorization();
            

            app.MapControllers();
            
            app.UseMiddleware<StreamBuilderMiddleware>();
            app.UseMiddleware<InMemoryCacheMiddleware>();
            app.UseMiddleware<RedisMiddleware>();


            app.Run();
        }
    }
}
