using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Extensions;
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

        [HttpPost("GetSelf")]
        public Task<ApiResult<GetUserResultData>> GetSelfAsync()
        {
            var selfId = HttpContext.GetUserId();

            return GetUserAsync(new GetUserRequestData(selfId));
        }


        [HttpPost("GetUser")]
        public async Task<ApiResult<GetUserResultData>> GetUserAsync(GetUserRequestData request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync();

            if (user is null)
            {
                return ApiResult<GetUserResultData>.CreateErr("Invalid user id");
            }

            return ApiResult<GetUserResultData>.CreateOk(new GetUserResultData((CommonModels.User)user));
        }

        [HttpPost("GetGroup")]
        public async Task<ApiResult<GetGroupResultData>> GetGroupAsync(GetGroupRequestData request)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync();

            if (group is null)
            {
                return ApiResult<GetGroupResultData>.CreateErr("Invalid group id");
            }

            return ApiResult<GetGroupResultData>.CreateOk(new GetGroupResultData((CommonModels.Group)group));
        }

        [HttpPost("SearchUser")]
        public async Task<ApiResult<SearchUserResultData>> SearchUser(SearchUserRequestData request)
        {
            var users = await _dbContext.Users
                .Where(user => user.UserName.Contains(request.Keyword) || user.Nickname.Contains(request.Keyword))
                .Skip(request.Skip)
                .Take(request.Count)
                .Select(user => (CommonModels.User)user)
                .ToListAsync();

            return ApiResult<SearchUserResultData>.CreateOk(new SearchUserResultData(users));
        }

        [HttpPost("SearchGroup")]
        public async Task<ApiResult<SearchGroupResultData>> SearchGroup(SearchGroupRequestData request)
        {
            var groups = await _dbContext.Groups
                .Where(group => group.Name.Contains(request.Keyword))
                .Skip(request.Skip)
                .Take(request.Count)
                .Select(group => (CommonModels.Group)group)
                .ToListAsync();

            return ApiResult<SearchGroupResultData>.CreateOk(new SearchGroupResultData(groups));
        }
    }
}
