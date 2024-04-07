namespace WpfStudentChat.Server.Utilities
{
    public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs args) where TEventArgs : EventArgs;
}
