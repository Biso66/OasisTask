using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Oasis.DTOS;
using Oasis.Models;
using Oasis.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Oasis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] string title, string token)
        {
            if (token != null)
            {
                var UserId = await _getUser.GetUserByToken(token);
                var work = new Work
                {
                    Title = title,
                    UserId = UserId
                };
                await _workContext.Works.AddAsync(work);
                await _workContext.SaveChangesAsync();
                return Ok(title + " Created");

                
            }
            return BadRequest();
        }
        [HttpGet("GetAllTasks")]
        [Authorize]
        public async Task<IActionResult> GetAll(string token)
        {
            if(token != null) 
            {
                var UserId = await _getUser.GetUserByToken(token);
                var tasks = _workContext.Works.Where(ww => ww.UserId == UserId);
                var mapping = _mapper.Map<IEnumerable<WorkDto>>(tasks);
                return Ok(mapping);
            }
            
            return BadRequest("your token is uncorrect ");
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
