using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        public ViewResponse(ViewPage page)
        {
            Binding = page;
        }
        
        public ViewPage Binding { get; }
            
        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return ShowMessageAsync(title, message, button);
        }

        public Task<bool> ShowActionSheet(string title, string description, string cancel, string accept)
        {
            return ShowActionSheetAsync(title, description, cancel, accept);
        }

        public void SetIsBusy(bool value, string tips) { }

        public static async Task ShowMessageAsync(string title, string message, string button = "确认")
        {
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(button));
            await dialog.ShowAsync();
        }

        public static async Task<bool> ShowActionSheetAsync(string title, string description, string cancel, string accept)
        {
            var dialog = new MessageDialog(description, title);
            dialog.Commands.Add(new UICommand(accept));
            dialog.Commands.Add(new UICommand(cancel));
            var result = await dialog.ShowAsync();
            return result.Label == accept;
        }
    }
}
