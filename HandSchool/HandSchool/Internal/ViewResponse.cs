using System.Threading.Tasks;

namespace HandSchool.Internal
{
    /// <summary>
    /// 页面的响应
    /// </summary>
    public interface IViewResponse
    {
        /// <summary>
        /// 弹出消息对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">消息正文</param>
        /// <param name="button">按钮文字</param>
        Task ShowMessage(string title, string message, string button = "确认");

        /// <summary>
        /// 设置忙状态
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="tips">提示</param>
        void SetIsBusy(bool value, string tips = "");
    }
}
