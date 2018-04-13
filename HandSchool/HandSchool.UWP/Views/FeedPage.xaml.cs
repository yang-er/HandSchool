using HandSchool.Models;
using HandSchool.ViewModels;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    public sealed partial class FeedPage : ViewPage
    {
        public FeedPage()
        {
            InitializeComponent();
            BindingContext = FeedViewModel.Instance;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FeedItem item)
            {
                Frame.Navigate(typeof(MessageDetailPage), item);
                ListView.SelectedItem = null;
            }
        }
    }
}
