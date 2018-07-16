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


            public static Windows.UI.Color[] ScheduleColors2 = {
                Windows.UI.Color.FromArgb(0xff, 0x59, 0xe0, 0x9e),
                Windows.UI.Color.FromArgb(0xff, 0xf4, 0x8f, 0xb1),
                Windows.UI.Color.FromArgb(0xff, 0xce, 0x93, 0xd8),
                Windows.UI.Color.FromArgb(0xff, 0xff, 0x8a, 0x65),
                Windows.UI.Color.FromArgb(0xff, 0x9f, 0xa8, 0xda),
                Windows.UI.Color.FromArgb(0xff, 0x42, 0xa5, 0xf5),
                Windows.UI.Color.FromArgb(0xff, 0x80, 0xde, 0xea),
                Windows.UI.Color.FromArgb(0xff, 0xc6, 0xde, 0x7c)
            };
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
