using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : PopContentPage
	{
        public AboutPage()
        {
            InitializeComponent();
            ViewModel = AboutViewModel.Instance;
#if __IOS__
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);
#endif
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is InfoEntranceWrapper iew)
            {
                var webpg = new WebViewPage(iew.Load.Invoke());
                await webpg.ShowAsync(Navigation);
            }
            else if (e.Item is TapEntranceWrapper tew)
            {
                await tew.Activate(Navigation);
            }
        }
    }
}
