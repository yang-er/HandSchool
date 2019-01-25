using HandSchool.Models;
using HandSchool.ViewModels;
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

        bool IsPushing = false;

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || IsPushing) return;
            IsPushing = true;
            var imi = e.Item as IMessageItem;
            imi.SetRead.Execute(null);
            await Navigation.PushAsync(typeof(DetailPage), imi);
            IsPushing = false;
        }
    }
}