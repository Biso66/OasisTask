using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oasis.Helpers;
using System.Text.Json;

namespace Oasis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveToDoController : ControllerBase
    {
        private readonly HttpClient httpClient = new HttpClient();

        [HttpGet("{page}")]
        public async Task<IActionResult> LiveToDo(int page)
        {

            using HttpResponseMessage response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/todos");
            var jsonResponse = await response.Content.ReadAsStreamAsync();
            var todos = await JsonSerializer.DeserializeAsync<List<object>>(jsonResponse);
            int itemsPerPage = 10; 
            int pageCount = (int)Math.Ceiling((double)todos.Count() / itemsPerPage);
            var tasks = todos.Skip( (page -1) * itemsPerPage).Take(itemsPerPage).ToList();
            var responce = new Pagination
            {
                Tasks = tasks,
                CurrentPage = page,
                Pages = pageCount,
            };
            return Ok(responce);
        }
    }
}
