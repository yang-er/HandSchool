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
        }

        private void ViewObject_Appearing(object sender, EventArgs e)
        {
            Task.Delay(3000).ContinueWith(s => MessagingCenter.Send(this, FinishSignal));
        }
    }
}