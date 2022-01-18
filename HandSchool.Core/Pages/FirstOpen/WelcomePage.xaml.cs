using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ViewObject
    {
        public const string FinishSignal = "HandSchool.SelectType.FinishAction";

        public WelcomePage()
        {
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:return;
                case Device.Android:
                    enter_main.IsVisible = false;
                    Appearing += (s, e) =>
                    {
                        System.Threading.Tasks.Task.Delay(2000).ContinueWith((a) => enter_main_clicked(s, e));
                    };

                    return;
            }
        }
        protected virtual void enter_main_clicked(object s,System.EventArgs e)
        {
            MessagingCenter.Send(this, FinishSignal);
        }
    }
}