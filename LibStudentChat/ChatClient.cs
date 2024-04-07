using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibStudentChat
{
    public class ChatClient
    {
        public ChatClient(Uri baseUri)
        {
            BaseUri = baseUri;
        }

        public Uri BaseUri { get; }

        public void LoginAsync(string userName, string password)
        {

        }
    }
}
