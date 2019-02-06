using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandSchool.Internals.HtmlObject
{
    /// <summary>
    /// ComboBox选择
    /// </summary>
    /// <inheritdoc cref="IHtmlObject" />
    public class Select : IHtmlObject, IDictionary<string, string>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OnChanged { get; set; } = string.Empty;
        public IDictionary<string, string> Options { get; set; }
        public KeyValuePair<string, string>? FirstKeyValuePair { get; set; }
        public string Id { get; }

        public Select(string id = null, IDictionary<string, string> dict = null)
        {
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString("N").Substring(0, 6) : id;
            Options = dict ?? new Dictionary<string, string>();
        }

        private IEnumerable<KeyValuePair<string, string>> MergeOptions()
        {
            return FirstKeyValuePair is null ? Options : new[] { FirstKeyValuePair.Value }.Union(Options);
        }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            if (full && Title.Length > 0)
                sb.Append($"<label for=\"{Id}\"><b>{Title}</b></label>");
            sb.Append($"<select class=\"form-control\" id=\"{Id}\"");
            if (!string.IsNullOrEmpty(OnChanged))
                sb.Append($" onchange=\"{OnChanged}\"");
            sb.Append(">");
            bool first = true;

            foreach (var pair in MergeOptions())
            {
                sb.Append($"<option value=\"{pair.Key}\"{(first ? " selected" : "")}>{pair.Value}</option>");
                first = false;
            }

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
        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => Options.IsReadOnly;
        public void Add(KeyValuePair<string, string> item) => Options.Add(item);
        void ICollection<KeyValuePair<string, string>>.Clear() => Options.Clear();
        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => Options.Contains(item);
        bool IDictionary<string, string>.ContainsKey(string key) => Options.ContainsKey(key);
        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => Options.CopyTo(array, arrayIndex);
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => Options.GetEnumerator();
        bool IDictionary<string, string>.Remove(string key) => Options.Remove(key);
        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => Options.Remove(item);
        bool IDictionary<string, string>.TryGetValue(string key, out string value) => Options.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => Options.GetEnumerator();
    }
}
