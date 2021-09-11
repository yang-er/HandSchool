using System.IO;
using System.Threading.Tasks;
using Foundation;
using HandSchool.Internals;
using HandSchool.Views;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.iOS
{
    public sealed class ViewResponseImpl : IViewResponseImpl
    {
        public const string UIViewControllerRequest = "HandSchool.iOS.UIViewControllerRequest";
        const string ChartPlaceHolder = "\n\n\n\n\n\n\n\n\n\n\n";

        public void ReqInpAsync(IViewPage sender, RequestInputArguments args)
        {
            var controller = UIAlertController.Create(args.Title, args.Message, UIAlertControllerStyle.Alert);
            controller.AddTextField((h)=> { });
            var cancel = UIAlertAction.Create(args.Cancel, UIAlertActionStyle.Default, (e) => { args.Result.SetResult(null); });
            controller.AddAction(cancel);
            var accept = UIAlertAction.Create(args.Accept, UIAlertActionStyle.Destructive, (e) => { args.Result.SetResult(string.IsNullOrWhiteSpace(controller.TextFields[0].Text) ? null : controller.TextFields[0].Text); });
            controller.AddAction(accept);
            (UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController.PresentViewController(controller, true, null);
        }

        public void ReqChtAsync(IViewPage sender, RequestChartArguments args)
        {
            var ca = UIAlertController.Create(args.Title, ChartPlaceHolder, UIAlertControllerStyle.Alert);
            var alertClose = UIAlertAction.Create(args.Close, UIAlertActionStyle.Default, a => args.ReturnTask.Start());
            ca.AddAction(alertClose);

            var chartView = new SKCanvasView(new CoreGraphics.CGRect(25, 50, 225, 180));

            chartView.PaintSurface += (s, e) =>
            {
                e.Surface.Canvas.Clear(SkiaSharp.SKColor.Parse("#f8f8f8"));
                args.Chart.DrawContent(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            };

            ca.View.AddSubview(chartView);
            (UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController.PresentViewController(ca, true, null);

        }

        public void ReqMsgAsync(IViewPage sender, AlertArguments args)
        {
            MessagingCenter.Send(sender as Page, Page.AlertSignalName, args);
        }

        public void ReqActAsync(IViewPage sender, ActionSheetArguments args)
        {
            MessagingCenter.Send(sender as Page, Page.ActionSheetSignalName, args);
        }

        //一个函数用来获取缩放后的图片大小以及要用到的占位空行
        static (double with, double heigth,string blank) ImageSizeConvert(CoreGraphics.CGSize size)
        {
            var heigth = (250.0 / size.Width) * size.Height;
            var n = heigth / 13.9;
            var sb = new System.Text.StringBuilder();
            for (var i = 0; n - i > 0.5; i++)
                sb.Append('\n');
            return (250, heigth, sb.ToString());
        }

        public void ReqInpWPicAsync(IViewPage sender, RequestInputWithPicArguments args)
        {
            if (args.Sources == null) return;
            var data = NSData.FromArray(args.Sources);
            var uiimage = UIImage.LoadFromData(data);
            var size = ImageSizeConvert(uiimage.Size);

            var controller = UIAlertController.Create(args.Title, args.Message + size.blank, UIAlertControllerStyle.Alert);
            
            var view = new UIImageView(frame: new CoreGraphics.CGRect(10, 80, size.Item1, size.Item2)); ;
            view.Image = uiimage;
            
            controller.View.AddSubview(view);

            controller.AddTextField((h) => { });
            var cancel = UIAlertAction.Create(args.Cancel, UIAlertActionStyle.Default, (e) => { args.Result.SetResult(null); });
            controller.AddAction(cancel);
            var accept = UIAlertAction.Create(args.Accept, UIAlertActionStyle.Destructive, (e) => { args.Result.SetResult(string.IsNullOrWhiteSpace(controller.TextFields[0].Text) ? null : controller.TextFields[0].Text); });
            controller.AddAction(accept);
            (UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController.PresentViewController(controller, true, null);
        }

        public void ReqWebDiaAsync(IViewPage sender, RequestWebDialogArguments args, WebDialogAdditionalArgs additionalArgs)
        {
            throw new System.NotImplementedException();
        }
    }
}
