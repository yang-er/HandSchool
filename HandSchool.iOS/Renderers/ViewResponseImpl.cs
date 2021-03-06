﻿using HandSchool.Internals;
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
            var alert = UIAlertController.Create(args.Title, args.Message, UIAlertControllerStyle.Alert);
            alert.AddTextField((field) => field.Placeholder = "");
            alert.AddAction(UIAlertAction.Create(args.Accept, UIAlertActionStyle.Default, (s) => args.SetResult(alert.TextFields[0].Text)));
            alert.AddAction(UIAlertAction.Create(args.Cancel, UIAlertActionStyle.Cancel, (s) => args.SetResult(null)));
            MessagingCenter.Send((object)this, UIViewControllerRequest, alert);
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
            MessagingCenter.Send((object)this, UIViewControllerRequest, ca);
        }

        public void ReqMsgAsync(IViewPage sender, AlertArguments args)
        {
            MessagingCenter.Send(sender as Page, Page.AlertSignalName, args);
        }

        public void ReqActAsync(IViewPage sender, ActionSheetArguments args)
        {
            MessagingCenter.Send(sender as Page, Page.ActionSheetSignalName, args);
        }
    }
}
