using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat;
using Microsoft.Extensions.Options;
using WpfStudentChat.Models;
using CommunityToolkit.Mvvm.Messaging;
using WpfStudentChat.Models.Messages;

namespace WpfStudentChat.Services
{
    public class ChatClientService
    {
        private readonly IMessenger _messenger;

        public ChatClient Client { get; }

        public ChatClientService(
            IOptionsSnapshot<AppConfig> optionsAppConfig,
            IMessenger messenger)
        {
            _messenger = messenger;

            Client = new(new Uri(optionsAppConfig.Value.BaseUri, UriKind.Absolute));
            Client.PrivateMessageReceived += Client_PrivateMessageReceived;
            Client.GroupMessageReceived += Client_GroupMessageReceived;
            Client.GroupIncreased += Client_GroupIncreased;
            Client.GroupDecreased += Client_GroupDecreased;
            Client.FriendIncreased += Client_FriendIncreased;
            Client.FriendDecreased += Client_FriendDecreased;
            Client.FriendRequestReceived += Client_FriendRequestReceived;
            Client.GroupRequestReceived += Client_GroupRequestReceived;
        }

        private void Client_GroupRequestReceived(object? sender, StudentChat.Models.Events.GroupRequestReceivedEventArgs e)
        {
            _messenger.Send(new GroupRequestReceivedMessage(e.Request));
        }

        private void Client_FriendRequestReceived(object? sender, StudentChat.Models.Events.FriendRequestReceivedEventArgs e)
        {
            _messenger.Send(new FriendRequestReceivedMessage(e.Request));
        }

        private void Client_FriendDecreased(object? sender, StudentChat.Models.Events.FriendChangedEventArgs e)
        {
            _messenger.Send(new FriendDecreasedMessage(e.Friend));
        }

        private void Client_FriendIncreased(object? sender, StudentChat.Models.Events.FriendChangedEventArgs e)
        {
            _messenger.Send(new FriendIncreasedMessage(e.Friend));
        }

        private void Client_GroupDecreased(object? sender, StudentChat.Models.Events.GroupChangedEventArgs e)
        {
            _messenger.Send(new GroupDecreasedMessage(e.Group));
        }

        private void Client_GroupIncreased(object? sender, StudentChat.Models.Events.GroupChangedEventArgs e)
        {
            _messenger.Send(new GroupIncreasedMessage(e.Group));
        }

        private void Client_GroupMessageReceived(object? sender, StudentChat.Models.Events.GroupMessageReceivedEventArgs e)
        {
            _messenger.Send(new GroupMessageReceivedMessage(e.Message));
        }

        private void Client_PrivateMessageReceived(object? sender, StudentChat.Models.Events.PrivateMessageReceivedEventArgs e)
        {
            _messenger.Send(new PrivateMessageReceivedMessage(e.Message));
        }
    }
}
