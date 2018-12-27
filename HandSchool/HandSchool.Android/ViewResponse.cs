using Android.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microcharts;
using SkiaSharp.Views.Android;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using XPage = Xamarin.Forms.Page;
using HSResource = HandSchool.Droid.Resource;

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

            LayoutInflater layoutInflater = LayoutInflater.From(context);
            var chartLayout = layoutInflater.Inflate(HSResource.Layout.SkiaChart, null);
            builder.SetView(chartLayout);

            var canvasView = chartLayout.FindViewById<SKCanvasView>(HSResource.Id.skia_chart_canvas);
            canvasView.PaintSurface += (sender, args) =>
            {
                chart.Draw(args.Surface.Canvas, args.Info.Width, args.Info.Height);
            };
            
            builder.SetTitle(title);
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
