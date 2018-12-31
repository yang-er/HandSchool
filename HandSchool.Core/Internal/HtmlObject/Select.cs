using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// ComboBox选择
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class Select : IHtmlObject, IDictionary<string, string>
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        public string Id { get; private set; }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            Id = string.Empty;
            if (full)
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 6);
                sb.Append($"<label for=\"{Id}\"><b>{Title}</b></label><select class=\"form-control\" name=\"{Name}\" id=\"{Id}\">");
            }
            else
                sb.Append($"<select class=\"form-control\" name=\"{Name}\">");
            foreach (var pair in Options)
                sb.Append($"<option value=\"{pair.Key}\">{pair.Value}</option>");
            sb.Append("</select>");
            if (full && Description.Length > 0)
                sb.Append($"<small id=\"{Id}\" class=\"form-text text-muted\">{Description}</small>");
        }

        public string this[string key]
        {
            get => Options[key];
            set => Options[key] = value;
        }

        public void Add(string key, string value) => Options.Add(key, value);
        ICollection<string> IDictionary<string, string>.Keys => Options.Keys;
        ICollection<string> IDictionary<string, string>.Values => Options.Values;
        int ICollection<KeyValuePair<string, string>>.Count => Options.Count;
        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => ((IDictionary<string, string>)Options).IsReadOnly;
        public void Add(KeyValuePair<string, string> item) => ((IDictionary<string, string>)Options).Add(item);
        void ICollection<KeyValuePair<string, string>>.Clear() => Options.Clear();
        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => ((IDictionary<string, string>)Options).Contains(item);
        bool IDictionary<string, string>.ContainsKey(string key) => Options.ContainsKey(key);
        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => ((IDictionary<string, string>)Options).CopyTo(array, arrayIndex);
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => ((IDictionary<string, string>)Options).GetEnumerator();
        bool IDictionary<string, string>.Remove(string key) => Options.Remove(key);
        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => ((IDictionary<string, string>)Options).Remove(item);
        bool IDictionary<string, string>.TryGetValue(string key, out string value) => Options.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, string>)Options).GetEnumerator();
    }
}
