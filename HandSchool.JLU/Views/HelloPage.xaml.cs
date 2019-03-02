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

            if (param != null)
            {
                Loader = (Loader)param;
                nextButton.Clicked += NextRequested;
            }
            else
            {
                nextButton.IsVisible = false;
                settingsPanel.IsVisible = false;
            }
        }

        private void NextRequested(object sender, EventArgs args)
        {
            Loader.SaveSettings(new Loader.SettingsJSON
            {
                OutsideSchool = outOfSchool.IsToggled
            });

            Navigation.PushAsync<InitializePage>(Loader);
        }
    }
}