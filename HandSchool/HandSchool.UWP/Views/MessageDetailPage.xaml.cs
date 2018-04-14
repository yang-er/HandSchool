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
using Command = Xamarin.Forms.Command;
using Device = Xamarin.Forms.Device;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MessageDetailPage : ViewPage
    {
        public string Title { get; private set; }
        public string Time { get; private set; }
        public string Sender { get; private set; }
        public string Body { get; private set; }

        public MessageDetailPage()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FeedItem feed)
            {
                BindingContext.Title = "通知详情";
                PrimaryMenu.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Flag), Label = "详情", Command = new Command(() => Device.OpenUri(new Uri(feed.Link))) });
                Title = feed.Title;
                Time = "时间：" + feed.PubDate;
                Sender = "分类：" + feed.Category;
                Body = feed.Description.Replace(' ', '\n');
            }
            else if (e.Parameter is IMessageItem msg)
            {
                BindingContext.Title = "消息详情";
                PrimaryMenu.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Delete), Label = "设为未读", Command = msg.SetUnread });
                PrimaryMenu.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Delete), Label = "删除", Command = msg.Delete });
                Title = msg.Title;
                Time = "时间：" + msg.Time.ToString();
                Sender = "发件人：" + msg.Sender;
                Body = msg.Body;
            }
        }
    }
}
