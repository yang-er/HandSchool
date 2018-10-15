using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
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
            ShowIsBusyDialog = true;
#if __IOS__
            var themer = new Style(typeof(Frame));
            themer.Setters.Add(Frame.HasShadowProperty, false);
            Resources.Add(themer);
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetLargeTitleDisplay(this, Xamarin.Forms.PlatformConfiguration.iOSSpecific.LargeTitleDisplayMode.Always);
#endif
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