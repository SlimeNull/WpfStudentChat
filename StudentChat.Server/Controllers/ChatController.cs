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
public class ChatController : ControllerBase
{
    private readonly ChatServerDbContext _dbContext;
    private readonly NotifyService _messageNotifyService;



    public ChatController(
        ChatServerDbContext dbContext,
        NotifyService messageNotifyService)
    {
        _dbContext = dbContext;
        _messageNotifyService = messageNotifyService;
    }


    [HttpPost("QueryPrivateMessages")]
    public async Task<ApiResult<QueryPrivateMessagesResultData>> QueryPrivateMessagesAsync(QueryPrivateMessagesRequestData request)
    {
        var selfUserId = HttpContext.GetUserId();
        var query = _dbContext.PrivateMessages
            .Where(msg => (msg.SenderId == selfUserId && msg.ReceiverId == request.UserId) || (msg.SenderId == request.UserId && msg.ReceiverId == selfUserId));

        if (request.Count > 100)
        {
            return ApiResult<QueryPrivateMessagesResultData>.CreateErr("Invalid query, count is too large");
        }

        if (request.StartTime.HasValue)
        {
            query = query
                .Where(msg => msg.SentTime >= request.StartTime.Value);

            if (request.EndTime.HasValue)
            {
                query = query
                    .Where(msg => msg.SentTime <= request.EndTime.Value);
            }

            query = query.OrderBy(msg => msg.SentTime);

            if (request.Count > 0)
            {
                query = query.Take(request.Count);
            }
        }
        else
        {
            if (request.EndTime.HasValue)
            {
                query = query
                    .Where(msg => msg.SentTime <= request.EndTime.Value);
            }

            if (request.Count > 0)
            {
                query = query
                    .OrderByDescending(msg => msg.SentTime)
                    .Take(request.Count)
                    .Reverse();
            }
            else
            {
                query = query
                    .OrderBy(msg => msg.SentTime);
            }
        }

        var messages = await query
            .Include(msg => msg.ImageAttachments)
            .Include(msg => msg.FileAttachments)
            .Select(msg => (CommonModels.PrivateMessage)msg)
            .ToArrayAsync();

        return ApiResult<QueryPrivateMessagesResultData>.CreateOk(new QueryPrivateMessagesResultData(messages));
    }


    [HttpPost("QueryGroupMessages")]
    public async Task<ApiResult<QueryGroupMessagesResultData>> QueryGroupMessagesAsync(QueryGroupMessagesRequestData request)
    {
        var selfUserId = HttpContext.GetUserId();
        var query = _dbContext.GroupMessages
            .Where(msg => msg.GroupId == request.GroupId);

        if (request.Count > 100)
        {
            return ApiResult<QueryGroupMessagesResultData>.CreateErr("无效查询, 数量太大");
        }

        if (request.StartTime.HasValue)
        {
            query = query
                .Where(msg => msg.SentTime >= request.StartTime.Value);

            if (request.EndTime.HasValue)
            {
                query = query
                    .Where(msg => msg.SentTime <= request.EndTime.Value);
            }

            query = query.OrderBy(msg => msg.SentTime);

            if (request.Count > 0)
            {
                query = query.Take(request.Count);
            }
        }
        else
        {
            if (request.EndTime.HasValue)
            {
                query = query
                    .Where(msg => msg.SentTime <= request.EndTime.Value);
            }

            if (request.Count > 0)
            {
                query = query
                    .OrderByDescending(msg => msg.SentTime)
                    .Take(request.Count)
                    .Reverse();
            }
            else
            {
                query = query
                    .OrderBy(msg => msg.SentTime);
            }
        }

        var messages = await query
            .Include(msg => msg.ImageAttachments)
            .Include(msg => msg.FileAttachments)
            .Select(msg => (CommonModels.GroupMessage)msg)
            .ToArrayAsync();

        return ApiResult<QueryGroupMessagesResultData>.CreateOk(new QueryGroupMessagesResultData(messages));
    }


    [HttpPost("SendPrivateMessage")]
    public async Task<ApiResult> SendPrivateMessageAsync(SendPrivateMessageRequestData request)
    {
        int selfUserId = HttpContext.GetUserId();
        bool selfHasFriend = await _dbContext.CheckUserHasFriendAsync(selfUserId, request.ReceiverId);

        if (!selfHasFriend)
        {
            return ApiResult.CreateErr("不是好友");
        }

        if (request.ImageAttachments is not null)
        {
            foreach (var attachment in request.ImageAttachments)
            {
                var dataExist = await _dbContext.Images
                    .AsNoTracking()
                    .AnyAsync(image => image.Hash == attachment.AttachmentHash);

                if (!dataExist)
                {
                    return ApiResult.CreateErr($"消息包含无效图片附件: {attachment.Name}");
                }
            }
        }

        if (request.FileAttachments is not null)
        {
            foreach (var attachment in request.FileAttachments)
            {
                var dataExist = await _dbContext.Files
                    .AsNoTracking()
                    .AnyAsync(image => image.Hash == attachment.AttachmentHash);

                if (!dataExist)
                {
                    return ApiResult.CreateErr($"消息包含无效文件附件: {attachment.Name}");
                }
            }
        }

        var entry = _dbContext.PrivateMessages.Add(
            new PrivateMessage()
            {
                SenderId = selfUserId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentTime = DateTimeOffset.Now,
            });

        await _dbContext.SaveChangesAsync();

        if (request.ImageAttachments is not null)
        {
            foreach (var attachment in request.ImageAttachments)
            {
                await _dbContext.PrivateMessageImageAttachments.AddAsync(
                    new PrivateMessageImageAttachment()
                    {
                        Name = attachment.Name,
                        MessageId = entry.Entity.Id,
                        AttachmentHash = attachment.AttachmentHash,
                    });
            }
        }

        if (request.FileAttachments is not null)
        {
            foreach (var attachment in request.FileAttachments)
            {
                await _dbContext.PrivateMessageFileAttachments.AddAsync(
                    new PrivateMessageFileAttachment()
                    {
                        Name = attachment.Name,
                        MessageId = entry.Entity.Id,
                        AttachmentHash = attachment.AttachmentHash,
                    });
            }
        }

        await _dbContext.SaveChangesAsync();
        await _messageNotifyService.OnPrivateMessageSent((CommonModels.PrivateMessage)entry.Entity);

        return ApiResult.CreateOk();
    }


    [HttpPost("SendGroupMessage")]
    public async Task<ApiResult> SendGroupMessageAsync(SendGroupMessageRequestData request)
    {
        int selfUserId = HttpContext.GetUserId();
        bool selfHasGroup = await _dbContext.CheckUserHasGroupAsync(selfUserId, request.GroupId);

        if (!selfHasGroup)
        {
            return ApiResult.CreateErr($"没有这个{Consts.GroupName}");
        }

        if (request.ImageAttachments is not null)
        {
            foreach (var attachment in request.ImageAttachments)
            {
                var dataExist = await _dbContext.Images
                    .AsNoTracking()
                    .AnyAsync(image => image.Hash == attachment.AttachmentHash);

                if (!dataExist)
                {
                    return ApiResult.CreateErr($"消息包含无效图片附件: {attachment.Name}");
                }
            }
        }

        if (request.FileAttachments is not null)
        {
            foreach (var attachment in request.FileAttachments)
            {
                var dataExist = await _dbContext.Files
                    .AsNoTracking()
                    .AnyAsync(image => image.Hash == attachment.AttachmentHash);

                if (!dataExist)
                {
                    return ApiResult.CreateErr($"消息包含无效文件附件: {attachment.Name}");
                }
            }
        }

        var entry = _dbContext.GroupMessages.Add(
            new GroupMessage()
            {
                SenderId = selfUserId,
                GroupId = request.GroupId,
                Content = request.Content,
                SentTime = DateTimeOffset.Now,
            });

        await _dbContext.SaveChangesAsync();

        if (request.ImageAttachments is not null)
        {
            foreach (var attachment in request.ImageAttachments)
            {
                await _dbContext.GroupMessageImageAttachments.AddAsync(
                    new GroupMessageImageAttachment()
                    {
                        Name = attachment.Name,
                        MessageId = entry.Entity.Id,
                        AttachmentHash = attachment.AttachmentHash,
                    });
            }
        }

        if (request.FileAttachments is not null)
        {
            foreach (var attachment in request.FileAttachments)
            {
                await _dbContext.GroupMessageFileAttachments.AddAsync(
                    new GroupMessageFileAttachment()
                    {
                        Name = attachment.Name,
                        MessageId = entry.Entity.Id,
                        AttachmentHash = attachment.AttachmentHash,
                    });
            }
        }

        await _dbContext.SaveChangesAsync();

        await _messageNotifyService.OnGroupMessageSent((CommonModels.GroupMessage)entry.Entity);

        return ApiResult.CreateOk();
    }

    [HttpPost("QueryLatestMessage")]
    public async Task<ApiResult<QueryLatestMessageResultData>> QueryLatestMessage(QueryLatestMessageRequestData request)
    {
        var count = Math.Min(request.Count, 100);

        var userId = HttpContext.GetUserId();

        Dictionary<CommonModels.Group, CommonModels.GroupMessage[]> groupMessages = await _dbContext.Groups
            .Include(v => v.Members)
            .Include(v => v.Messages)
            .Where(group => group.Members.Any(member => member.Id == userId))
            .SelectMany(v => v.Messages)
            .GroupBy(v => (CommonModels.Group)v.Group)
            .ToDictionaryAsync(v => v.Key, v => v.Select(v => (CommonModels.GroupMessage)v).ToArray());

        Dictionary<CommonModels.User, CommonModels.PrivateMessage[]> privateMessages = await _dbContext.PrivateMessages
            .Include(v => v.SenderId)
            .Where(msg => msg.ReceiverId == userId)
            .OrderBy(msg => msg.SentTime)
            .Take(count)
            .GroupBy(v => v.Sender)
            .ToDictionaryAsync(v => (CommonModels.User)v.Key, v => v.Select(v => (CommonModels.PrivateMessage)v).ToArray());

        return ApiResult<QueryLatestMessageResultData>.CreateOk(new(privateMessages, groupMessages));
    }
}