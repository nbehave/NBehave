using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public abstract class NBehaveContext : Dictionary<string, object>
    {
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