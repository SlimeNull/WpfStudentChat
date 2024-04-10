using System.Text.RegularExpressions;
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

        [HttpPost("GetUsers")]
        public async Task<ApiResult<GetUsersResultData>> GetUsers(GetUsersRequestData request)
        {
            int count = request.Count;
            if (count < 0)
                count = 1;

            int skip = request.Skip;
            if (skip < 0)
                skip = 0;

            var queryable = _dbContext.Users.AsQueryable();
            if(!string.IsNullOrEmpty(request.NicknameKeyword))
                queryable = queryable.Where(user => user.Nickname.Contains(request.NicknameKeyword));
            if(!string.IsNullOrEmpty(request.UserNameKeyword))
                queryable = queryable.Where(user => user.UserName.Contains(request.UserNameKeyword));

            var totalCount = await queryable.CountAsync();

            var users = await queryable
                .Skip(skip)
                .Take(count)
                .Select(user => (CommonModels.User)user)
                .ToArrayAsync();

            return ApiResult<GetUsersResultData>.CreateOk(new(totalCount, users));
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
