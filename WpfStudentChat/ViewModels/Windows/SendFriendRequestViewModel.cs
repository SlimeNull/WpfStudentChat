using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows
{
    public partial class SendFriendRequestViewModel : ObservableObject
    {
        [ObservableProperty]
        private User _profile = new();

        [ObservableProperty]
        private string _message = string.Empty;
    }
}
