using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HandSchool.Models;
using Newtonsoft.Json;
using Xamarin.Forms.Internals;

namespace HandSchool.Internal
{
    public class NamedCookieDictionary : IDictionary<string, Cookie>
    {
        private readonly IDictionary<string, Cookie> _inner;

        public long Version { get; private set; }

        public NamedCookieDictionary()
        {
            _inner = new Dictionary<string, Cookie>();
            Version = 0;
        }

        public Cookie this[string key]
        {
            get => _inner.TryGetValue(key, out var cookie) ? cookie : null;
            set
            {
                _inner[key] = value;
                Version++;
            }
        }

        public ICollection<string> Keys => _inner.Keys;

        public ICollection<Cookie> Values => _inner.Values;

        public int Count => _inner.Count;

        public bool IsReadOnly => _inner.IsReadOnly;

        public void Add(string key, Cookie value)
        {
            _inner.Add(key, value);
            Version++;
        }

        public void Add(KeyValuePair<string, Cookie> item)
        {
            _inner.Add(item);
            Version++;
        }

        public void Clear()
        {
            _inner.Clear();
            Version = 0;
        }

        public bool Contains(KeyValuePair<string, Cookie> item)
            => _inner.Contains(item);

        public bool ContainsKey(string key)
            => _inner.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, Cookie>[] array, int arrayIndex)
            => _inner.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, Cookie>> GetEnumerator()
            => _inner.GetEnumerator();

        public bool Remove(string key)
        {
            var res = _inner.Remove(key);
            Version++;
            return res;
        }

        public bool Remove(KeyValuePair<string, Cookie> item)
        {
            var res = _inner.Remove(item);
            Version++;
            return res;
        }

        public bool TryGetValue(string key, out Cookie value)
            => _inner.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        public static IEnumerable<Cookie> Filter(IEnumerable<Cookie> cookies, params string[] names)
        {
            if (cookies is null) yield break;
            if (names.Length == 0) yield break;
            using var enumerator = cookies.GetEnumerator();
            var count = names.Length;

            if (count == 1)
            {
                var name = names[0];
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current?.Name != name) continue;
                    yield return enumerator.Current;
                    yield break;
                }
            }
            else
            {
                var found = 0;
                var set = new HashSet<string>();
                names.ForEach(c => set.Add(c));
                while (enumerator.MoveNext())
                {
                    if (!set.Contains(enumerator.Current?.Name)) continue;
                    yield return enumerator.Current;
                    found++;
                    if (found >= count) yield break;
                }
            }
        }

        public IEnumerable<Cookie> Filter(params string[] names)
        {
            return Filter(_inner.Values.ToArray(), names);
        }

        /// <summary>
        /// 只更新Cookie值
        /// </summary>
        /// <param name="name">Cookie名</param>
        /// <param name="value">新的Cookie值</param>
        /// <returns>是否可以只更新Cookie值，即容器中是否含有该名字的Cookie</returns>
        public bool OnlyUpdateValue(string name, string value)
        {
            var res = false;
            this[name]?.Let(c =>
            {
                if (c.Value != value)
                {
                    c.Value = value;
                    Version++;
                }
                res = true;
            });
            return res;
        }

        /// <summary>
        /// 尽力只更新值，即先尝试只更新值，若失败，则直接更新Cookie
        /// </summary>
        public void TryOnlyUpdateValue(Cookie value)
        {
            if (OnlyUpdateValue(value.Name, value.Value)) return;
            this[value.Name] = value;
        }

        public string ToJson()
        {
            if (Values.Count == 0) return "[]";
            return JsonConvert.SerializeObject(Values.Select(c => new CookieLite(c)).ToArray());
        }
    }
}