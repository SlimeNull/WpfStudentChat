using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibStudentChat;
using Microsoft.Extensions.Options;
using WpfStudentChat.Models;

namespace WpfStudentChat.Services
{
    public class ChatClientService
    {
        ChatClient _client;

        public ChatClientService(IOptionsSnapshot<AppConfig> optionsAppConfig)
        {
            _client = new(new Uri(optionsAppConfig.Value.BaseUri, UriKind.Absolute));
        }

        
    }
}
