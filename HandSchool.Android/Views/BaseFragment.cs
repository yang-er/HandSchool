using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using HandSchool.Internal;
using HandSchool.ViewModels;
using Microcharts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace HandSchool.Views
{
    public class ViewFragment : Fragment, IViewPage
    {
        /// <summary>
        /// 显示为Fragment的试图资源xml
        /// </summary>
        protected int FragmentViewResource { get; set; }

        /// <summary>
        /// 页面的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        public BaseViewModel ViewModel { get; set; }

        bool IViewPage.IsModal => false;

        Xamarin.Forms.View IViewPage.Content
        {
            get => throw new InvalidCastException();
            set => throw new InvalidCastException();
        }

        public void AddToolbarEntry(MenuEntry item)
        {
            throw new NotImplementedException();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(FragmentViewResource, container, false);
        }

        #region 页面生命周期

        /// <summary>
        /// 页面正在消失时
        /// </summary>
        public event EventHandler Disappearing;

        /// <summary>
        /// 页面正在出现时
        /// </summary>
        public event EventHandler Appearing;

        /// <summary>
        /// 视图导航控制器
        /// </summary>
        public INavigate Navigation { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Appearing?.Invoke(this, EventArgs.Empty);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Disappearing?.Invoke(this, EventArgs.Empty);
        }

        public void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }

        #endregion

        #region 视图的响应：通过 MessagingCenter 传递

        /// <summary>
        /// 弹出询问对话框，用作请求输入内容。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="description">弹出消息的正文。</param>
        /// <param name="cancel">取消按钮的文字。</param>
        /// <param name="accept">确认按钮的文字。</param>
        /// <returns>用户输入的内容，如果点击取消则为null。</returns>
        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            var args = new RequestInputArguments(title, description, cancel, accept);
            Core.Platform.ViewResponseImpl.ReqInpAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出选择对话框，从中选择一个操作。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="cancel">对话框的取消按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="destruction">对话框的删除按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="buttons">可选的动作列表每一项的文字。</param>
        /// <returns>按下的按钮标签文字。</returns>
        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            var args = new ActionSheetArguments(title, cancel, destruction, buttons);
            Core.Platform.ViewResponseImpl.ReqActAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出询问对话框，用作操作确认。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="description">弹出消息的正文。</param>
        /// <param name="cancel">取消按钮的文字。</param>
        /// <param name="accept">确认按钮的文字。</param>
        /// <returns>按下的是否为确定。</returns>
        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            var args = new AlertArguments(title, description, accept, cancel);
            Core.Platform.ViewResponseImpl.ReqMsgAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出消息对话框，用作消息提醒。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="message">弹出消息的正文。</param>
        /// <param name="button">确认按钮的文字。</param>
        public Task RequestMessageAsync(string title, string message, string button)
        {
            var args = new AlertArguments(title, message, null, button);
            Core.Platform.ViewResponseImpl.ReqMsgAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出图表对话框，用作展示图表。
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="title">对话框标题</param>
        /// <param name="close">关闭按钮文字</param>
        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            var args = new RequestChartArguments(chart, title, close);
            Core.Platform.ViewResponseImpl.ReqChtAsync(this, args);
            return args.ReturnTask;
        }

        #endregion
    }
}