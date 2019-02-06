using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// 表示主内容加侧边栏
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class MasterDetail : IHtmlObject, IList<IHtmlObject>
    {
        public string Id { get; } = Guid.NewGuid().ToString("N").Substring(0, 6);
        public Form InfoGather { get; set; }
        public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();
        public MasterDetail(Form infoGather) { InfoGather = infoGather; }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append("<div class=\"row\"><div class=\"col-12 col-md-3 order-md-1 pt-3\">");
            InfoGather.ToHtml(sb);
            sb.Append("</div><div class=\"col-12 col-md-9 mt-3 mb-3 pr-4 pl-4 order-md-0\" id=\"" + Id + "\">");
            Children.ForEach((obj) => obj.ToHtml(sb));
            sb.Append("</div></div>");
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
