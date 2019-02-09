using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ViewObject
	{
        FeedItem LastItem = null;
        bool IsPushing = false;
        ListView ListView = null;

		public FeedPage()
		{
			InitializeComponent();
            ViewModel = FeedViewModel.Instance;
            ListView = Content as ListView;

            if (Core.Platform.RuntimeName == "Android")
            {
                ListView.SeparatorVisibility = SeparatorVisibility.None;
                ListView.Header = new StackLayout { HeightRequest = 4 };
                ListView.BackgroundColor = Color.FromRgb(244, 244, 244);
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem || IsPushing) return;
            IsPushing = true;

            LastItem = e.Item as FeedItem;
            await Navigation.PushAsync<DetailPage>(LastItem);

            if (Device.Idiom != TargetIdiom.Tablet) LastItem = null;
            IsPushing = false;
        }

        private async void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (FeedViewModel.Instance.LeftPageCount == 0) return;
            if (FeedViewModel.Instance.IsBusy) return;

            if (e.Item == FeedViewModel.Instance.Last())
            {
                await FeedViewModel.Instance.LoadItems(true);
            }
        }
    }
}