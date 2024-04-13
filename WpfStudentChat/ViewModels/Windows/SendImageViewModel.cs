using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows
{
    public partial class SendImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _imagePath = string.Empty;

        [ObservableProperty]
        private string _caption = string.Empty;

        [ObservableProperty]
        private IIdentifiable? _target;
    }
}
