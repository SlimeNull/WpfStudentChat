﻿namespace StudentChat.Models.Events
{
    public class GroupChangedEventArgs(Group group) : EventArgs
    {
        public Group Group { get; } = group;
    }
}
