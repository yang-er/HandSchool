using System.Text;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// 表示原始 HTML 代码
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class RawHtml : IHtmlObject
    {
        public string Raw { get; set; } = string.Empty;
        public string Id => "";

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append(Raw);
        }

        public static RawHtml operator +(RawHtml a, string b)
        {
            a.Raw += b;
            return a;
        }

        public static RawHtml operator +(string a, RawHtml b)
        {
            b.Raw = a + b.Raw;
            return b;
        }
    }
}
