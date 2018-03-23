using Android.App;
using Android.Content;
using System;

namespace HandSchool.Internal
{
    partial class Helper
    {
        public static Context AndroidContext;

        public static Action ShowLoadingAlert(string tips, string title = "提示")
        {
            var dialog = new ProgressDialog(AndroidContext);
            dialog.SetTitle(title);
            dialog.SetMessage(tips);
            dialog.Indeterminate = true;
            dialog.Show();
            return () => dialog.Dismiss();
        }
    }
}