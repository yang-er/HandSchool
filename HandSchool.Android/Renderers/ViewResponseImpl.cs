using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using HandSchool.Forms;
using HandSchool.Views;
using SkiaSharp.Views.Android;

namespace HandSchool.Droid
{
    public sealed partial class PlatformImpl : PlatformFormsImpl
    {
        /// <summary>
        /// 展示图表请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public override void ChartRequested(ViewPage sender, RequestChartArguments args)
        {
            // Create and assign layout
            var builder = new AlertDialog.Builder(Context);
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            var chartLayout = layoutInflater.Inflate(Resource.Layout.SkiaChart, null);
            builder.SetView(chartLayout);
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Close, (IDialogInterfaceOnClickListener)null);

            // Set drawing content
            args.Chart.LabelTextSize = Dip2Px(12);
            var canvasView = chartLayout.FindViewById<SKCanvasView>(Resource.Id.skia_chart_canvas);
            canvasView.PaintSurface += (s, e) =>
            {
                args.Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            };

            // Show and fix position
            var dialog = builder.Create();
            dialog.DismissEvent += (s, e) => args.ReturnTask.Start();
            dialog.Show();
            IWindowManager manager = MainActivity.Instance.WindowManager;
            Display d = manager.DefaultDisplay;
            Window window = dialog.Window;
            WindowManagerLayoutParams param = window.Attributes;
            param.Height = Dip2Px(340);
            param.Gravity = GravityFlags.CenterHorizontal;
            dialog.Window.Attributes = param;
        }

        /// <summary>
        /// 输入文字请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public override void InputRequested(ViewPage sender, RequestInputArguments args)
        {
            var builder = new AlertDialog.Builder(Context);
            string answer = null;

            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            var inputLayout = layoutInflater.Inflate(Resource.Layout.InputLayout, null);
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
                var imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(textBox, ShowFlags.Forced);
            };

            dialog.Show();
        }
    }
}