using System.Threading.Tasks;
using Microcharts;
using UIKit;
using XPage = Xamarin.Forms.Page;
using Xamarin.Forms.Platform.iOS;
using System;
using SkiaSharp.Views.iOS;

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

        public void SetIsBusy(bool value, string tips)
        {
            Binding.IsBusy = value;
        }

        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Binding.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public static Task ShowChartDialog(Chart chart, string title = "", string close = "关闭")
        {
            var finishTask = new Task(() => { });
            var ca = UIAlertController.Create(title, "\n\n\n\n\n\n\n\n\n\n\n", UIAlertControllerStyle.Alert);
            var alertClose = UIAlertAction.Create(close, UIAlertActionStyle.Default, (aa) => finishTask.Start());
            ca.AddAction(alertClose);

            var chartView = new SKCanvasView(new CoreGraphics.CGRect(25, 50, 225, 180));
            chartView.BackgroundColor = UIColor.Black;
            chartView.PaintSurface += (sender, args) =>
            {
                args.Surface.Canvas.Clear(SkiaSharp.SKColor.Parse("#f8f8f8"));
                chart.DrawContent(args.Surface.Canvas, args.Info.Width, args.Info.Height);
            };

            ca.View.AddSubview(chartView);
            iOS.MainPageRenderer.GlobalViewController.PresentViewController(ca, true, null);
            return finishTask;
        }
    }
}
