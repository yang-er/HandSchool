using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using HandSchool.Droid;
using HandSchool.Internals;
using HandSchool.ViewModels;
using HandSchool.Views;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace HandSchool.Droid
{
    /// <summary>
    /// 实现 <see cref="IViewPage"/> 的Android本机基础碎片。
    /// </summary>
    public class ViewFragment : Fragment, IViewPage, IViewLifecycle, IBindTarget
    {
        /// <summary>
        /// 创建一个视图碎片。
        /// </summary>
        public ViewFragment()
        {
            ToolbarMenu = new ToolbarMenuTracker
            {
                List = new ObservableCollection<MenuEntry>()
            };
            
            RetainInstance = true;
        }

        /// <summary>
        /// 创建一个视图碎片并初始化状态资源。
        /// </summary>
        /// <param name="layoutResId"></param>
        public ViewFragment(int layoutResId) : this()
        {
            FragmentViewResource = layoutResId;
        }

        /// <summary>
        /// 显示为Fragment的试图资源xml
        /// </summary>
        protected int FragmentViewResource { get; set; }

        /// <summary>
        /// 页面的标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        public virtual BaseViewModel ViewModel { get; set; }

        /// <summary>
        /// 工具栏的菜单
        /// </summary>
        public virtual ToolbarMenuTracker ToolbarMenu { get; }

        /// <summary>
        /// 是否为模态页面
        /// </summary>
        bool IViewPage.IsModal => false;
        
        /// <summary>
        /// 添加工具栏菜单。
        /// </summary>
        /// <param name="item">菜单</param>
        public void AddToolbarEntry(MenuEntry item)
        {
            ToolbarMenu.List.Add(item);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(FragmentViewResource, container, false);
        }
        
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.SolveView(view);
        }

        public virtual bool IsBusy { get; set; }

        #region 页面生命周期

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            SendAppearing();
        }

        public override void OnDetach()
        {
            base.OnDetach();
            SendDisappearing();
        }

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

        public ToolbarMenuTracker ToolbarTracker => ToolbarMenu;

        /// <summary>
        /// 注册视图导航控制器。
        /// </summary>
        /// <param name="navigate">视图导航控制器</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }

        /// <summary>
        /// 设置导航参数。
        /// </summary>
        /// <param name="param">导航参数</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SetNavigationArguments(object param) { }

        public virtual void SendDisappearing()
        {
            Disappearing?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SendAppearing()
        {
            Appearing?.Invoke(this, EventArgs.Empty);
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

        public virtual void SolveBindings() { }
    }
}