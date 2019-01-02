using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using KVP = System.Collections.Generic.KeyValuePair<string, string>;

namespace HandSchool.Internal
{
    public class KeyValueDict : NameValueCollection, IDictionary<string, string>
    {
        bool ICollection<KVP>.IsReadOnly => IsReadOnly;
        public new ICollection<string> Keys => AllKeys;
        public ICollection<string> Values => throw new NotImplementedException();

        public void Add(KVP item)
        {
            Add(item.Key, item.Value);
        }

        public bool ContainsKey(string key)
        {
            return GetValues(key) != null;
        }

        public new IEnumerator<KVP> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        public IEnumerable<KeyValuePair<string, string>> GetEnumerable()
        {
            return AllKeys.Select(key => new KVP(key, this[key]));
        }

        public bool Remove(KVP item)
        {
            return ((IDictionary<string, string>) this).Remove(item.Key);
        }

        public bool Contains(KVP item)
        {
            var values = GetValues(item.Key);
            if (values is null) return false;
            return values[0] == item.Value;
        }

        bool IDictionary<string, string>.Remove(string key)
        {
            var result = ContainsKey(key);
            if (result) base.Remove(key);
            return result;
        }

        public bool TryGetValue(string key, out string value)
        {
            if (ContainsKey(key))
            {
                value = Get(key);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        void ICollection<KVP>.CopyTo(KVP[] array, int arrayIndex)
        {
            GetEnumerable().ToList().CopyTo(array, arrayIndex);
        }
    }

    public class EnumerableAdapter : IDictionary<string, string>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> innerEnumerable;

        public EnumerableAdapter(IEnumerable<KeyValuePair<string, string>> wrap)
        {
            innerEnumerable = wrap;
        }

        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<string> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return innerEnumerable.GetEnumerator();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)innerEnumerable).GetEnumerator();
        }
    }
}
