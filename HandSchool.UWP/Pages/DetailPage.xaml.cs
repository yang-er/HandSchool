using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.Views
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
            ViewModel = new BaseViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tag = e.Parameter;

            if (e.Parameter is FeedItem feed)
            {
                ViewModel.Title = "通知详情";

                PrimaryMenu.Add(new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.Flag),
                    Label = "详情",
                    Command = new CommandAction(() => Core.Platform.OpenUrl(feed.Link))
                });

                Title = feed.Title;
                Time = "时间：" + feed.PubDate;
                Sender = "分类：" + feed.Category;
                var desc = feed.Description.Trim();
                while (desc.Contains("    ")) desc = desc.Replace("    ", "  ");
                Body = desc;
            }
            else if (e.Parameter is IMessageItem msg)
            {
                ViewModel.Title = "消息详情";

                PrimaryMenu.Add(new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.Delete),
                    Label = "删除",
                    Command = msg.Delete
                });

                Title = msg.Title;
                Time = "时间：" + msg.Time.ToString();
                Sender = "发件人：" + msg.Sender;
                Body = msg.Body;
            }
        }
    }
}
