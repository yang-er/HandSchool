using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// 表示 Bootstrap 文档
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class Bootstrap : IHtmlObject, IList<IHtmlObject>
    {
        public string Title { get; set; } = "WebViewPage";
        public const string Charset = "utf-8";
        public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();
        public List<string> JavaScript { get; set; } = new List<string>();
        public string Css { get; set; } = "";
        public string Id => "";

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append($"<!doctype html><html><head><meta charset=\"{Charset}\"><base href=\"{{webview_base_url}}\"></base>");
            sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\">");
            sb.Append($"<link rel=\"stylesheet\" href=\"bootstrap.css\"><style>{Css}</style><title>{Title}</title></head>");
            sb.Append($"<body class=\"{Core.Platform.RuntimeName}\"><div class=\"container-fluid\">");
            Children.ForEach((obj) => obj.ToHtml(sb));
            sb.Append("</div><script src=\"jquery.js\"></script>");
            JavaScript.ForEach((obj) => sb.Append("<script>" + obj + "</script>"));
            if (Core.Platform.RuntimeName == "Android")
                sb.Append("<script>function invokeCSharpAction(data){jsBridge.invokeAction(data);}</script>");
            sb.Append("</body></html>");
        }

        IHtmlObject IList<IHtmlObject>.this[int index] { get => Children[index]; set => Children[index] = value; }
        int ICollection<IHtmlObject>.Count => Children.Count;
        bool ICollection<IHtmlObject>.IsReadOnly => ((ICollection<IHtmlObject>)Children).IsReadOnly;
        public void Add(IHtmlObject item) => Children.Add(item);
        void ICollection<IHtmlObject>.Clear() => Children.Clear();
        bool ICollection<IHtmlObject>.Contains(IHtmlObject item) => Children.Contains(item);
        void ICollection<IHtmlObject>.CopyTo(IHtmlObject[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        IEnumerator<IHtmlObject> IEnumerable<IHtmlObject>.GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
        int IList<IHtmlObject>.IndexOf(IHtmlObject item) => Children.IndexOf(item);
        void IList<IHtmlObject>.Insert(int index, IHtmlObject item) => Children.Insert(index, item);
        bool ICollection<IHtmlObject>.Remove(IHtmlObject item) => Children.Remove(item);
        void IList<IHtmlObject>.RemoveAt(int index) => Children.RemoveAt(index);
    }
}
