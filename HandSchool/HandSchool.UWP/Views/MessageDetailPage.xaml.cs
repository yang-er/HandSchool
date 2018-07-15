using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Command = Xamarin.Forms.Command;
using Device = Xamarin.Forms.Device;

namespace HandSchool.UWP
{
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
            Tag = e.Parameter;

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
                PrimaryMenu.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Delete), Label = "删除", Command = msg.Delete });
                Title = msg.Title;
                Time = "时间：" + msg.Time.ToString();
                Sender = "发件人：" + msg.Sender;
                Body = msg.Body;
            }
        }
    }
}
