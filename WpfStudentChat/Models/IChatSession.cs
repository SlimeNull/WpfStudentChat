using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.Models
{
    interface IChatSession
    {
        public IIdentifiable Subject { get; }

        public IEnumerable<Message> Messages { get; }
    }
}
