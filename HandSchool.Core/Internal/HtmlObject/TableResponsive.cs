using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// 响应式数据表格。
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class TableResponsive : IHtmlObject, IDictionary<string, decimal>
    {
        public string Class { get; set; } = "table table-responsive";
        public IDictionary<string, decimal> Column { get; set; } = new Dictionary<string, decimal>();
        public string Id { get; set; }
        public bool UseTBody { get; set; }
        public string DefaultContent { get; set; } = string.Empty;

        public TableResponsive(bool useTBody = true, string bodyId = null)
        {
            UseTBody = useTBody;
            Id = bodyId ?? Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            if (UseTBody)
            {
                sb.Append($"<table class=\"{Class}\"><thead><tr>");
                Column.ForEach((s) => sb.Append($"<th scope=\"col\" style=\"min-width:{s.Value}em\">{s.Key}</th>"));
                sb.Append($"</tr></thead><tbody id=\"{Id}\">{DefaultContent}</tbody></table>");
            }
            else
            {
                sb.Append($"<table class=\"{Class}\" id=\"{Id}\"><tr>");
                Column.ForEach((s) => sb.Append($"<th scope=\"col\" style=\"min-width:{s.Value}em\">{s.Key}</th>"));
                sb.Append($"</tr></table>");
            }
        }

        #region IDictionary<string, decimal> Implement

        public decimal this[string key] { get => Column[key]; set => Column[key] = value; }
        public void Add(string key, decimal value) => Column.Add(key, value);
        ICollection<string> IDictionary<string, decimal>.Keys => Column.Keys;
        ICollection<decimal> IDictionary<string, decimal>.Values => Column.Values;
        int ICollection<KeyValuePair<string, decimal>>.Count => Column.Count;
        bool ICollection<KeyValuePair<string, decimal>>.IsReadOnly => Column.IsReadOnly;
        public void Add(KeyValuePair<string, decimal> item) => Column.Add(item);
        void ICollection<KeyValuePair<string, decimal>>.Clear() => Column.Clear();
        bool ICollection<KeyValuePair<string, decimal>>.Contains(KeyValuePair<string, decimal> item) => Column.Contains(item);
        bool IDictionary<string, decimal>.ContainsKey(string key) => Column.ContainsKey(key);
        void ICollection<KeyValuePair<string, decimal>>.CopyTo(KeyValuePair<string, decimal>[] array, int arrayIndex) => Column.CopyTo(array, arrayIndex);
        IEnumerator<KeyValuePair<string, decimal>> IEnumerable<KeyValuePair<string, decimal>>.GetEnumerator() => Column.GetEnumerator();
        bool IDictionary<string, decimal>.Remove(string key) => Column.Remove(key);
        bool ICollection<KeyValuePair<string, decimal>>.Remove(KeyValuePair<string, decimal> item) => Column.Remove(item);
        bool IDictionary<string, decimal>.TryGetValue(string key, out decimal value) => Column.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => Column.GetEnumerator();

        #endregion
    }
}
