using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// 表示数据表。
    /// </summary>
    /// <inheritdoc cref="Table" />
    public class Table : IHtmlObject, IList<string>
    {
        public string Class { get; set; } = "table";
        public IList<string> Column { get; set; } = new List<string>();
        public string Id { get; set; }
        public bool UseTBody { get; set; }

        public Table(bool useTBody = true, string bodyId = null)
        {
            UseTBody = useTBody;
            Id = bodyId ?? Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            if (UseTBody)
            {
                sb.Append($"<table class=\"{Class}\"><thead><tr>");
                Column.ForEach((s) => sb.Append($"<th scope=\"col\">{s}</th>"));
                sb.Append($"</tr></thead><tbody id=\"{Id}\"></tbody></table>");
            }
            else
            {
                sb.Append($"<table class=\"{Class}\" id=\"{Id}\"><tr>");
                Column.ForEach((s) => sb.Append($"<th scope=\"col\">{s}</th>"));
                sb.Append($"</tr></table>");
            }
        }

        #region IList<string> Implement

        public void Add(string item) => Column.Add(item);
        int ICollection<string>.Count => Column.Count;
        bool ICollection<string>.IsReadOnly => Column.IsReadOnly;
        void ICollection<string>.Clear() => Column.Clear();
        bool ICollection<string>.Contains(string item) => Column.Contains(item);
        public string this[int index] { get => Column[index]; set => Column[index] = value; }
        void ICollection<string>.CopyTo(string[] array, int arrayIndex) => Column.CopyTo(array, arrayIndex);
        int IList<string>.IndexOf(string item) => Column.IndexOf(item);
        void IList<string>.Insert(int index, string item) => Column.Insert(index, item);
        bool ICollection<string>.Remove(string item) => Column.Remove(item);
        void IList<string>.RemoveAt(int index) => Column.RemoveAt(index);
        IEnumerator<string> IEnumerable<string>.GetEnumerator() => Column.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Column.GetEnumerator();

        #endregion
    }
}
