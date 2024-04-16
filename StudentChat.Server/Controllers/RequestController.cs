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
public class RequestController : ControllerBase
{
    private readonly ChatServerDbContext _dbContext;
    private readonly NotifyService _notifyService;

    public RequestController(
        ChatServerDbContext dbContext,
        NotifyService notifyService)
    {
        _dbContext = dbContext;
        _notifyService = notifyService;
    }

    [HttpPost("GetSentFriendRequests")]
    public async Task<ApiResult<GetSentFriendRequestsResultData>> GetSentFriendRequestsAsync(QueryRequestData request)
    {
        if (request.Count == 0)
        {
            request = request with
            {
                Count = 20
            };
        }

        var selfUserId = HttpContext.GetUserId();
        var requests = await _dbContext.FriendRequests
            .Where(request => request.SenderId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(request.Skip)
            .Take(request.Count)
            .Include(request => request.Sender)
            .Include(request => request.Receiver)
            .Select(request => (CommonModels.FriendRequest)request)
            .ToArrayAsync();

        return ApiResult<GetSentFriendRequestsResultData>.CreateOk(new GetSentFriendRequestsResultData(requests));
    }

    [HttpPost("GetSentGroupRequests")]
    public async Task<ApiResult<GetSentGroupRequestsResultData>> GetSentGroupRequestsAsync(QueryRequestData request)
    {
        if (request.Count == 0)
        {
            request = request with
            {
                Count = 20
            };
        }

        var selfUserId = HttpContext.GetUserId();
        var requests = await _dbContext.GroupRequests
            .Where(request => request.SenderId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(request.Skip)
            .Take(request.Count)
            .Include(request => request.Sender)
            .Include(request => request.Group)
            .Select(request => (CommonModels.GroupRequest)request)
            .ToArrayAsync();

        return ApiResult<GetSentGroupRequestsResultData>.CreateOk(new GetSentGroupRequestsResultData(requests));
    }

    [HttpPost("GetReceivedFriendRequests")]
    public async Task<ApiResult<GetReceivedFriendRequestsResultData>> GetReceivedFriendRequestsAsync(QueryRequestData request)
    {
        if (request.Count == 0)
        {
            request = request with
            {
                Count = 20
            };
        }

        var selfUserId = HttpContext.GetUserId();

        var requests = await _dbContext.FriendRequests
            .Where(request => request.ReceiverId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(request.Skip)
            .Take(request.Count)
            .Include(request => request.Sender)
            .Include(request => request.Receiver)
            .Select(request => (CommonModels.FriendRequest)request)
            .ToArrayAsync();

        return ApiResult<GetReceivedFriendRequestsResultData>.CreateOk(new GetReceivedFriendRequestsResultData(requests));
    }

    [HttpPost("GetReceivedGroupRequests")]
    public async Task<ApiResult<GetReceivedGroupRequestsResultData>> GetReceivedGroupRequestsAsync(QueryRequestData request)
    {
        if (request.Count == 0)
        {
            request = request with
            {
                Count = 20
            };
        }

        var selfUserId = HttpContext.GetUserId();
        var requests = await _dbContext.GroupRequests
            .Where(request => request.Group.OwnerId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(request.Skip)
            .Take(request.Count)
            .Include(request => request.Sender)
            .Include(request => request.Group)
            .Select(request => (CommonModels.GroupRequest)request)
            .ToArrayAsync();

        return ApiResult<GetReceivedGroupRequestsResultData>.CreateOk(new GetReceivedGroupRequestsResultData(requests));
    }

    [HttpPost("GetFriendRequests")]
    public async Task<ApiResult<GetFriendRequestsResultData>> GetFriendRequests(QueryRequestData request)
    {
        if (request.Count == 0)
        {
            request = request with
            {
                Count = 20
            };
        }

        var selfUserId = HttpContext.GetUserId();
        var requests = await _dbContext.FriendRequests
            .Where(request => request.SenderId == selfUserId || request.ReceiverId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(request.Skip)
            .Take(request.Count)
            .Include(request => request.Sender)
            .Include(request => request.Receiver)
            .Select(request => (CommonModels.FriendRequest) request)
            .ToArrayAsync();

        return ApiResult<GetFriendRequestsResultData>.CreateOk(new GetFriendRequestsResultData(requests));
    }

    [HttpPost("GetGroupRequests")]
    public async Task<ApiResult<GetGroupRequestsResultData>> GetGroupRequests(QueryRequestData request)
    {
        var count = request.Count;
        if (count <= 0)
            count = 20;

        var skip = Math.Max(0, request.Skip);

        var selfUserId = HttpContext.GetUserId();
        var requests = await _dbContext.GroupRequests
            .Where(request => request.SenderId == selfUserId || request.Group.OwnerId == selfUserId)
            .OrderByDescending(request => request.SentTime)
            .Skip(skip)
            .Take(count)
            .Include(request => request.Sender)
            .Include(request => request.Group)
            .Select(request => (CommonModels.GroupRequest)request)
            .ToArrayAsync();

        return ApiResult<GetGroupRequestsResultData>.CreateOk(new GetGroupRequestsResultData(requests));
    }


    [HttpPost("SendFriendRequest")]
    public async Task<ApiResult> SendFriendRequestAsync(SendFriendRequestRequestData request)
    {
        var selfUserId = HttpContext.GetUserId();

        if (selfUserId == request.UserId)
        {
            return ApiResult.CreateErr("无法添加自己为好友");
        }

        var friendAlreadyExist = 
            await _dbContext.UserFriends.AnyAsync(r => (r.FromUserId == selfUserId && r.ToUserId == request.UserId) || (r.FromUserId == request.UserId && r.ToUserId == selfUserId));

        if (friendAlreadyExist)
        {
            return ApiResult.CreateErr("已经是好友了");
        }

        var requestAlreadyExist = 
            await _dbContext.FriendRequests.AnyAsync(r => r.SenderId == selfUserId && r.ReceiverId == request.UserId && !r.IsDone);
        if (requestAlreadyExist)
        {
            return ApiResult.CreateErr("已存在一个好友请求");
        }

        var userExist = await _dbContext.Users
            .AnyAsync(user => user.Id == request.UserId);

        if (!userExist)
        {
            return ApiResult.CreateErr("用户不存在");
        }

        var entry = _dbContext.FriendRequests.Add(
            new FriendRequest()
            {
                SenderId = selfUserId,
                ReceiverId = request.UserId,
                Message = request.Message,
                SentTime = DateTimeOffset.Now,
            });

        await _dbContext.SaveChangesAsync();

        await entry.Entity.LoadAllPropertiesAsync(_dbContext);
        await _notifyService.OnFriendRequestSent((CommonModels.FriendRequest)entry.Entity);

        return ApiResult.CreateOk();
    }

    [HttpPost("SendGroupRequest")]
    public async Task<ApiResult> SendGroupRequestAsync(SendGroupRequestRequestData request)
    {
        var selfUserId = HttpContext.GetUserId();
        var groupAlreadyExist =
            await _dbContext.Groups.AnyAsync(g => g.OwnerId == selfUserId && g.Id == request.GroupId) ||
            await _dbContext.GroupMembers.AnyAsync(gm => gm.UserId == selfUserId && gm.GroupId == request.GroupId);

        if (groupAlreadyExist)
        {
            return ApiResult.CreateErr("已经是群聊成员");
        }

        var requestAlreadyExist = await _dbContext.GroupRequests.AnyAsync(r => r.SenderId == selfUserId && r.GroupId == request.GroupId && !r.IsDone);

        if (requestAlreadyExist)
        {
            return ApiResult.CreateErr("已存在一个请求");
        }

        var groupExist = await _dbContext.Groups
            .AnyAsync(group => group.Id == request.GroupId);

        if (!groupExist)
        {
            return ApiResult.CreateErr("群不存在");
        }

        var entry = _dbContext.GroupRequests.Add(
            new GroupRequest()
            {
                SenderId = selfUserId,
                GroupId = request.GroupId,
                Message = request.Message,
                SentTime = DateTimeOffset.Now,
            });

        await _dbContext.SaveChangesAsync();

        await entry.Entity.LoadAllPropertiesAsync(_dbContext);
        await _notifyService.OnGroupRequestSent((CommonModels.GroupRequest)entry.Entity);

        return ApiResult.CreateOk();
    }


    [HttpPost("AcceptFriendRequest")]
    public async Task<ApiResult> AcceptFriendRequestAsync(AcceptRequestRequestData request)
    {
        var selfId = HttpContext.GetUserId();
        var friendRequest = await _dbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.ReceiverId == selfId && fr.Id == request.RequestId);

        if (friendRequest is null)
        {
            return ApiResult.CreateErr("没有这样的好友请求");
        }

        if (friendRequest.IsDone)
        {
            return ApiResult.CreateErr("这个请求已处理");
        }

        friendRequest.IsDone = true;

        await _dbContext.UserFriends.AddAsync(
            new UserFriend()
            {
                FromUserId = friendRequest.SenderId,
                ToUserId = selfId,
            });

        await _dbContext.SaveChangesAsync();

        var requestSender = _dbContext.Users.First(u => u.Id == friendRequest.SenderId);
        var requestReceiver = _dbContext.Users.First(u => u.Id == friendRequest.ReceiverId);
        await _notifyService.OnFriendIncreased(requestSender.Id, (CommonModels.User)requestReceiver);
        await _notifyService.OnFriendIncreased(requestReceiver.Id, (CommonModels.User)requestSender);

        return ApiResult.CreateOk();
    }

    [HttpPost("RejectFriendRequest")]
    public async Task<ApiResult> RejectFriendRequestAsync(AcceptRequestRequestData request)
    {
        var selfId = HttpContext.GetUserId();
        var friendRequest = await _dbContext.FriendRequests
            .FirstOrDefaultAsync(fr => fr.ReceiverId == selfId && fr.Id == request.RequestId);

        if (friendRequest is null)
        {
            return ApiResult.CreateErr("没有这样的好友请求");
        }

        if (friendRequest.IsDone)
        {
            return ApiResult.CreateErr("请求已经处理");
        }

        friendRequest.IsDone = true;

        //await _dbContext.UserFriends.AddAsync(
        //    new UserFriend()
        //    {
        //        FromUserId = friendRequest.SenderId,
        //        ToUserId = selfId,
        //    });

        await _dbContext.SaveChangesAsync();

        //var requestSender = _dbContext.Users.First(u => u.Id == friendRequest.SenderId);
        //var requestReceiver = _dbContext.Users.First(u => u.Id == friendRequest.ReceiverId);
        //await _notifyService.OnFriendIncreased(requestSender.Id, (CommonModels.User)requestReceiver);
        //await _notifyService.OnFriendIncreased(requestReceiver.Id, (CommonModels.User)requestSender);

        return ApiResult.CreateOk();
    }

    [HttpPost("AcceptGroupRequest")]
    public async Task<ApiResult> AcceptGroupRequestAsync(AcceptRequestRequestData request)
    {
        var selfId = HttpContext.GetUserId();
        var groupRequest = await _dbContext.GroupRequests
            .FirstOrDefaultAsync(gr => gr.Id == request.RequestId);

        if (groupRequest is null)
        {
            return ApiResult.CreateErr("没有这样的群请求");
        }

        if (groupRequest.IsDone)
        {
            return ApiResult.CreateErr("群请求已处理");
        }

        var group = await _dbContext.Groups
            .AsNoTracking()
            .FirstAsync(g => g.Id == groupRequest.GroupId);

        if (group.OwnerId != selfId)
        {
            return ApiResult.CreateErr("不是群的所有者");
        }

        groupRequest.IsDone = true;

        await _dbContext.GroupMembers.AddAsync(
            new GroupMember()
            {
                UserId = groupRequest.SenderId,
                GroupId = groupRequest.GroupId,
            });

        await _dbContext.SaveChangesAsync();
        await _notifyService.OnGroupIncreased(groupRequest.SenderId, (CommonModels.Group)group);

        return ApiResult.CreateOk();
    }

    [HttpPost("RejectGroupRequest")]
    public async Task<ApiResult> RejectGroupRequestAsync(AcceptRequestRequestData request)
    {
        var selfId = HttpContext.GetUserId();
        var groupRequest = await _dbContext.GroupRequests
            .FirstOrDefaultAsync(gr => gr.Id == request.RequestId);

        if (groupRequest is null)
        {
            return ApiResult.CreateErr("没有这样的群请求");
        }

        if (groupRequest.IsDone)
        {
            return ApiResult.CreateErr("群请求已处理");
        }

        var group = await _dbContext.Groups
            .AsNoTracking()
            .FirstAsync(g => g.Id == groupRequest.GroupId);

        if (group.OwnerId != selfId)
        {
            return ApiResult.CreateErr("不是群的所有者");
        }

        groupRequest.IsDone = true;

        //await _dbContext.GroupMembers.AddAsync(
        //    new GroupMember()
        //    {
        //        UserId = groupRequest.SenderId,
        //        GroupId = groupRequest.GroupId,
        //    });

        await _dbContext.SaveChangesAsync();
        //await _notifyService.OnGroupIncreased(groupRequest.SenderId, (CommonModels.Group)group);

        return ApiResult.CreateOk();
    }
}
