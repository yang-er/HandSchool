using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
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
            PrimaryListView.ItemsSource = NavigationViewModel.Instance.PrimaryItems;
            SecondaryListView.ItemsSource = NavigationViewModel.Instance.SecondaryItems;
            SecondaryListView.HeightRequest = 12 + 48 * NavigationViewModel.Instance.SecondaryItems.Count;
            Core.App.Service.LoginStateChanged += (sender, e) => { if (e.State == LoginState.Succeeded) UpdateSideBar(); };
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
            currentMsg.Text = Core.App.Service.CurrentMessage;
            welcomeMsg.Text = Core.App.Service.WelcomeMessage;
        }
    }
}