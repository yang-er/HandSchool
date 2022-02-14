using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.Internals;
using System.Threading.Tasks;
using HandSchool.Internal;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ViewObject
	{
        public new AboutViewModel ViewModel
        {
            get => BindingContext as AboutViewModel;
            set => base.ViewModel = value;
        }

        InfoEntranceGroup AboutEntrances;

        public AboutPage()
        {
            InitializeComponent();
            On<_iOS_>().UseSafeArea();

            ViewModel = AboutViewModel.Instance;
            myListView.HeightRequest = 51 * 4;
            
            AboutEntrances = new InfoEntranceGroup { GroupTitle = "关于" };
            AboutEntrances.Add(new TapEntranceWrapper("开源项目", "", (nav) => Task.Run(() => ViewModel.OpenSource())));
            AboutEntrances.Add(new TapEntranceWrapper("软件评分", "", (nav) => Task.Run(() => ViewModel.OpenMarket())));
            AboutEntrances.Add(InfoEntranceWrapper.From<AboutViewModel.PrivacyPolicy>());
            AboutEntrances.Add(InfoEntranceWrapper.From<AboutViewModel.LicenseInfo>());
            myListView.ItemsSource = AboutEntrances;
        }

        async void ItemTapped(object sender, CollectionItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is InfoEntranceWrapper iew)
            {
                await Navigation.PushAsync<WebViewPage>(iew.Load.Invoke());
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
