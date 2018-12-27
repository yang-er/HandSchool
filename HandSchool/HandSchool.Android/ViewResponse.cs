using Android.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microcharts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using XPage = Xamarin.Forms.Page;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        public ViewResponse(XPage page)
        {
            Binding = page;
        }

        public XPage Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public Task<bool> ShowAskMessage(string title, string description, string cancel, string accept)
        {
            return Binding.DisplayAlert(title, description, accept, cancel);
        }

        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Binding.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public static Task ShowChartDialog(Chart chart, string title = "", string close = "关闭")
        {
            var dbtask = new DismissByTask();
            Context context = Droid.MainActivity.ActivityContext;
            var builder = new AlertDialog.Builder(context);
            var chartView = new Microcharts.Forms.ChartView();
            chartView.Chart = chart;
            chartView.HeightRequest = 300;

            LinearLayout layout = new LinearLayout(context);
            int width = Droid.MainActivity.Dip2Px(370);
            int height = Droid.MainActivity.Dip2Px(220);
            int margin_left = Droid.MainActivity.Dip2Px(30 - Droid.MainActivity.Px2Dip((int)chartView.Chart.Margin));
            int margin_top = Droid.MainActivity.Dip2Px(10 - Droid.MainActivity.Px2Dip((int)chartView.Chart.Margin));
            var layoutParams = new LinearLayout.LayoutParams(width, height);
            layout.LayoutParameters = layoutParams;
            layout.SetPadding(margin_left, 0, margin_left, 0);
            layout.AddView(Platform.CreateRendererWithContext(chartView, context) as View);

            builder.SetTitle(title);
            builder.SetView(layout);
            builder.SetPositiveButton(close, (sender, args) => { });
            builder.SetOnDismissListener(dbtask);
            var dialog = builder.Create();
            dialog.Show();
            
            IWindowManager manager = Droid.MainActivity.Instance.WindowManager;
            Display d = manager.DefaultDisplay;
            Window window = dialog.Window;
            WindowManagerLayoutParams param = window.Attributes;
            param.Height = Droid.MainActivity.Dip2Px(340);
            param.Gravity = GravityFlags.CenterHorizontal;
            dialog.Window.Attributes = param;

            return dbtask.EndTask;
        }

        class DismissByTask : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            public Task EndTask { get; } = new Task(() => { });

            public void OnDismiss(IDialogInterface dialog)
            {
                EndTask.Start();
                Core.Log("OnDismiss()");
            }
        }
    }
}
