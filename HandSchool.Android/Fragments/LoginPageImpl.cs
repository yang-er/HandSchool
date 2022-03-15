using System.Threading.Tasks;
using HandSchool.Droid;
using HandSchool.Internals;
using Xamarin.Forms;

[assembly:ExportRenderer(typeof(CheckBox), typeof(Xamarin.Forms.Platform.Android.CheckBoxRenderer))]

namespace HandSchool.Views
{
    public class LoginPageImpl : BaseLoginPage
    {
        public override Task ShowAsync()
        {
            var context  = PlatformImplV2.Instance.PeekAliveActivity();
            var navigate = context as INavigate;
            navigate.PushAsync<SecondActivity>(this);
            return Task.CompletedTask;
        }

        protected override Task CloseAsync()
        {
            PlatformImplV2.Instance.PeekAliveActivity().Finish();
            return Task.CompletedTask;
        }
    }
}