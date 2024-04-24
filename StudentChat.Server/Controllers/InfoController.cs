using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Extensions;
using StudentChat.Server.Models.Database;
using StudentChat.Server.Services;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers;

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
        var existAnyUserWithSameUserName = await _dbContext.Users.AnyAsync(user => user.UserName == newProfile.UserName && user.Id != selfUserId);

        if (existAnyUserWithSameUserName)
        {
            return ApiResult.CreateErr("已经有一个用户具有相同的用户名");
        }

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

        if (string.IsNullOrWhiteSpace(request.OldPasswordHash))
            return ApiResult.CreateErr("密码哈希值无效");

        if (string.IsNullOrWhiteSpace(request.NewPasswordHash))
            return ApiResult.CreateErr("密码哈希值无效");

        if(request.OldPasswordHash != user.PasswordHash)
            return ApiResult.CreateErr("旧密码不正确");

        user.PasswordHash = request.NewPasswordHash;

        await _dbContext.SaveChangesAsync();

        return ApiResult.CreateOk();
    }

    [HttpPost("GetUser")]
    public async Task<ApiResult<GetUserResultData>> GetUserAsync(GetUserRequestData request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId);

        if (user is null)
        {
            return ApiResult<GetUserResultData>.CreateErr("用户 ID 无效");
        }

        return ApiResult<GetUserResultData>.CreateOk(new GetUserResultData((CommonModels.User)user));
    }

    [HttpPost("GetGroup")]
    public async Task<ApiResult<GetGroupResultData>> GetGroupAsync(GetGroupRequestData request)
    {
        var group = await _dbContext.Groups.FirstOrDefaultAsync(group => group.Id == request.GroupId);

        if (group is null)
        {
            return ApiResult<GetGroupResultData>.CreateErr($"{Consts.GroupName} ID 无效");
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
            return ApiResult.CreateErr($"找不到{Consts.GroupName}");
        }

        if (group.OwnerId != selfUserId)
        {
            return ApiResult.CreateErr($"您不是该{Consts.GroupName}的所有者");
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

        if (string.IsNullOrWhiteSpace(request.Group.Name))
        {
            return ApiResult<CreateGroupResultData>.CreateErr($"{Consts.GroupName}不能为空");
        }

        if (_dbContext.Groups.Any(g => g.Name == request.Group.Name))
        {
            return ApiResult<CreateGroupResultData>.CreateErr($"{Consts.GroupName}已被使用");
        }

        var entry = await _dbContext.Groups.AddAsync(
            new Group()
            {
                OwnerId = selfUserId,
                Name = request.Group.Name,
                Description = request.Group.Description,
                AvatarHash = request.Group.AvatarHash,
            });

        await _dbContext.SaveChangesAsync();

        await _dbContext.GroupMembers.AddAsync(
            new GroupMember()
            {
                GroupId = entry.Entity.Id,
                UserId = selfUserId,
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
            .FirstOrDefaultAsync(group => group.Id == request.GroupId);

        if (group is null)
        {
            return ApiResult.CreateErr($"{Consts.GroupName}未找到");
        }

        if (group.OwnerId == selfUserId)
        {
            _dbContext.Groups.Remove(group);
            await _dbContext.GroupMembers
                .Where(gm => gm.GroupId == group.Id)
                .ExecuteDeleteAsync();

            await _dbContext.SaveChangesAsync();

            var commonGroup = (CommonModels.Group)group;
            await _notifyService.OnGroupDecreased(selfUserId, commonGroup);

            await _dbContext.Entry(group)
                .Collection(e => e.Members)
                .LoadAsync();

            foreach (var member in group.Members)
            {
                await _notifyService.OnGroupDecreased(member.Id, commonGroup);
            }
        }
        else
        {
            var member = _dbContext.GroupMembers.FirstOrDefault(m => m.GroupId == group.Id && m.UserId == selfUserId);

            if (member is null)
            {
                return ApiResult.CreateErr($"你不是该{Consts.GroupName}的成员");
            }

            _dbContext.GroupMembers.Remove(member);
            await _dbContext.SaveChangesAsync();

            var commonGroup = (CommonModels.Group)group;
            await _notifyService.OnGroupDecreased(selfUserId, commonGroup);
        }

        return ApiResult.CreateOk();
    }

    [HttpPost("DeleteFriend")]
    public async Task<ApiResult> DeleteFriendAsync(DeleteFriendRequestData request)
    {
        var selfUserId = HttpContext.GetUserId();
        var friend = await _dbContext.UserFriends
            .FirstOrDefaultAsync(f => (f.FromUserId == selfUserId && f.ToUserId == request.UserId) || (f.FromUserId == request.UserId && f.ToUserId == selfUserId));

        if (friend is null)
        {
            return ApiResult.CreateErr("没有这个好友");
        }

        var otherUserId = friend.FromUserId == selfUserId ? friend.ToUserId : friend.FromUserId;
        var self = await _dbContext.Users.FirstAsync(u => u.Id == selfUserId);
        var other = await _dbContext.Users.FirstAsync(u => u.Id == otherUserId);

        _dbContext.UserFriends.Remove(friend);
        await _dbContext.SaveChangesAsync();

        await _notifyService.OnFriendDecreased(selfUserId, (CommonModels.User)other);
        await _notifyService.OnFriendDecreased(otherUserId, (CommonModels.User)self);

        return ApiResult.CreateOk();
    }

    [HttpPost("SearchUser")]
    public async Task<ApiResult<SearchUserResultData>> SearchUser(KeywordQueryRequestData request)
    {
        var count = request.Count;
        if (count <= 0)
            count = 10;

        var skip = Math.Max(0, request.Skip);

        var users = await _dbContext.Users
            .Where(user => user.UserName.Contains(request.Keyword) || user.Nickname.Contains(request.Keyword))
            .Skip(skip)
            .Take(count)
            .Select(user => (CommonModels.User)user)
            .ToArrayAsync();

        return ApiResult<SearchUserResultData>.CreateOk(new SearchUserResultData(users));
    }

    [HttpPost("SearchGroup")]
    public async Task<ApiResult<SearchGroupResultData>> SearchGroup(KeywordQueryRequestData request)
    {
        int count = request.Count;
        if (count == 0)
            count = 10;

        int skip = Math.Max(0, request.Skip);

        var groups = await _dbContext.Groups
            .Where(group => group.Name.Contains(request.Keyword))
            .Skip(skip)
            .Take(count)
            .Select(group => (CommonModels.Group)group)
            .ToArrayAsync();

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
            .ToArray();

        return ApiResult<GetFriendsResultData>.CreateOk(new GetFriendsResultData(friends));
    }

    [HttpPost("GetGroups")]
    public async Task<ApiResult<GetGroupsResultData>> GetGroups()
    {
        var selfUserId = HttpContext.GetUserId();
        var selfUser = await _dbContext.Users
            //.Include(user => user.OwnedGroups)
            .Include(user => user.JoindGroups)
            .FirstAsync(user => user.Id == selfUserId);

        var groups = selfUser.JoindGroups
            //.Concat(selfUser.OwnedGroups)
            .Select(group => (CommonModels.Group)group)
            .ToArray();

        return ApiResult<GetGroupsResultData>.CreateOk(new GetGroupsResultData(groups));
    }

    [HttpPost("GetFriendMessageLastTime")]
    public async Task<ApiResult<GetFriendMessageLastTimeResultData>> GetFriendMessageLastTime(GetFriendMessageLastTimeRequestData request)
    {
        var userId = HttpContext.GetUserId();

        var from = await _dbContext.UserFriends.FirstOrDefaultAsync(v => v.FromUserId == userId && v.ToUserId == request.FriendUserId);
        if (from is { })
        {
            return ApiResult<GetFriendMessageLastTimeResultData>.CreateOk(new(from.FromLastReadTime));
        }

        var to = await _dbContext.UserFriends.FirstOrDefaultAsync(v => v.FromUserId == request.FriendUserId && v.ToUserId == userId);
        if (to is { })
        {
            return ApiResult<GetFriendMessageLastTimeResultData>.CreateOk(new(to.ToLastReadTime));
        }

        return ApiResult<GetFriendMessageLastTimeResultData>.CreateErr("找不到好友关系");
    }

    [HttpPost("SetFriendMessageLastTime")]
    public async Task<ApiResult> SetFriendMessageLastTime(SetFriendMessageLastTimeRequestData request)
    {
        var userId = HttpContext.GetUserId();

        var from = await _dbContext.UserFriends.FirstOrDefaultAsync(v => v.FromUserId == userId && v.ToUserId == request.FriendUserId);
        if (from is { })
        {
            from.FromLastReadTime = request.DateTime;
            _dbContext.Update(from);
            await _dbContext.SaveChangesAsync();
            return ApiResult.CreateOk();
        }

        var to = await _dbContext.UserFriends.FirstOrDefaultAsync(v => v.FromUserId == request.FriendUserId && v.ToUserId == userId);
        if (to is { })
        {
            to.ToLastReadTime = request.DateTime;
            _dbContext.Update(to);
            await _dbContext.SaveChangesAsync();
            return ApiResult.CreateOk();
        }

        return ApiResult.CreateErr("找不到好友关系");
    }

    [HttpPost("GetGroupMessageLastTime")]
    public async Task<ApiResult<GetGroupMessageLastTimeResultData>> GetGroupMessageLastTime(GetGroupMessageLastTimeRequestData request)
    {
        var userId = HttpContext.GetUserId();

        var member = await _dbContext.GroupMembers.FirstOrDefaultAsync(v => v.UserId == userId && v.GroupId == request.GroupId);
        if (member is { })
        {
            return ApiResult<GetGroupMessageLastTimeResultData>.CreateOk(new(member.LastReadTime));
        }

        return ApiResult<GetGroupMessageLastTimeResultData>.CreateErr("找不到群组成员关系");
    }

    [HttpPost("SetGroupMessageLastTime")]
    public async Task<ApiResult> SetGroupMessageLastTime(SetGroupMessageLastTimeRequestData request)
    {
        var userId = HttpContext.GetUserId();

        var member = await _dbContext.GroupMembers.FirstOrDefaultAsync(v => v.UserId == userId && v.GroupId == request.GroupId);
        if (member is { })
        {
            member.LastReadTime = request.DateTime;
            _dbContext.Update(member);
            await _dbContext.SaveChangesAsync();
            return ApiResult.CreateOk();
        }

        return ApiResult.CreateErr("找不到群组成员关系");
    }


}
