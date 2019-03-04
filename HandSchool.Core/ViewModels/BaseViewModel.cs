using HandSchool.Internals;
using HandSchool.Views;
using Microcharts;
using System.Threading.Tasks;
using HandSchool.Design;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 提供了视图模型的基类。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="IViewResponse" />
    public class BaseViewModel : NotifyPropertyChanged, IViewResponse, IBusySignal
    {
        bool isBusy = false;
        string sTitle = string.Empty;
        
        /// <summary>
        /// 视图模型是否处于忙碌状态。
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, nameof(IsBusy));
        }
        
        /// <summary>
        /// 视图显示的窗口标题。
        /// </summary>
        public string Title
        {
            get => sTitle;
            set => SetProperty(ref sTitle, value);
        }
        
        /// <summary>
        /// ViewModel 绑定的 View 内容。
        /// </summary>
        public IViewResponse View { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        protected ILogger<BaseViewModel> Logger { get; set; }

        /// <summary>
        /// 弹出选择对话框，从中选择一个操作。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="cancel">对话框的取消按钮文字。为 null 时不显示按钮。</param>
        /// <param name="destruction">对话框的删除按钮文字。为 null 时不显示按钮。</param>
        /// <param name="buttons">可选的动作列表每一项的文字。</param>
        /// <returns>按下的按钮标签文字。</returns>
        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return View?.RequestActionAsync(title, cancel, destruction, buttons) ?? Task.FromResult<string>(null);
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
            return View?.RequestAnswerAsync(title, description, cancel, accept) ?? Task.FromResult(false);
        }

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
            return View?.RequestInputAsync(title, description, cancel, accept) ?? Task.FromResult<string>(null);
        }

        /// <summary>
        /// 弹出消息对话框，用作消息提醒。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="message">弹出消息的正文。</param>
        /// <param name="button">确认按钮的文字。</param>
        public Task RequestMessageAsync(string title, string message, string button = "知道了")
        {
            return View?.RequestMessageAsync(title, message, button) ?? Task.CompletedTask;
        }

        /// <summary>
        /// 弹出图表对话框，用作展示图表。
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="title">对话框标题</param>
        /// <param name="close">关闭按钮文字</param>
        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            return View?.RequestChartAsync(chart, title, close) ?? Task.CompletedTask;
        }
    }
}