using System.Text;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// 网页第一个段落元素。
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class FirstPara : IHtmlObject
    {
        public string Id { get; } = string.Empty;
        public string Content { get; }
        public string AttachClass { get; }

        public FirstPara(string content, string attachClass = "")
        {
            Content = content;
            AttachClass = attachClass;
        }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append($"<p class=\"mt-3 {AttachClass}\">{Content}</p>");
        }
    }
}
