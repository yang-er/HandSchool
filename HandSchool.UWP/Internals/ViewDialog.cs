using HandSchool.Internal;
using HandSchool.UWP;
using HandSchool.ViewModels;
using Microcharts;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class ViewDialog : ContentDialog, IViewPage
    {
        public BaseViewModel ViewModel
        {
            get => DataContext as BaseViewModel;
            set => DataContext = value;
        }

        public bool IsModal => true;

        View IViewPage.Content { get; set; }

        string IViewPage.Title
        {
            get => (string)Title;
            set => Title = value;
        }

        #region INavigate Impl
        
        public ViewDialog()
        {
            Closed += (sender, args) => Disappearing?.Invoke(sender, EventArgs.Empty);
            Loaded += (sender, args) => Appearing?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler Disappearing;

        public event EventHandler Appearing;

        public INavigate Navigation { get; private set; }

        public void AddToolbarEntry(MenuEntry item)
        {
            throw new InvalidOperationException();
        }

        public Task CloseAsync()
        {
            throw new InvalidOperationException();
        }

        public void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }

        public async Task ShowAsync(INavigate parent)
        {
            await ShowAsync();
        }

        #endregion

        #region IViewResponse Impl

        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            throw new InvalidOperationException();
        }

        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            return ViewResponseImpl.ShowAskAsync2(title, description, cancel, accept);
        }

        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            throw new InvalidOperationException();
        }

        public Task RequestMessageAsync(string title, string message, string button)
        {
            return ViewResponseImpl.ShowMessageAsync2(title, message, button);
        }

        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
