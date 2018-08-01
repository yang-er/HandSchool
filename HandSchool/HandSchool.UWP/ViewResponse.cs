using HandSchool.UWP.Views;
using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using XPage = Xamarin.Forms.Page;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        public ViewResponse(ViewPage page)
        {
            Binding = page;
        }

        public ViewResponse(XPage page)
        {
            System.Diagnostics.Debug.Assert(false, "UWP modules shouldn't accept Xamarin");
        }

        public ViewPage Binding { get; }
            
        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return ShowMessageAsync(title, message, button);
        }

        public void SetIsBusy(bool value, string tips) { }

        public static async Task ShowMessageAsync(string title, string message, string button = "确认")
        {
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(button));
            await dialog.ShowAsync();
        }
    }
}
