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
            (Content as CollectionView).ItemsSource = Core.App.InfoEntrances;
        }

        bool IsPushing = false;

        public async void ItemTapped(object sender, System.EventArgs args)
        {
            var e = (sender as BindableObject)?.BindingContext;

            if (e is null || IsPushing)
                return;
            IsPushing = true;

            if (e is InfoEntranceWrapper iew)
            {
                await Navigation.PushAsync<IWebViewPage>(iew.Load.Invoke());
            }
            else if (e is TapEntranceWrapper tew)
            {
                await tew.Activate(Navigation);
            }

            IsPushing = false;
        }
    }
}