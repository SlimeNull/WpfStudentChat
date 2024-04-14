using StudentChat.Models;

namespace WpfStudentChat.Models;

public interface IChatSession
{
    public IIdentifiable Subject { get; }

    public IEnumerable<Message> Messages { get; }

    public string LastMessageSummary { get; }
}
