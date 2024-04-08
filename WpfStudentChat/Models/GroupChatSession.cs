using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.Models
{
    public class GroupChatSession : IChatSession
    {
        public GroupChatSession(Group subject)
        {
            Subject = subject;
        }

        public Group Subject { get; }

        public ObservableCollection<GroupMessage> Messages { get; } = new();

        IIdentifiable IChatSession.Subject => Subject;
        IEnumerable<Message> IChatSession.Messages => Messages;
    }
}
