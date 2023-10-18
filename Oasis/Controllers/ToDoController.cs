using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Oasis.DTOS;
using Oasis.Models;
using Oasis.Services;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Oasis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly WorkDbContext _workContext;
        private readonly IGetUser _getUser;
        private readonly IMapper _mapper;
        public ToDoController( WorkDbContext workContext, IGetUser getUser, IMapper mapper)
        {
            _workContext = workContext;
            _getUser = getUser;
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] string title)
        {
            var x = "sdas";
            x.ToLower();
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var useremail = User?.FindFirst("UserEmail")?.Value;
                var parameter = headerValue.Parameter;
                var UserId = await _getUser.GetUserByToken(parameter);
                var tasks = _workContext.Works.Where(ww => ww.UserId == UserId);
                var work = new Work
                {
                    Title = title,
                    UserId = UserId
                };
                await _workContext.Works.AddAsync(work);
                await _workContext.SaveChangesAsync();
                return Ok(title + " Created");
            }
            return BadRequest("You have problem in ....");
        }
        [HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAll()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var parameter = headerValue.Parameter;
                var UserId = await _getUser.GetUserByToken(parameter);
                var tasks = _workContext.Works.Where(ww => ww.UserId == UserId);
                var mapping = _mapper.Map<IEnumerable<WorkDto>>(tasks);
                return Ok(mapping);
            }
            return BadRequest("You have problem in ....");

        }
        [HttpGet("GetOneTask/{id}")] 
        public async Task<IActionResult> GetOne(int id)
        {
            var task = await _workContext.Works.FirstOrDefaultAsync(w => w.Id == id);
            var mapping = _mapper.Map<WorkDto>(task);
            return Ok(mapping);
        }
        [HttpPut("EditOneTask/{id}")]
        public async Task<IActionResult> EditTask([FromBody]string title ,int id)
        {
            var task = await _workContext.Works.FirstOrDefaultAsync(w => w.Id == id);
            if (task != null)
            {
                task.Title = title;
                _workContext.Works.Update(task);
                await _workContext.SaveChangesAsync();
                return Ok(title+ " is Updated");
            }
            return BadRequest("You don`t have this task");
        }
        [HttpPut("CompleteOneTask/{id}")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _workContext.Works.FirstOrDefaultAsync(w => w.Id == id);
            if (task != null)
            {
                task.Completed = true;
                _workContext.Works.Update(task);
                await _workContext.SaveChangesAsync();
                return Ok("You Completed this task"+ task.Title);
            }
            return BadRequest("You don`t have this task");
        }
        [HttpPut("CompleteTaskes")]
        public async Task<IActionResult> CompleteTask()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var parameter = headerValue.Parameter;
                var UserId = await _getUser.GetUserByToken(parameter);
                var Tasks = _workContext.Works.Where(ww => ww.UserId == UserId);
                foreach (var task in Tasks)
                {
                    task.Completed = true;

                }
                _workContext.Works.UpdateRange(Tasks);
                await _workContext.SaveChangesAsync();
                var mapping = _mapper.Map<IEnumerable<WorkDto>>(Tasks);

                return Ok(mapping);
            }
            return BadRequest("We have some issues");
        }
        [HttpDelete("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _workContext.Works.FirstOrDefaultAsync(w => w.Id == id);
            if (task != null)
            {
                _workContext.Works.Remove(task);
                await _workContext.SaveChangesAsync();
                return Ok("You Deleted this task " + task.Title);
            }
            return BadRequest("You don`t have this task");
        }
    }
}
