using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
    {
		public MainPage()
		{
			InitializeComponent();

            Outline.PrimaryListView.ItemSelected += MasterPageItemSelected;
            Outline.SecondaryListView.ItemSelected += MasterPageItemSelected;
            
            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
            
            Detail = App.Current.PrimaryItems[0].DestPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(App.Current.Service.NeedLogin && !App.Current.Service.IsLogin)
            {
                (new LoginPage()).ShowAsync();
            }
        }

        private async void MasterPageItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                Detail = item.DestPage;
                
                (sender as ListView).SelectedItem = null;

                App.Current.PrimaryItems.ForEach((one) => { one.Selected = false; one.Color = Color.Black; });
                App.Current.SecondaryItems.ForEach((one) => { one.Selected = false; one.Color = Color.Black; });
                
                item.Selected = true;
                item.Color = App.Current.ActiveColor;

                // Funny fucky question: why this makes fluency?
                await Task.Run(() => { });
                IsPresented = false;
            }
        }
    }
}