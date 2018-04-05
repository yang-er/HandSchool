using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoQueryPage : ContentPage
	{
		public InfoQueryPage()
		{
			InitializeComponent();
            MyListView.ItemsSource = App.Current.InfoEntrances;
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var iew = e.Item as InfoEntranceWrapper;
            var webpg = new WebViewPage(iew.Load.Invoke());
            await webpg.ShowAsync(Navigation);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}