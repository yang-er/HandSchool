using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

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

        public Task<bool> ShowAskMessage(string title, string description, string cancel, string accept)
        {
            return ShowActionSheetAsync(title, description, cancel, accept);
        }
        
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            var options = new ActionSheetArguments(title, cancel, destruction, buttons);
            var actionSheet = ActionSheetFlyout(options);
            actionSheet.ShowAt(Binding.Frame);
            return options.Result.Task;
        }

        public void SetIsBusy(bool value, string tips) { }

        public static async Task ShowMessageAsync(string title, string message, string button = "确认")
        {
            var dialog2 = new TextDialog(title, message, button);
            await dialog2.ShowAsync();

            //var dialog = new MessageDialog(message, title);
            //dialog.Commands.Add(new UICommand(button));
            //await dialog.ShowAsync();
        }

        static Flyout ActionSheetFlyout(ActionSheetArguments options)
        {
            bool userDidSelect = false;
            var flyoutContent = new FormsFlyout(options);
            
            var actionSheet = new Flyout
            {
                FlyoutPresenterStyle = (Style) Application.Current.Resources["FormsFlyoutPresenterStyle"],
                Placement = FlyoutPlacementMode.Full,
                Content = flyoutContent
            };
            
            flyoutContent.OptionSelected += (s, e) =>
            {
                userDidSelect = true;
                actionSheet.Hide();
            };
            
            actionSheet.Closed += (s, e) =>
            {
                if (!userDidSelect)
                    options.SetResult(null);
            };

            return actionSheet;
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
