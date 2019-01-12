using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class MessagePage : ViewPage
    {
        public MessagePage()
        {
            InitializeComponent();
            ViewModel = MessageViewModel.Instance;
            MessageViewModel.Instance.FirstOpen();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IMessageItem item)
            {
                var a = e.ClickedItem as IMessageItem;
                Task.Run(() => a.SetRead.Execute(null));
                Frame.Navigate(typeof(MessageDetailPage), item);
            }
        }
    }
}
