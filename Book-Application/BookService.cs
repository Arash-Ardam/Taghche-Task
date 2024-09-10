using Book_Infra;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Book_Application
{
    public class BookService
    {
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _httpclient;

        public BookService(IHttpClientFactory httpClientFactory)
        {
            _factory = httpClientFactory;
            _httpclient = _factory.CreateClient("Book-Client");
            
        }



        public async Task<BookDto> GetBookFromApiAsync(short id)
        {
            try
            {
                string route = $"{_httpclient.BaseAddress}/{id}";

                var response = await _httpclient.GetAsync(route);

                //var content = await response.Content.ReadAsStringAsync();
                var content = await response.Content.ReadFromJsonAsync<BookDto>();



                return content;
            }
            catch (Exception)
            {

                throw;
            }
        }







    }
}
