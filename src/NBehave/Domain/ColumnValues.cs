using System;
using System.Collections;
using System.Collections.Generic;

namespace NBehave
{
    [Serializable]
    public class ColumnValues : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        public ColumnValues(Dictionary<string, string> columnValues)
        {
            foreach (var value in columnValues)
                values.Add(value.Key.ToLower(), value.Value);
        }

        public string this[string key]
        {
            get { return values[key.ToLower()]; }
            set { values[key.ToLower()] = value; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            values.Clear();
        }

        public void Add(string key, string value)
        {
            values.Add(key.ToLower(), value);
        }
    }
}