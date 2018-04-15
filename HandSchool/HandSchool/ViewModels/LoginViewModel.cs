using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; set; }
        public ILoginField Form { get; }
        public LoginPage Page { get; set; }
        
        private LoginViewModel(ILoginField form)
        {
            Form = form;
        }

        public static async Task<bool> RequestAsync(ILoginField form)
        {
            var viewModel = new LoginViewModel(form);
            viewModel.LoginCommand = new Command(viewModel.Login);
#if __UWP__
            /*
            var newView = Windows.ApplicationModel.Core.CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var newWindow = Windows.UI.Xaml.Window.Current;
                var newAppView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                var frame = new Windows.UI.Xaml.Controls.Frame();
                frame.Navigate(typeof(UWP.SchedulePage));
                newWindow.Content = frame;
                newWindow.Activate();
                newViewId = newAppView.Id;
            });
            var viewShown = await Windows.UI.ViewManagement.ApplicationViewSwitcher.TryShowAsViewModeAsync(newViewId, Windows.UI.ViewManagement.ApplicationViewMode.CompactOverlay);*/
            await (new UWP.LoginDialog(viewModel)).ShowAsync();
#else
            viewModel.Page = new LoginPage(viewModel);
            await viewModel.Page.ShowAsync();
#endif
            return form.IsLogin;
        }
        
        async void Login()
        {
            if (IsBusy)
            {
                Page.Response(this, new LoginStateEventArgs(LoginState.Processing));
                return;
            }

            IsBusy = true;

            var behavior = new LoadingBehavior("正在登录……");
            Page.Behaviors.Add(behavior);
            Form.LoginStateChanged += Page.Response;

            try
            {
                await Form.Login();
            }
            finally
            {
                IsBusy = false;
                Page.Behaviors.Remove(behavior);
                Form.LoginStateChanged -= Page.Response;
            }
        }
    }
}
