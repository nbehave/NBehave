using System;
using System.Collections;
using System.Collections.Generic;

namespace NBehave.Domain
{
    [Serializable]
    public class ColumnValues : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public ColumnValues(Dictionary<string, string> columnValues)
        {
            foreach (var value in columnValues)
                _values.Add(value.Key.ToLower(), value.Value);
        }

        public string this[string key]
        {
            get { return _values[key.ToLower()]; }
            set { _values[key.ToLower()] = value; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _values.Clear();
        }

        public void Add(string key, string value)
        {
            _values.Add(key.ToLower(), value);
        }
    }
}