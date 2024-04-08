using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.Models
{
    public class PrivateChatSession : IChatSession
    {
        public PrivateChatSession(User subject)
        {
            Subject = subject;
        }

        public User Subject { get; set; }

        public ObservableCollection<PrivateMessage> Messages { get; } = new();

        IIdentifiable IChatSession.Subject => Subject;
        IEnumerable<Message> IChatSession.Messages => Messages;
    }
}
