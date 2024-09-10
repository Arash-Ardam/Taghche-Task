
using Book_Api.Controllers;
using Book_Application;

namespace Book_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddHttpClient("Book-Client", httpclient =>  httpclient.BaseAddress = new Uri(builder.Configuration.GetSection("Book-Api:api-address").Value));
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

            app.Run();
        }
    }
}
