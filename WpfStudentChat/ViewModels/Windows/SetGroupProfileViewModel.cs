using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows
{
    public partial class SetGroupProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private Group profile = new();
    }
}
