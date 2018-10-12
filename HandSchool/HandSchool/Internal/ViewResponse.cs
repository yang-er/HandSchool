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
        /// 弹出询问框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="description">消息正文</param>
        /// <param name="cancel">取消按钮文字</param>
        /// <param name="accept">确定按钮文字</param>
        /// <returns>是否确定</returns>
        Task<bool> ShowAskMessage(string title, string description, string cancel, string accept);

        /// <summary>
        /// 弹出动作列表
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="cancel">取消</param>
        /// <param name="destruction">删除</param>
        /// <param name="buttons">按钮</param>
        /// <returns>按下的按钮标签</returns>
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
    }
}
