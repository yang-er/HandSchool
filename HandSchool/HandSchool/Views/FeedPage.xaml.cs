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
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is null || IsPushing)
                return;
            IsPushing = true;
            await (new MessageDetailPage(e.Item as FeedItem)).ShowAsync(Navigation);

            // Deselect Item
            ((ListView)sender).SelectedItem = null;
            IsPushing = false;
        }
    }
}