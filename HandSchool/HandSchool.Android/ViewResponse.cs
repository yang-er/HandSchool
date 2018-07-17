using Android.App;
using HandSchool.Droid;
using System;
using System.Threading.Tasks;
using XPage = Xamarin.Forms.Page;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        private Action alertCallback;

        public ViewResponse(XPage page)
        {
            Binding = page;
        }

        public XPage Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public void SetIsBusy(bool value, string tips)
        {
            if (value)
            {
                var dialog = new ProgressDialog(MainActivity.ActivityContext);
                dialog.SetTitle(tips);
                dialog.SetMessage(tips);
                dialog.Indeterminate = true;
                dialog.Show();
                alertCallback = dialog.Dismiss;
            }
            else
            {
                alertCallback?.Invoke();
            }
        }
    }
}