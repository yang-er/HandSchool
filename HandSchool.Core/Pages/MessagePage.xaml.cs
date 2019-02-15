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

            if (Core.Platform.RuntimeName == "Android")
            {
                var ListView = Content as ListView;
                ListView.SeparatorVisibility = SeparatorVisibility.None;
                ListView.Header = new StackLayout { HeightRequest = 4 };
                ListView.Footer = new StackLayout { HeightRequest = 4 };
                ListView.BackgroundColor = Color.FromRgb(244, 244, 244);
            }
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
            await Navigation.PushAsync<DetailPage>(imi);
            IsPushing = false;
        }
    }
}