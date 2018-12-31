using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// 表示表单的 HTML 对象
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class Form : IHtmlObject, IList<IHtmlObject>
    {
        public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();
        public string SubmitOption { get; set; } = "return false";
        public string Id { get; set; } = "";

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append($"<form class=\"setting-form-group\" onsubmit=\"{SubmitOption}\">");
            Children.ForEach((obj) => obj.ToHtml(sb));
            sb.Append("</form>");
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
