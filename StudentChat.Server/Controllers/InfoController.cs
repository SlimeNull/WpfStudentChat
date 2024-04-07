using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentChat.Server.Models.Database;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;

        public InfoController(
            ChatServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("User/{id}")]
        public async Task<CommonModels.User> GetUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("Group/{id}")]
        public async Task<CommonModels.Group> GetGroupAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
