using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class GroupRequestsViewModel : ObservableObject
    {
        public ObservableCollection<GroupRequest> Requests { get; } = new();
        
    }
}
