using HandSchool.Models;
using HandSchool.ViewModels;
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
    }
}
