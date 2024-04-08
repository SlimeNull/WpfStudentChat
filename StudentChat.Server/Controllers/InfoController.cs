using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Extensions;
using StudentChat.Server.Models.Database;
using StudentChat.Server.Services;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly NotifyService _notifyService;

        public InfoController(
            ChatServerDbContext dbContext,
            NotifyService notifyService)
        {
            _dbContext = dbContext;
            _notifyService = notifyService;
        }

        [HttpPost("GetSelf")]
        public Task<ApiResult<GetUserResultData>> GetSelfAsync()
        {
            var selfId = HttpContext.GetUserId();

            return GetUserAsync(new GetUserRequestData(selfId));
        }

        [HttpPost("SetSelf")]
        public async Task<ApiResult> SetSelfAsync(SetUserRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var user = await _dbContext.Users.FirstAsync(user => user.Id == selfUserId);
            var newProfile = request.User;

            if (!string.IsNullOrWhiteSpace(newProfile.AvatarHash))
            {
                user.AvatarHash = newProfile.AvatarHash;
            }

            if (!string.IsNullOrWhiteSpace(newProfile.UserName))
            {
                user.UserName = newProfile.UserName;
            }

            if (!string.IsNullOrWhiteSpace(newProfile.Nickname))
            {
                user.Nickname = newProfile.Nickname;
            }

            if (!string.IsNullOrWhiteSpace(newProfile.Bio))
            {
                user.Bio = newProfile.Bio;
            }

            await _dbContext.SaveChangesAsync();

            return ApiResult.CreateOk();
        }

        [HttpPost("SetSelfPassword")]
        public async Task<ApiResult> SetSelfPasswordAsync(SetPasswordRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var user = await _dbContext.Users.FirstAsync(user => user.Id == selfUserId);

            if (string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                return ApiResult.CreateErr("Invalid password hash");
            }

            user.PasswordHash = request.PasswordHash;

            await _dbContext.SaveChangesAsync();

            return ApiResult.CreateOk();
        }

        [HttpPost("GetUser")]
        public async Task<ApiResult<GetUserResultData>> GetUserAsync(GetUserRequestData request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId);

            if (user is null)
            {
                return ApiResult<GetUserResultData>.CreateErr("Invalid user id");
            }

            return ApiResult<GetUserResultData>.CreateOk(new GetUserResultData((CommonModels.User)user));
        }

        [HttpPost("GetGroup")]
        public async Task<ApiResult<GetGroupResultData>> GetGroupAsync(GetGroupRequestData request)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == request.GroupId);

            if (group is null)
            {
                return ApiResult<GetGroupResultData>.CreateErr("Invalid group id");
            }

            return ApiResult<GetGroupResultData>.CreateOk(new GetGroupResultData((CommonModels.Group)group));
        }

        [HttpPost("SetGroup")]
        public async Task<ApiResult> SetGroupAsync(SetGroupRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var group = await _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == request.Group.Id);

            if (group is null)
            {
                return ApiResult.CreateErr("Group not found");
            }

            if (group.OwnerId != selfUserId)
            {
                return ApiResult.CreateErr("You are not owner of that group");
            }

            var newProfile = request.Group;

            if (string.IsNullOrWhiteSpace(newProfile.Name))
            {
                group.Name = newProfile.Name;
            }

            if (string.IsNullOrWhiteSpace(newProfile.Description))
            {
                group.Description = newProfile.Description;
            }

            if (string.IsNullOrWhiteSpace(newProfile.AvatarHash))
            {
                group.AvatarHash = newProfile.AvatarHash;
            }

            await _dbContext.SaveChangesAsync();

            return ApiResult.CreateOk();
        }

        [HttpPost("CreateGroup")]
        public async Task<ApiResult<CreateGroupResultData>> CreateGroupAsync(CreateGroupRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var entry = await _dbContext.Groups.AddAsync(
                new Group()
                {
                    Name = request.Group.Name,
                    Description = request.Group.Description,
                    AvatarHash = request.Group.AvatarHash,
                });

            await _dbContext.SaveChangesAsync();

            var commonGroup = (CommonModels.Group)entry.Entity;
            await _notifyService.OnGroupIncreased(selfUserId, commonGroup);

            return ApiResult<CreateGroupResultData>.CreateOk(new CreateGroupResultData(commonGroup));
        }

        [HttpPost("DeleteGroup")]
        public async Task<ApiResult> DeleteGroupAsync(DeleteGroupRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var group = await _dbContext.Groups
                .Include(group => group.Members)
                .FirstOrDefaultAsync(group => group.Id == request.GroupId);

            if (group is null)
            {
                return ApiResult.CreateErr("Group not found");
            }

            if (group.OwnerId != selfUserId)
            {
                return ApiResult.CreateErr("You are not owner of that group");
            }

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            var commonGroup = (CommonModels.Group)group;
            await _notifyService.OnGroupDecreased(selfUserId, commonGroup);

            foreach (var member in group.Members)
            {
                await _notifyService.OnGroupDecreased(member.Id, commonGroup);
            }

            return ApiResult.CreateOk();
        }

        [HttpPost("SearchUser")]
        public async Task<ApiResult<SearchUserResultData>> SearchUser(SearchUserRequestData request)
        {
            if (request.Count == 0)
            {
                request = request with
                {
                    Count = 10
                };
            }

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
            if (request.Count == 0)
            {
                request = request with
                {
                    Count = 10
                };
            }

            var groups = await _dbContext.Groups
                .Where(group => group.Name.Contains(request.Keyword))
                .Skip(request.Skip)
                .Take(request.Count)
                .Select(group => (CommonModels.Group)group)
                .ToListAsync();

            return ApiResult<SearchGroupResultData>.CreateOk(new SearchGroupResultData(groups));
        }

        [HttpPost("GetFriends")]
        public async Task<ApiResult<GetFriendsResultData>> GetFriends()
        {
            var selfUserId = HttpContext.GetUserId();
            var selfUser = await _dbContext.Users
                .Include(user => user.AddedFriends)
                .Include(user => user.AcceptedFriends)
                .FirstAsync(user => user.Id == selfUserId);

            var friends = selfUser.AddedFriends
                .Concat(selfUser.AcceptedFriends)
                .Select(user => (CommonModels.User)user)
                .ToList();

            return ApiResult<GetFriendsResultData>.CreateOk(new GetFriendsResultData(friends));
        }

        [HttpPost("GetGroups")]
        public async Task<ApiResult<GetGroupsResultData>> GetGroups()
        {
            var selfUserId = HttpContext.GetUserId();
            var selfUser = await _dbContext.Users
                .Include(user => user.OwnedGroups)
                .Include(user => user.JoindGroups)
                .FirstAsync(user => user.Id == selfUserId);

            var groups = selfUser.OwnedGroups
                .Concat(selfUser.JoindGroups)
                .Select(group => (CommonModels.Group)group)
                .ToList();

            return ApiResult<GetGroupsResultData>.CreateOk(new GetGroupsResultData(groups));
        }
    }
}
