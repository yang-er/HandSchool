using System.Text;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// 表示按钮
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class Button : IHtmlObject
    {
        public string Title { get; set; } = " 提交 ";
        public string Color { get; set; } = "primary";
        public string Type { get; set; } = "type=\"submit\"";
        public string Id => "";

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            if (full) sb.Append("<div class=\"form-group\">");
            sb.Append($"<button {Type} class=\"btn btn-{Color}\">{Title}</button>");
            if (full) sb.Append("</div>");
        }
    }
}
