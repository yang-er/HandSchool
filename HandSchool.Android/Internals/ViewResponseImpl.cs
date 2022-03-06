using Android.Content;
using Android.Graphics;
using AppActivity = AndroidX.AppCompat.App.AppCompatActivity;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using HandSchool.Internals;
using HandSchool.Views;
using SkiaSharp.Views.Android;
using System.Linq;
using Google.Android.Material.Dialog;
using Xamarin.Forms.Internals;
using HandSchool.Droid.Internals;

namespace HandSchool.Droid
{
    public class ViewResponseImpl : IViewResponseImpl
    {
        public void ReqActAsync(IViewPage sender, ActionSheetArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded);
            builder.SetTitle(args.Title);

            var items = args.Buttons.ToArray();
            builder.SetItems(items, (o, selected) => args.Result.TrySetResult(items[selected.Which]));
            if (args.Cancel != null)
                builder.SetPositiveButton(args.Cancel, (o, e) => args.Result.TrySetResult(args.Cancel));

            if (args.Destruction != null)
                builder.SetNegativeButton(args.Destruction, (o, e) => args.Result.TrySetResult(args.Destruction));

            var dialog = builder.Create();
            builder.Dispose();

            dialog.SetCanceledOnTouchOutside(true);
            dialog.CancelEvent += (o, e) => args.SetResult(args.Cancel);
            dialog.Show();
        }

        public void ReqChtAsync(IViewPage sender, RequestChartArguments args)
        {
            // Create and assign layout
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded);

            var chartLayout = View.Inflate(context, Resource.Layout.dialog_chart, null);
            builder.SetView(chartLayout);
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Close, listener: null);

            // Set drawing content
            args.Chart.LabelTextSize = Core.Platform.Dip2Px(12);
            var canvasView = chartLayout.FindViewById<SKCanvasView>(Resource.Id.skia_chart_canvas);
            canvasView.PaintSurface += (s, e) => { args.Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height); };

            // Show and fix position
            var dialog = builder.Create();
            dialog.DismissEvent += (s, e) => args.ReturnTask.Start();
            dialog.Show();

            var attr = dialog.Window.Attributes;
            attr.Height = Core.Platform.Dip2Px(430);
            attr.Gravity = GravityFlags.CenterHorizontal;
            dialog.Window.Attributes = attr;
        }

        public void ReqInpAsync(IViewPage sender, RequestInputArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded);
            string answer = null;

            LayoutInflater layoutInflater = LayoutInflater.From(sender.ToContext());
            var inputLayout = layoutInflater.Inflate(Resource.Layout.dialog_input, null);
            builder.SetView(inputLayout);

            var textBox = inputLayout.FindViewById<EditText>(Resource.Id.input_layout_edittext);
            var headerLabel = inputLayout.FindViewById<TextView>(Resource.Id.input_layout_textview);
            headerLabel.Text = args.Message;

            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Accept, (s, e) => answer = textBox.Text);
            builder.SetNegativeButton(args.Cancel, (IDialogInterfaceOnClickListener) null);
            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) => { args.SetResult(answer); };

            dialog.ShowEvent += (s, e) =>
            {
                var imm = (InputMethodManager) sender.ToContext().GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(textBox, ShowFlags.Forced);
            };

            dialog.Show();
        }

        public void ReqInpWPicAsync(IViewPage sender, RequestInputWithPicArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded);
            string answer = null;

            var imageInput = View.Inflate(context, Resource.Layout.ask_for_code, null) as LinearLayout;
            builder.SetView(imageInput);

            var msg = imageInput.FindViewById(Resource.Id.askcode_msg) as TextView;
            msg.Text = args.Message;

            var img = imageInput.FindViewById(Resource.Id.code_img) as ImageView;
            img.SetImageBitmap(BitmapFactory.DecodeByteArray(args.Sources, 0, args.Sources.Length));

            var edit = imageInput.FindViewById(Resource.Id.code_input) as EditText;
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Accept, (s, e) => answer = edit.Text);
            builder.SetNegativeButton(args.Cancel, listener: null);
            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) => { args.SetResult(answer); };

            dialog.ShowEvent += (s, e) =>
            {
                var imm = (InputMethodManager) sender.ToContext().GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(edit, ShowFlags.Forced);
            };

            dialog.Show();
        }

        public void ReqMsgAsync(IViewPage sender, AlertArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var alert = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded).Create();
            alert.SetTitle(args.Title);
            alert.SetMessage(args.Message);

            if (args.Accept != null)
                alert.SetButton((int) DialogButtonType.Positive, args.Accept, (o, e) => args.SetResult(true));

            alert.SetButton((int) DialogButtonType.Negative, args.Cancel, (o, e) => args.SetResult(false));
            alert.CancelEvent += (o, e) => args.SetResult(false);
            alert.Show();
        }

        public void ReqWebDiaAsync(IViewPage sender, RequestWebDialogArguments args,
            WebDialogAdditionalArgs additionalArgs)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new MaterialAlertDialogBuilder(context, Resource.Style.MaterialAlertDialog_Rounded);
            string answer = null;

            //获取页面
            var linearLayout = View.Inflate(context, Resource.Layout.web_dialog, null) as LinearLayout;
            builder.SetView(linearLayout);


            var msg = linearLayout.FindViewById<TextView>(Resource.Id.web_dia_text);
            if (args.Message.IsBlank())
                msg.Visibility = ViewStates.Gone;
            msg.Text = args.Message;

            //找到WebView
            var webView = linearLayout.FindViewById<WebView>(Resource.Id.web_dia_browser);
            webView.LoadUrl(args.Url);
            webView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
            webView.Settings.JavaScriptEnabled = true;

            if (additionalArgs != null)
            {
                var addArgs = (AndroidWebDialogAdditionalArgs) additionalArgs;
                webView.SetWebChromeClient(addArgs.WebChromeClient);
                webView.SetWebViewClient(addArgs.WebViewClient);
                addArgs.Cookies?.Let(cookies =>
                {
                    foreach (var item in addArgs.Cookies)
                    {
                        var pre = args.Url.ToLower().StartsWith("https://") ? "https://" : "http://";
                        CookieManager.Instance!.SetCookie(pre + item.Domain + item.Path, item.ToString());
                    }
                });
            }

            var edit = linearLayout.FindViewById<EditText>(Resource.Id.web_dia_resp);
            if (!args.HasInput)
            {
                edit.Visibility = ViewStates.Gone;
            }
            else
            {
                edit.Hint = args.InputHint;
            }

            var url = linearLayout.FindViewById<TextView>(Resource.Id.web_dia_url_text);
            url.Text = "此页面由" + args.Url + "提供";

            var goBack = linearLayout.FindViewById<Button>(Resource.Id.web_dia_goback);
            var goForward = linearLayout.FindViewById<Button>(Resource.Id.web_dia_goforward);
            var reload = linearLayout.FindViewById<ImageButton>(Resource.Id.web_dia_reload);
            reload.Click += (s, e) => { webView.Reload(); };
            if (args.Navigation)
            {
                goBack.Click += (s, e) =>
                {
                    if (webView.CanGoBack()) webView.GoBack();
                };
                goForward.Click += (s, e) =>
                {
                    if (webView.CanGoForward()) webView.GoForward();
                };
            }
            else
            {
                reload.Visibility = ViewStates.Visible;
                goForward.Visibility = goBack.Visibility = ViewStates.Gone;
            }

            builder.SetTitle(args.Title);
            if (args.Accept.IsNotBlank())
            {
                builder.SetPositiveButton(args.Accept, (s, e) =>
                {
                    if (args.HasInput) answer = edit.Text;
                    webView.LoadUrl("about:blank");
                    webView.Dispose();
                });
            }

            if (args.Cancel.IsNotBlank())
            {
                builder.SetNegativeButton(args.Cancel, (s, e) =>
                {
                    webView.LoadUrl("about:blank");
                    webView.Dispose();
                });
            }

            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) => { args.SetResult(answer); };
            dialog.Show();
        }
    }
}