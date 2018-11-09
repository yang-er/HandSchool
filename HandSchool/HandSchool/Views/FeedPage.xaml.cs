using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : PopContentPage
    {
        public bool IsPushing { get; set; } = false;

        public FeedPage()
        {
            InitializeComponent();
            ViewModel = FeedViewModel.Instance;
            TabletEnabled = true;
        }

        object LastItem;

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem || IsPushing)
                return;
            if (Device.Idiom == TargetIdiom.Tablet)
                LastItem = e.Item;
            IsPushing = true;
            await Navigation.PushAsync(new MessageDetailPage(e.Item as FeedItem));
            IsPushing = false;
        }
    }
}
