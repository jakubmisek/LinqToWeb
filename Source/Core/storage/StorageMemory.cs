using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.storage
{
    class StorageMemory : StorageBase
    {
        private struct DictionaryValue
        {
            public DateTime ExpirationTime;
            public object Value;
        }

        private Dictionary<string, DictionaryValue> Items = new Dictionary<string, DictionaryValue>();

        private object[] locks = new object[64];

        public StorageMemory()
        {
            for(int i = 0; i < locks.Length; ++i)
                locks[i] = new object();
        }

        public override object GetItem(string key, StorageBase.ComputeItemMethod method)
        {
            lock (locks[key.GetHashCode() % locks.Length])
            {
                DictionaryValue item;
                if (!Items.TryGetValue(key, out item) || item.ExpirationTime <= DateTime.Now)
                {
                    DateTime expiration;

                    item = new DictionaryValue() { Value = method(out expiration), ExpirationTime = expiration };

                    Items[key] = item;
                }

                return item.Value;
            }
        }
    }
}
