using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using HandSchool.Internal;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MessagePage : ViewPage
    {
        public MessagePage()
        {
            this.InitializeComponent();
            BindingContext = MessageViewModel.Instance;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IMessageItem item)
            {
                var a = e.ClickedItem as IMessageItem;
                Task.Run(async () => { await Core.App.Message.SetReadState(a.Id, true); a.Unread = false; });
                Frame.Navigate(typeof(MessageDetailPage), item);
                ListView.SelectedItem = null;
            }
        }
    }
}
