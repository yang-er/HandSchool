using HandSchool.Models;
using HandSchool.ViewModels;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    public sealed partial class FeedPage : ViewPage
    {
        public FeedPage()
        {
            InitializeComponent();
            ViewModel = FeedViewModel.Instance;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FeedItem item)
            {
                Frame.Navigate(typeof(MessageDetailPage), item);
            }
        }
    }
}
