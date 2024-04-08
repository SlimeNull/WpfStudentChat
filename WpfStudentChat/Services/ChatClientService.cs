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
