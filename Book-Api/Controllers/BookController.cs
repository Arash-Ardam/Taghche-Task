using Book_Application;
using Book_Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Book_Api.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private BookService _service;

        public BookController(BookService service)
        {
            _service = service;
        }


        [HttpGet("{id}")]
        public async Task<BookDto> GetBookAsync(short id)
        {
            var response = await _service.GetBookFromApiAsync(id);
            return response;
        }
    }
}
