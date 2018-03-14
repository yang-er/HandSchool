using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 山宏岳
    public class PopContentPage : ContentPage
	{
        public PopContentPage()
        {
            Disappearing += Page_Disappearing;
        }
        
        private Task ContinueTask { get; } = new Task(() => { });

        public Task ShowAsync(INavigation navigation = null)
        {
            if(navigation is null)
            {
                App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
            }
            else
            {
                navigation.PushAsync(this);
            }
            return ContinueTask;
        }

        public void Close()
        {
            Navigation.PopAsync();
        }

        private bool _destoried;

        private void Page_Disappearing(object sender, EventArgs e)
        {
            if (_destoried)
            {
                return;
            }
            _destoried = true;
            Disappearing -= Page_Disappearing;

            ContinueTask?.Start();
        }
    }
}