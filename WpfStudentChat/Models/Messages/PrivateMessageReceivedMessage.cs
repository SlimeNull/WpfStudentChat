using StudentChat.Models;

namespace WpfStudentChat.Models.Messages;

public class PrivateMessageReceivedMessage(PrivateMessage message)
{
    public PrivateMessage Message { get; } = message;
}
