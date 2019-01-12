using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using HandSchool.Forms;
using HandSchool.Views;
using SkiaSharp.Views.Android;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environment = System.Environment;

namespace HandSchool.Droid
{
    internal sealed class PlatformImpl : PlatformFormsImpl
    {
        /// <summary>
        /// 平台实现的实例。
        /// </summary>
        public static PlatformImpl Instance { get; private set; }

        /// <summary>
        /// 缩放尺寸
        /// </summary>
        private static float Scale { get; set; } = 1;

        /// <summary>
        /// 安卓系统的上下文
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// 更新管理器
        /// </summary>
        public UpdateManager UpdateManager { get; }

        /// <summary>
        /// 应用商店链接
        /// </summary>
        public override string StoreLink { get; }

        /// <summary>
        /// 运行时名称
        /// </summary>
        public override string RuntimeName => "Android";

        /// <summary>
        /// 设置文件夹
        /// </summary>
        public override string ConfigureDirectory { get; }

        /// <summary>
        /// 系统导航内容
        /// </summary>
        public List<NavMenuItemImpl> NavigationItems { get; }

        /// <summary>
        /// 初始化平台相关的参数。
        /// </summary>
        /// <param name="context">安卓系统的上下文</param>
        public PlatformImpl(Context context)
        {
            Context = context;
            Scale = context.Resources.DisplayMetrics.Density;
            StoreLink = "https://www.coolapk.com/apk/com.x90yang.HandSchool";
            ConfigureDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Core.InitPlatform(Instance = this);
            NavigationItems = new List<NavMenuItemImpl>();
            UpdateManager = new UpdateManager(context);
            UpdateManager.Update();
        }

        /// <summary>
        /// 添加菜单入口点到主要页面中。
        /// </summary>
        /// <param name="title">入口点菜单的标题。</param>
        /// <param name="dest">目标页面的类名称，将通过反射创建实例。</param>
        /// <param name="category">学校命名空间，如果为空默认为全局类。</param>
        /// <param name="uwp">UWP 的图标。</param>
        /// <param name="ios">iOS 系统展示的图标。为空时收起到信息查询中。</param>
        public override void AddMenuEntry(string title, string dest, string category, string uwp, string ios)
        {
            NavigationItems.Add(new NavMenuItemImpl(title, dest, category));
        }
        
        /// <summary>
        /// 检查应用程序更新。
        /// </summary>
        public override void CheckUpdate() => UpdateManager.Update();

        /// <summary>
        /// 输入文字请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public override void InputRequested(ViewPage sender, RequestInputArguments args)
        {
            var dismiss = new DismissByTaskSource(args.Result);
            var builder = new AlertDialog.Builder(Context);
            var textBox = new EditText(Context);
            var headerLabel = new TextView(Context);
            headerLabel.Text = args.Message;
            var layout = new StackView(Context);
            layout.AddView(headerLabel);
            layout.AddView(textBox);

            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Accept, delegate { dismiss.ResultCache = textBox.Text; });
            builder.SetNegativeButton(args.Cancel, delegate { });
            builder.SetOnDismissListener(dismiss);
            builder.SetCancelable(false);
            builder.SetView(layout);

            var dialog = builder.Create();
            dialog.Show();
        }

        /// <summary>
        /// 展示图表请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public override void ChartRequested(ViewPage sender, RequestChartArguments args)
        {
            var dbtask = new DismissByTask(args.ReturnTask);
            var builder = new AlertDialog.Builder(Context);

            args.Chart.LabelTextSize = Dip2Px(12);
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            var chartLayout = layoutInflater.Inflate(Resource.Layout.SkiaChart, null);
            builder.SetView(chartLayout);

            var canvasView = chartLayout.FindViewById<SKCanvasView>(Resource.Id.skia_chart_canvas);
            canvasView.PaintSurface += (s, e) =>
            {
                args.Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            };

            builder.SetTitle(args.Title);
            builder.SetPositiveButton(args.Close, delegate { });
            builder.SetOnDismissListener(dbtask);
            var dialog = builder.Create();
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
        /// 将dp值转换为px值
        /// </summary>
        /// <param name="dpValue">dp值</param>
        /// <returns>px值</returns>
        public static int Dip2Px(float dpValue) => (int)(dpValue * Scale + 0.5f);

        /// <summary>
        /// 将px值转换为dp值
        /// </summary>
        /// <param name="pxValue">px值</param>
        /// <returns>dp值</returns>
        public static float Px2Dip(int pxValue) => pxValue / Scale;

        /// <summary>
        /// 随着对话框消失的任务
        /// </summary>
        private class DismissByTask : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            public Task EndTask { get; }
            
            public DismissByTask(Task taskToWait)
            {
                EndTask = taskToWait;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                EndTask.Start();
            }
        }

        /// <summary>
        /// 随着对话框消失的任务
        /// </summary>
        private class DismissByTaskSource : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            public TaskCompletionSource<string> EndTask { get; }

            public string ResultCache { get; set; }

            public DismissByTaskSource(TaskCompletionSource<string> taskToWait)
            {
                EndTask = taskToWait;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                EndTask.TrySetResult(ResultCache);
            }
        }
    }
}
