using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    // Thanks to 山宏岳
    public class PopContentPage : ContentPage
	{
        private bool _destoried;
        private NavigationPage _navpg;

        public bool IsModal { get; set; } = false;
        private Task ContinueTask { get; } = new Task(() => { });
        public event Action Destorying;

        public Task ShowAsync(INavigation navigation = null)
        {
            if(navigation is null)
            {
                App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
                Disappearing += Page_Disappearing;
                IsModal = true;
            }
            else
            {
                navigation.PushAsync(this);
                if (Parent is NavigationPage)
                {
                    _navpg = Parent as NavigationPage;
                    _navpg.Popped += Page_Popped;
                }
                else
                {
                    throw new NotSupportedException("This kind of access");
                }
            }
            return ContinueTask;
        }

        public async Task Close()
        {
            if (IsModal)
                await Navigation.PopModalAsync();
            else
                await Navigation.PopAsync();
        }
        
        private void Page_Disappearing(object sender, EventArgs e)
        {
            if (_destoried) return;
            _destoried = true;

            Disappearing -= Page_Disappearing;
            Destorying.Invoke();

            ContinueTask?.Start();
        }

        private void Page_Popped(object sender, NavigationEventArgs e)
        {
            if (_destoried) return;
            _destoried = true;
            
            _navpg.Popped -= Page_Popped;
            Destorying.Invoke();

            ContinueTask?.Start();
        }
    }
}