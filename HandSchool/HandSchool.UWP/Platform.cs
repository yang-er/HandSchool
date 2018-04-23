using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Page = Windows.UI.Xaml.Controls.Page;
using Behavior = Xamarin.Forms.Behavior;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace HandSchool
{
    namespace Internal
    {
        partial class Helper
        {
            public static Action ShowLoadingAlert(string tips, string title = "提示")
            {
                // xxx.Show()?
                return () => {
                    // xxxx.Close()?
                };
            }

            public static async Task ShowMessage(string title, string message, string button = "确认")
            {
                var dialog = new MessageDialog(message, title);
                dialog.Commands.Add(new UICommand(button));
                await dialog.ShowAsync();
            }
        }
    }

    namespace UWP
    {
        public class ViewResponse : IViewResponse
        {
            public ViewResponse(ViewPage page)
            {
                Binding = page;
                _behavior = new ObservableCollection<Behavior>();
                _behavior.CollectionChanged += CollectionChanged;
            }

            public ViewPage Binding { get; }

            private ObservableCollection<Behavior> _behavior;
            public IList<Behavior> Behaviors => _behavior;

            private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.OldItems != null)
                {
                    foreach (Behavior p in e.OldItems)
                    {
                        //p.
                    }
                }
            }

            public Task ShowMessage(string title, string message, string button = "确认")
            {
                return Helper.ShowMessage(title, message, button);
            }

            public void SetIsBusy(bool value, string tips)
            {
                Binding.BindingContext.IsBusy = value;
            }
        }
    }
}
