﻿namespace StudentChat.Models.Events
{
    public class GroupMessageReceivedEventArgs(GroupMessage message) : EventArgs
    {
        public GroupMessage Message { get; } = message;
    }
}
