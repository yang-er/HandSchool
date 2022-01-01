using Android.App;
using Android.Content;
using Android.Graphics;
using AppActivity = AndroidX.AppCompat.App.AppCompatActivity;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using HandSchool.Internals;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using HtmlAgilityPack;
using SkiaSharp.Views.Android;
using System;
using System.Linq;
using System.Text;
using System.Threading;
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
            var builder = new AlertDialog.Builder(context);
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
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new AlertDialog.Builder(context);
            LayoutInflater layoutInflater = LayoutInflater.From(sender.ToContext());
            var chartLayout = layoutInflater.Inflate(Resource.Layout.dialog_chart, null);
            builder.SetView(chartLayout);
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Close, (IDialogInterfaceOnClickListener)null);

            // Set drawing content
            args.Chart.LabelTextSize = Core.Platform.Dip2Px(12);
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
            param.Height = Core.Platform.Dip2Px(340);
            param.Gravity = GravityFlags.CenterHorizontal;
            dialog.Window.Attributes = param;
        }

        public void ReqInpAsync(IViewPage sender, RequestInputArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new AlertDialog.Builder(context);
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

        public void ReqInpWPicAsync(IViewPage sender, RequestInputWithPicArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new AlertDialog.Builder(context);
            string answer = null;

            var image_input = View.Inflate(context, Resource.Layout.ask_for_code, null) as LinearLayout;
            builder.SetView(image_input);

            var msg = image_input.FindViewById(Resource.Id.askcode_msg) as TextView;
            msg.Text = args.Message;

            var img = image_input.FindViewById(Resource.Id.code_img) as ImageView;
            img.SetImageBitmap(BitmapFactory.DecodeByteArray(args.Sources, 0, args.Sources.Length));

            var edit = image_input.FindViewById(Resource.Id.code_input) as EditText;
            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Accept, (s, e) => answer = edit.Text);
            builder.SetNegativeButton(args.Cancel, listener: null);
            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) =>
            {
                args.SetResult(answer);
            };

            dialog.ShowEvent += (s, e) =>
            {
                var imm = (InputMethodManager)sender.ToContext().GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(edit, ShowFlags.Forced);
            };

            dialog.Show();
        }

        public void ReqMsgAsync(IViewPage sender, AlertArguments args)
        {
            var context = sender.ToContext();
            if (context is null) return;
            AlertDialog alert = new AlertDialog.Builder(context).Create();
            alert.SetTitle(args.Title);
            alert.SetMessage(args.Message);

            if (args.Accept != null)
                alert.SetButton((int)DialogButtonType.Positive, args.Accept, (o, e) => args.SetResult(true));

            alert.SetButton((int)DialogButtonType.Negative, args.Cancel, (o, e) => args.SetResult(false));
            alert.CancelEvent += (o, e) => args.SetResult(false);
            alert.Show();
        }
        
        public void ReqWebDiaAsync(IViewPage sender, RequestWebDialogArguments args, WebDialogAdditionalArgs additionalArgs)
        {
            var context = sender.ToContext();
            if (context is null) return;
            var builder = new AlertDialog.Builder(context);
            string answer = null;

            //获取页面
            var webView = View.Inflate(context, Resource.Layout.web_dialog, null) as LinearLayout;
            builder.SetView(webView);
            

            var msg = webView.FindViewById(Resource.Id.web_dia_text) as TextView;
            if (args.Message == null || args.Message.Length == 0)
                msg.Visibility = ViewStates.Gone;
            msg.Text = args.Message;

            //找到WebView
            var browser = webView.FindViewById(Resource.Id.web_dia_browser) as WebView;
            browser.LoadUrl(args.Url);
            browser.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
            browser.Settings.JavaScriptEnabled = true;
            
            if (additionalArgs != null)
            {
                var addArgs = (AndroidWebDialogAdditionalArgs)additionalArgs;
                browser.SetWebChromeClient(addArgs.WebChromeClient);
                browser.SetWebViewClient(addArgs.WebViewClient);
                var cm = CookieManager.Instance;
                foreach(var item in addArgs.Cookies)
                {
                    var pre = args.Url.ToLower().StartsWith("https://") ? "https://" : "http://";
                    cm.SetCookie(pre + item.Domain + item.Path, item.ToString());
                }
            }
            var edit = webView.FindViewById(Resource.Id.web_dia_resp) as EditText;
            if (!args.HasInput)
            {
                edit.Visibility = ViewStates.Gone;
            }
            else
            {
                edit.Hint = args.InputHint;
            }

            var url = webView.FindViewById(Resource.Id.web_dia_url_text) as TextView;
            url.Text = "此页面由" + args.Url + "提供";

            var goback = webView.FindViewById(Resource.Id.web_dia_goback) as Button;
            var goforward = webView.FindViewById(Resource.Id.web_dia_goforward) as Button;
            var relod = webView.FindViewById(Resource.Id.web_dia_reload) as ImageButton;
            relod.Click += (s, e) =>
            {
                var web = ((View)((View)s).Parent.Parent).FindViewById(Resource.Id.web_dia_browser) as WebView;
                web.Reload();
            };
            if (args.Navigation)
            {
                goback.Click += (s, e) =>
                {
                    var web = ((View)((Button)s).Parent.Parent).FindViewById(Resource.Id.web_dia_browser) as WebView;
                    if (web.CanGoBack()) web.GoBack();
                };
                goforward.Click += (s, e) =>
                {
                    var web = ((View)((Button)s).Parent.Parent).FindViewById(Resource.Id.web_dia_browser) as WebView;
                    if (web.CanGoForward()) web.GoForward();
                };
                
            }
            else
            {
                relod.Visibility = ViewStates.Visible;
                goforward.Visibility = goback.Visibility = ViewStates.Gone;
            }
            builder.SetTitle(args.Title);
            if (args.Accept != null && args.Accept != "")
            {
                builder.SetPositiveButton(args.Accept, (s, e) =>
                {
                    if (args.HasInput) answer = edit.Text;
                    browser.LoadUrl("about:blank");
                    browser.Dispose();
                });
            }
            if (args.Cancel != null && args.Cancel != "")
            {
                builder.SetNegativeButton(args.Cancel, (s,e)=> { 
                    browser.LoadUrl("about:blank");
                    browser.Dispose(); 
                });
            }
            builder.SetCancelable(false);

            var dialog = builder.Create();

            dialog.DismissEvent += (s, e) =>
            {
                args.SetResult(answer);
            };
            dialog.Show();
        }
    }
}