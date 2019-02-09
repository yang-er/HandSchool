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
            ViewModel = new BaseViewModel { Title = "信息查询" };
            (Content as ListView).ItemsSource = Core.App.InfoEntrances;
        }

        object LastItem = null;
        bool IsPushing = false;

        public async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem || IsPushing)
                return; IsPushing = true;

            if (Device.Idiom == TargetIdiom.Tablet)
                LastItem = e.Item;

            if (e.Item is InfoEntranceWrapper iew)
            {
                await Navigation.PushAsync<IWebViewPage>(iew.Load.Invoke());
            }
            else if (e.Item is TapEntranceWrapper tew)
            {
                await tew.Activate(Navigation);
            }

            IsPushing = false;
        }
    }
}