namespace Book_Api.Middlewares
{
    public class StreamBuilderMiddleware
    {
        private readonly RequestDelegate _next;

        public StreamBuilderMiddleware(RequestDelegate next)
        {
            _next = next;        
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }

            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
