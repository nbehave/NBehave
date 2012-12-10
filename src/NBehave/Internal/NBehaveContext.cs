using System.Collections.Generic;

namespace NBehave.Internal
{
    public abstract class NBehaveContext : Dictionary<string, object>
    {
        private readonly List<string> tags = new List<string>();
        public IEnumerable<string> Tags { get { return tags; } }

        internal void AddTags(IEnumerable<string> tagsToAdd)
        {
            tags.AddRange(tagsToAdd);
        }

        internal void ClearTags()
        {
            tags.Clear();
        }

        public T Get<T>(string key)
        {
            return (T)this[key];
        }

        public bool TryGet<T>(string key, out T value)
        {
            object v;
            var hasItem = TryGetValue(key, out v);
            value = (T)v;
            return hasItem;
        }
    }
}