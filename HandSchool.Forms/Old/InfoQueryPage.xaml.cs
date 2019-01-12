using HandSchool.Internal;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoQueryPage : ViewPage
	{
		public InfoQueryPage()
		{
			InitializeComponent();
            MyListView.ItemsSource = Core.App.InfoEntrances;
            this.On<iOS, ViewPage>().UseTabletMode();
        }

        object LastItem;

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem)
                return;
            if (Device.Idiom == TargetIdiom.Tablet)
                LastItem = e.Item;

            if (e.Item is InfoEntranceWrapper iew)
            {
                var webpg = new WebViewPage(iew.Load.Invoke());
                await Navigation.PushAsync(webpg);
            }
            else if (e.Item is TapEntranceWrapper tew)
            {
                await tew.Activate(Navigation);
            }
        }

        /* public override Page SetTabletDefaultPage()
        {
            LastItem = Core.App.InfoEntrances[Core.App.InfoEntrances.Count - 1][1];
            return new AboutPage();
        } */
    }
}