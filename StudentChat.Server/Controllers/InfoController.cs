using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [ProducesResponseType<CommonModels.User>(200)]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(v => v.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            return new JsonResult(new CommonModels.User()
            {
                Id = id,
                UserName = user.UserName,
                Nickname = user.Nickname,
                AvatarHash = user.AvatarHash,
                Bio = user.Bio
            });
        }

        [HttpGet("Group/{id}")]
        [ProducesResponseType<CommonModels.Group>(200)]
        public async Task<IActionResult> GetGroupAsync(int id)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(v => v.Id == id);
            if (group is null)
            {
                return NotFound();
            }

            return new JsonResult(new CommonModels.Group()
            {
                Id = id,
                Name = group.Name,
                AvatarHash = group.AvatarHash,
                Description = group.Description,
                OwnerId = group.OwnerId,
            });

        }
    }
}
