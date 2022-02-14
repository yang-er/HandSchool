using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoQueryPage : ViewObject
	{
		public InfoQueryPage()
		{
            InitializeComponent();
            ViewModel = new BaseViewModel { Title = "其他功能" };
            Collection.ItemsSource = Core.App.InfoEntrances;
        }

        private bool _isPushing;

        public async void ItemTapped(object sender, CollectionItemTappedEventArgs args)
        {
            var e = args.Item;

            if (e is null || _isPushing)
                return;
            _isPushing = true;

            if (e is InfoEntranceWrapper iew)
            {
                await Navigation.PushAsync<IWebViewPage>(iew.Load.Invoke());
            }
            else if (e is TapEntranceWrapper tew)
            {
                await tew.Activate(Navigation);
            }

            _isPushing = false;
        }
    }
}