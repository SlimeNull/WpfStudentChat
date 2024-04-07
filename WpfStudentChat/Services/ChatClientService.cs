using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat;
using Microsoft.Extensions.Options;
using WpfStudentChat.Models;

namespace WpfStudentChat.Services
{
    public class ChatClientService
    {
        public ChatClient Client { get; }

        public ChatClientService(IOptionsSnapshot<AppConfig> optionsAppConfig)
        {
            Client = new(new Uri(optionsAppConfig.Value.BaseUri, UriKind.Absolute));
        }

        
    }
}
