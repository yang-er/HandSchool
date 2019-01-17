using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ViewObject
	{
        FeedItem LastItem = null;
        bool IsPushing = false;

		public FeedPage()
		{
			InitializeComponent();
            ViewModel = FeedViewModel.Instance;
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem || IsPushing) return;
            IsPushing = true;

            LastItem = e.Item as FeedItem;
            await Navigation.PushAsync("MessageDetailPage", LastItem);

            if (Device.Idiom != TargetIdiom.Tablet) LastItem = null;
            IsPushing = false;
        }
    }
}