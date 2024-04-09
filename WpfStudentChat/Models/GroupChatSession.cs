using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.Models
{
    public class GroupChatSession : ObservableObject, IChatSession
    {
        public GroupChatSession(Group subject)
        {
            Subject = subject;

            Messages.CollectionChanged += Messages_CollectionChanged;
        }

        public Group Subject { get; }

        public ObservableCollection<GroupMessage> Messages { get; } = new();

        public string LastMessageSummary
        {
            get
            {
                var lastMessage = Messages.LastOrDefault();
                if (lastMessage is null)
                    return string.Empty;

                string content = lastMessage.Content;
                int end = content.IndexOf('\n');
                if (end != -1)
                {
                    content = content.Substring(0, end);
                }

                return lastMessage.Content;
            }
        }

        IIdentifiable IChatSession.Subject => Subject;
        IEnumerable<Message> IChatSession.Messages => Messages;

        private void Messages_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LastMessageSummary));
        }
    }
}
