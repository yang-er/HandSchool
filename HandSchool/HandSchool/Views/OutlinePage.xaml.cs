using HandSchool.Internal;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutlinePage : ContentPage
	{
        public OutlinePage()
		{
			InitializeComponent();
            PrimaryListView.ItemsSource = App.Current.PrimaryItems;
            SecondaryListView.ItemsSource = App.Current.SecondaryItems;
            SecondaryListView.HeightRequest = 12 + 48 * App.Current.SecondaryItems.Count;
            App.Current.Service.LoggedIn += UpdateSideBar;
            UpdateSideBar();
            LayoutChanged += WSizeChanged;
        }

        private void WSizeChanged(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                infoBar.HeightRequest = Width * 0.625;
                stackOfInfo.Margin = new Thickness(20, Width * 0.625 - 70, 0, 0);
            }
        }

        public void UpdateSideBar()
        {
            currentMsg.Text = App.Current.Service.CurrentMessage;
            welcomeMsg.Text = App.Current.Service.WelcomeMessage;
        }
    }
}