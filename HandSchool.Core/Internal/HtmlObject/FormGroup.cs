using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// 表示表单项外 div 的 HTML 元素
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class FormGroup : IHtmlObject, IList<IHtmlObject>
    {
        public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();
        public string Id => "";
        
        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append("<div class=\"form-group\">");
            Children.ForEach((obj) => obj.ToHtml(sb));
            sb.Append("</div>");
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
