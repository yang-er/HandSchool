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

        public Task<bool> ShowActionSheet(string title, string description, string cancel, string accept)
        {
            return Binding.DisplayAlert(title, description, accept, cancel);
        }

        public void SetIsBusy(bool value, string tips)
        {
            if (value)
            {
                var dialog = new ProgressDialog(MainActivity.ActivityContext);
                dialog.SetTitle("请稍后");
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