using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WpfStudentChat.Server.Models;
using WpfStudentChat.Server.Models.Database;

namespace WpfStudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;

        public record class SendFriendRequestRequestData(int UserId, string Message);
        public record class SendGroupRequestRequestData(int GroupId, string Message);

        public RequestController(ChatServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ApiResult> SendFriendRequest(SendFriendRequestRequestData request)
        {
            throw new NotImplementedException();
        }


    }
}
