using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class PrivateMessagesViewModel : ObservableObject
    {
        [ObservableProperty]
        private User? _target;

        [ObservableProperty]
        private string _textInput = string.Empty;

        public ObservableCollection<PrivateMessage> Messages { get; } = new();


    }
}
