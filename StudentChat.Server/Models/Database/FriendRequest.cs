using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string Message { get; set; } = string.Empty;

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public static explicit operator CommonModels.FriendRequest(FriendRequest request)
        {
            return new CommonModels.FriendRequest()
            {
                Id = request.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Message = request.Message,
            };
        }
    }
}
