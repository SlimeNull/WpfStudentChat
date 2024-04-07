using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentChat.Models.Events
{
    public class PrivateMessageReceivedEventArgs(PrivateMessage message) : EventArgs
    {
        public PrivateMessage Message { get; } = message;
    }
}
