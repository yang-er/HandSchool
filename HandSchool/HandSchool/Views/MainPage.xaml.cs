using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
    {
		public MainPage ()
		{
			InitializeComponent();

            Outline.PrimaryListView.ItemSelected += MasterPageItemSelected;
            Outline.SecondaryListView.ItemSelected += MasterPageItemSelected;
            
            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }

            Detail = new NavigationPage(Outline.PrimaryItems[0].DestPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(App.Current.Service.NeedLogin && !App.Current.Service.IsLogin)
            {
                (new LoginPage()).ShowAsync();
            }
        }

        private void MasterPageItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                foreach (MasterPageItem mpi in Outline.PrimaryListView.ItemsSource)
                {
                    mpi.Selected = false;
                    mpi.Color = Color.Black;
                }

                foreach (MasterPageItem mpi in Outline.SecondaryListView.ItemsSource)
                {
                    mpi.Selected = false;
                    mpi.Color = Color.Black;
                }

                item.Selected = true;
                item.Color = Color.DeepSkyBlue;
                
                Detail = new NavigationPage(item.DestPage);

                Outline.PrimaryListView.SelectedItem = null;
                Outline.SecondaryListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}