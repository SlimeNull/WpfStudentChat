using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string? Message { get; set; }
        public string? RejectReason { get; set; }

        public bool IsDone { get; set; }

        public DateTimeOffset SentTime { get; set; }

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public async Task LoadAllPropertiesAsync(ChatServerDbContext dbContext)
        {
            await dbContext.Entry(this)
                .Reference(v => v.Sender)
                .LoadAsync();

            await dbContext.Entry(this)
                .Reference(v => v.Receiver)
                .LoadAsync();
        }


        public static explicit operator CommonModels.FriendRequest(FriendRequest request)
        {
            return new CommonModels.FriendRequest()
            {
                Id = request.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Message = request.Message,
                RejectReason = request.RejectReason,
                IsDone = request.IsDone,
                SentTime = request.SentTime,
                SenderName = request.Sender.Nickname,
                ReceiverName = request.Receiver.Nickname,
            };
        }
    }
}
