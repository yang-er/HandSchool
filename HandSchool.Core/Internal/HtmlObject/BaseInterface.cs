using System.Text;

namespace HandSchool.Internals
{
    /// <summary>
    /// HTML 对象
    /// </summary>
    public interface IHtmlObject
    {
        /// <summary>
        /// HTML 元素的 ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 将HTML对象转化为字符串。
        /// </summary>
        /// <param name="sb">输入的字符串缓冲区。</param>
        /// <param name="full">是否选择完整输出。</param>
        void ToHtml(StringBuilder sb, bool full = true);
    }
}
