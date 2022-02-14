using System;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ViewObject
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

        private bool _isPushing;

        private async void MessageClicked(object sender, CollectionItemTappedEventArgs e)
        {
            var item = e.Item as IMessageItem;
            if (_isPushing) return;
            _isPushing = true;
            item?.SetRead.Execute(null);
            await Navigation.PushAsync<DetailPage>(item);
            _isPushing = false;
        }

        private async void MessageLongClicked(object sender, CollectionItemTappedEventArgs e)
        {
            var item = e.Item as IMessageItem;
            var opts = new[] {"设为已读", "设为未读", "删除"};
            var opt = await RequestActionAsync("你要将此消息", "取消", null, opts);
            if (string.IsNullOrEmpty(opt)) return;
            if (opt == opts[0])
            {
                item?.SetRead.Execute(null);
            }
            else if (opt == opts[1])
            {
                item?.SetUnread.Execute(null);
            }
            else if (opt == opts[2])
            {
                item?.Delete.Execute(null);
            }
        }
    }
}