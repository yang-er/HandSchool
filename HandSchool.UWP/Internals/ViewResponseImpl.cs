using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

namespace HandSchool.UWP
{
    internal static class ViewResponseImpl
    {
        public static Task<string> DisplayActionSheet(Frame frame, string title, string cancel, string destruction, params string[] buttons)
        {
            var options = new ActionSheetArguments(title, cancel, destruction, buttons);
            var actionSheet = ActionSheetFlyout(options);
            actionSheet.ShowAt(frame);
            return options.Result.Task;
        }

        public static Task<string> DisplayActionSheet(Views.INavigate navigate, string title, string cancel, string destruction, params string[] buttons)
        {
            return DisplayActionSheet((navigate as NavigateImpl).InnerFrame, title, cancel, destruction, buttons);
        }

        public static async Task ShowMessageAsync1(string title, string message, string button)
        {
            var dialog2 = new TextDialog(title, message, button);
            await dialog2.ShowAsync();
        }

        public static async Task ShowMessageAsync2(string title, string message, string button)
        {
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(button));
            await dialog.ShowAsync();
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

        public static async Task<string> ShowInputAsync1(string title, string description, string cancel, string accept)
        {
            var dialog = new TextDialog(title, description, accept, cancel, "");
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary ? dialog.TextBox.Text : null;
        }

        public static async Task<bool> ShowAskAsync1(string title, string description, string cancel, string accept)
        {
            var dialog = new TextDialog(title, description, accept, cancel);
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public static async Task<bool> ShowAskAsync2(string title, string description, string cancel, string accept)
        {
            var dialog = new MessageDialog(description, title);
            dialog.Commands.Add(new UICommand(accept));
            dialog.Commands.Add(new UICommand(cancel));
            var result = await dialog.ShowAsync();
            return result.Label == accept;
        }
    }
}
