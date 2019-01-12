using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ViewPage
    {
        public MessagePage()
        {
            InitializeComponent();
            ViewModel = MessageViewModel.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessageViewModel.Instance.FirstOpen();
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            var imi = e.Item as IMessageItem;
            imi.SetRead.Execute(null);
            await Navigation.PushAsync(new MessageDetailPage(imi));

            // Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
        
        private async void Handle_Clicked(object sender, EventArgs e)
        {
            var result = await RequestActionAsync(
                "消息中心", "取消", null,
                "刷新列表", "全部已读", "全部删除");

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
