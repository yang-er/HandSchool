using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.JLU.ViewModels
{
    class LibZwyyViewModel : BaseViewModel
    {
        public static readonly Lazy<LibZwyyViewModel> Lazy = new Lazy<LibZwyyViewModel>(() =>
        {
            return new LibZwyyViewModel();
        });

        private LibZwyyViewModel()
        {
            Title = "鼎新馆研讨间预约";

        }
    }
}
