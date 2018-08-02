using HandSchool.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : PopContentPage
	{
        public IndexPage()
		{
            InitializeComponent();
            ViewModel = IndexViewModel.Instance;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (IsBusy) return;

            IsBusy = true;
            await Core.App.Service.RequestLogin();
            IsBusy = false;
        }
    }
}