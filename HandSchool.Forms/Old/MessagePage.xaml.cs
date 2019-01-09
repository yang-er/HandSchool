using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : PopContentPage
    {
        public MessagePage()
        {
            InitializeComponent();
            ViewModel = MessageViewModel.Instance;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            SetReadState(e.Item as IMessageItem);
            
            await (new MessageDetailPage(e.Item as IMessageItem)).ShowAsync(Navigation);

            // Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void SetReadState(IMessageItem item)
        {
            Task.Run(async () => { await Core.App.Message.SetReadState(item.Id, true); item.Unread = false; });
        }

        private async void Handle_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("消息中心", "取消", null, "刷新列表", "全部已读", "全部删除");

            switch (result)
            {
                case "刷新列表":
                    MessageViewModel.Instance.LoadItemsCommand.Execute(null);
                    break;
                case "全部已读":
                    MessageViewModel.Instance.ReadAllCommand.Execute(null);
                    break;
                case "全部删除":
                    MessageViewModel.Instance.DeleteAllCommand.Execute(null);
                    break;
                default:
                    break;
            }
        }
    }
}
