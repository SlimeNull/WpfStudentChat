using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class GroupRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }

        public string Message { get; set; } = string.Empty;

        public User Sender { get; set; } = null!;
        public Group Group { get; set; } = null!;


        public static explicit operator CommonModels.GroupRequest(GroupRequest request)
        {
            return new CommonModels.GroupRequest()
            {
                Id = request.Id,
                SenderId = request.SenderId,
                GroupId = request.GroupId,
                Message = request.Message,
            };
        }
    }
}
