
using Book_Api.Controllers;
using Book_Api.Middlewares;
using Book_Api.RabbitBackgroundService;
using Book_Application;
using Book_Infra;
using Polly;
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
            builder.Services.Configure<Book_Application.CacheConfig>(builder.Configuration.GetSection(nameof(Book_Application.CacheConfig)));
            builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection(nameof(RabbitConfig)));
            var config = builder.Configuration.GetSection(nameof(Book_Application.CacheConfig));
            var options = config.Get<Book_Application.CacheConfig>();
            Policy.Handle<RedisCommandException>()
                .WaitAndRetry(5, x => TimeSpan.FromSeconds(15))
                .Execute(() =>
                {
                    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.redis.url));
                }
                );
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<ConnectionFactory>();
            builder.Services.AddScoped<BookService>();
            builder.Services.AddScoped<BookController>();
            builder.Services.AddHostedService<RabbitBackgroundService.RabbitBackgroundService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "taghche-Cors_Specs",
                                  policy =>
                                  {
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyOrigin();
                                      policy.AllowAnyMethod();

                                  });
            });

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
            
            //app.UseMiddleware<StreamBuilderMiddleware>();
            //app.UseMiddleware<InMemoryCacheMiddleware>();
            //app.UseMiddleware<RedisMiddleware>();

            app.Run();
        }
    }
}
