using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class PrivateMessageReceivedMessage(PrivateMessage message)
    {
        public PrivateMessage Message { get; } = message;
    }
}
