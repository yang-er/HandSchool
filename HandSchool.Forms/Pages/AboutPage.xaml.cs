using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ViewPage
	{
        public AboutPage()
        {
            InitializeComponent();
            ViewModel = AboutViewModel.Instance;
            On<_iOS_>().UseSafeArea();
        }

        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

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

        private void PopContentPage_SizeChanged(object sender, System.EventArgs e)
        {
            string visualState = Width > Height ? "Landscape" : "Portrait";
            VisualStateManager.GoToState(mainLayout, visualState);
            VisualStateManager.GoToState(aboutIcon, visualState);
            VisualStateManager.GoToState(myListView, visualState);
            VisualStateManager.GoToState(entranceLayout, visualState);
        }
    }
}
