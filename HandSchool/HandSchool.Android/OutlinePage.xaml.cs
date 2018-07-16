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
            LayoutChanged += WSizeChanged;
        }

        private void WSizeChanged(object sender, EventArgs e)
        {
#if __ANDROID__
            infoBar.HeightRequest = Width * 0.625;
            stackOfInfo.Margin = new Thickness(20, Width * 0.625 - 70, 0, 0);
#endif
        }

        public void UpdateSideBar()
        {
            currentMsg.Text = Core.App.Service.CurrentMessage;
            welcomeMsg.Text = Core.App.Service.WelcomeMessage;
        }
    }
}