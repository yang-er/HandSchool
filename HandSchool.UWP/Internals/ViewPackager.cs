using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    internal class ViewPackager : IViewPage
    {
        public Task Pushed { get; } = new Task(() => { });
        public BaseViewModel ViewModel { get; set; }
        public View Content { get; set; }
        public string Title { get; set; }
        public List<MenuEntry> MenuEntries { get; } = new List<MenuEntry>();

        public INavigate Navigation { get; private set; }

        public void RegisterNavigation(INavigate navigate) => Navigation = navigate;

        public bool IsModal { get; set; }
        
        public event EventHandler Disappearing;
        public event EventHandler Appearing;

        public void RaiseAppearing(object sender, EventArgs args) => Appearing?.Invoke(sender, args);
        public void RaiseDisappearing(object sender, EventArgs args) => Disappearing?.Invoke(sender, args);

        public void AddToolbarEntry(MenuEntry item) => MenuEntries.Add(item);

        public Task ShowAsync(INavigate parent) => parent?.PushAsync(this);

        public Task CloseAsync() => Navigation.PopAsync();

        public Task RequestMessageAsync(string title, string message, string button)
        {
            return ViewResponseImpl.ShowMessageAsync1(title, message, button);
        }

        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            return ViewResponseImpl.ShowAskAsync1(title, description, cancel, accept);
        }

        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return ViewResponseImpl.DisplayActionSheet(Navigation, title, cancel, destruction, buttons);
        }

        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            throw new NotImplementedException();
        }
    }
}
