using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using HandSchool.Internals;
using HandSchool.Views;
using SkiaSharp.Views.Android;
using System.Linq;
using Xamarin.Forms.Internals;

namespace HandSchool.Droid
{
    public class ViewResponseImpl : IViewResponseImpl
    {
        public void ReqActAsync(IViewPage sender, ActionSheetArguments args)
        {
            var builder = new AlertDialog.Builder(sender.ToContext());
            builder.SetTitle(args.Title);

            string[] items = args.Buttons.ToArray();
            builder.SetItems(items, (o, fargs) => args.Result.TrySetResult(items[fargs.Which]));
            
            if (args.Cancel != null)
                builder.SetPositiveButton(args.Cancel, (o, e) => args.Result.TrySetResult(args.Cancel));
            
            if (args.Destruction != null)
                builder.SetNegativeButton(args.Destruction, (o, e) => args.Result.TrySetResult(args.Destruction));
            
            AlertDialog dialog = builder.Create();
            builder.Dispose();
            
            dialog.SetCanceledOnTouchOutside(true);
            dialog.CancelEvent += (o, e) => args.SetResult(null);
            dialog.Show();
        }

        public void ReqChtAsync(IViewPage sender, RequestChartArguments args)
        {
            // Create and assign layout
            var builder = new AlertDialog.Builder(sender.ToContext());
            LayoutInflater layoutInflater = LayoutInflater.From(sender.ToContext());
            var chartLayout = layoutInflater.Inflate(Resource.Layout.dialog_chart, null);
            builder.SetView(chartLayout);
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Close, (IDialogInterfaceOnClickListener)null);

            // Set drawing content
            args.Chart.LabelTextSize = sender.ToContext().Dip2Px(12);
            var canvasView = chartLayout.FindViewById<SKCanvasView>(Resource.Id.skia_chart_canvas);
            canvasView.PaintSurface += (s, e) =>
            {
                args.Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            };

            // Show and fix position
            var dialog = builder.Create();
            dialog.DismissEvent += (s, e) => args.ReturnTask.Start();
            dialog.Show();

            IWindowManager manager = ((BaseActivity)sender.ToContext()).WindowManager;
            Display d = manager.DefaultDisplay;
            Window window = dialog.Window;
            WindowManagerLayoutParams param = window.Attributes;
            param.Height = sender.ToContext().Dip2Px(340);
            param.Gravity = GravityFlags.CenterHorizontal;
            dialog.Window.Attributes = param;
        }

        public void ReqInpAsync(IViewPage sender, RequestInputArguments args)
        {
            var builder = new AlertDialog.Builder(sender.ToContext());
            string answer = null;

            LayoutInflater layoutInflater = LayoutInflater.From(sender.ToContext());
            var inputLayout = layoutInflater.Inflate(Resource.Layout.dialog_input, null);
            builder.SetView(inputLayout);

            var textBox = inputLayout.FindViewById<EditText>(Resource.Id.input_layout_edittext);
            var headerLabel = inputLayout.FindViewById<TextView>(Resource.Id.input_layout_textview);
            headerLabel.Text = args.Message;

            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Accept, (s, e) => answer = textBox.Text);
            builder.SetNegativeButton(args.Cancel, (IDialogInterfaceOnClickListener)null);
            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) =>
            {
                args.SetResult(answer);
            };

            dialog.ShowEvent += (s, e) =>
            {
                var imm = (InputMethodManager)sender.ToContext().GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(textBox, ShowFlags.Forced);
            };

            dialog.Show();
        }

        public void ReqMsgAsync(IViewPage sender, AlertArguments args)
        {
            AlertDialog alert = new AlertDialog.Builder(sender.ToContext()).Create();
            alert.SetTitle(args.Title);
            alert.SetMessage(args.Message);

            if (args.Accept != null)
                alert.SetButton((int)DialogButtonType.Positive, args.Accept, (o, e) => args.SetResult(true));

            alert.SetButton((int)DialogButtonType.Negative, args.Cancel, (o, e) => args.SetResult(false));
            alert.CancelEvent += (o, e) => args.SetResult(false);
            alert.Show();
        }
    }
}