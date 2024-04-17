using System.Collections.ObjectModel;
using System.Windows.Data;
using StudentChat.Models;

namespace WpfStudentChat.Models;

public partial class PrivateChatSession : ObservableObject, IChatSession
{
    public PrivateChatSession(User subject)
    {
        Subject = subject;

        Messages.CollectionChanged += Messages_CollectionChanged;
        BindingOperations.EnableCollectionSynchronization(Messages, Messages);
    }

    public User Subject { get; set; }

    public ObservableCollection<PrivateMessage> Messages { get; } = new();

    [ObservableProperty] private int _unreadMessageCount;

    public DateTimeOffset LastReadTime { get; set; }

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
