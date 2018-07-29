using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP.Views
{
    public sealed partial class MessagePage : ViewPage
    {
        public MessagePage()
        {
            InitializeComponent();
            ViewModel = MessageViewModel.Instance;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IMessageItem item)
            {
                var a = e.ClickedItem as IMessageItem;
                Task.Run(async () => { await Core.App.Message.SetReadState(a.Id, true); a.Unread = false; });
                Frame.Navigate(typeof(MessageDetailPage), item);
            }
        }
    }
}
