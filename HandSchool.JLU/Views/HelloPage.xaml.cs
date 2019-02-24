using HandSchool.Internals;
using HandSchool.Views;
using System;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelloPage : ViewObject
    {
        Loader Loader { get; set; }

        public HelloPage()
        {
            InitializeComponent();
        }

        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);
            Loader = (Loader)param;
        }

        private void NextRequested(object sender, EventArgs args)
        {
            Navigation.PushAsync<InitializePage>(Loader);
        }
    }
}