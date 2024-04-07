using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Models;
using StudentChat.Server.Models.Database;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ManageController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;

        public ManageController(ChatServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("AddUser")]
        public async Task<ApiResult<AddUserResultData>> AddUser(AddUserRequestData request)
        {
            var alreadyExist = await _dbContext.Users.AnyAsync(user => user.UserName == request.User.UserName);
            if (alreadyExist)
            {
                return ApiResult<AddUserResultData>.CreateErr("已经有相同用户名的用户了");
            }

            var entry = await _dbContext.Users.AddAsync(
                new Models.Database.User()
                {
                    UserName = request.User.UserName,
                    PasswordHash = request.PasswordHash
                });

            return ApiResult<AddUserResultData>.CreateOk(
                new AddUserResultData((CommonModels.User)entry.Entity));
        }
    }
}
