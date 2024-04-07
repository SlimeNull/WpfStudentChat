namespace LibStudentChat.Models.Events
{
    public class GroupRequestReceivedEventArgs(GroupRequest request) : EventArgs
    {
        public GroupRequest Request { get; } = request;
    }
}
