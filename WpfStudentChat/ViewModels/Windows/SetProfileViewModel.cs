﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows
{
    public partial class SetProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private User profile = new();
    }
}