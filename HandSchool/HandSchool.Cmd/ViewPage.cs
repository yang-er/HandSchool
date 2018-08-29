using System;
using System.Threading.Tasks;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class EmptyPage
    {
        public Task ShowAsync(INavigation iv = null) => Task.Delay(100);
    }

    public class LoginPage : EmptyPage
    {
        internal LoginPage(LoginViewModel viewModel) { }
        internal void Response(object sender, LoginStateEventArgs e) { }
    }

    public class CurriculumPage : EmptyPage
    {
        internal CurriculumPage(CurriculumItem a, bool b) { }
    }
}
