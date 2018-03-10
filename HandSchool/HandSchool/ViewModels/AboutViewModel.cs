using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://github.com/yang-er")));
            OpenWebCommand2 = new Command(() => Device.OpenUri(new Uri("http://www.90yang.com")));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand OpenWebCommand2 { get; }
    }
}