using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.storage
{
    class StorageMemory : StorageBase
    {
        /// <summary>
        /// Item in the cache. Value of the dictionary.
        /// </summary>
        private struct DictionaryValue
        {
            public DateTime ExpirationTime;
            public object Value;
        }

        /// <summary>
        /// Information about single dictionary.
        /// </summary>
        private class DictionaryInfo
        {
            public DateTime LastCheck = DateTime.Now;
            public DateTime MinExpirationTime = DateTime.MaxValue;
        }

        /// <summary>
        /// List of dictionaries (distributed cache).
        /// </summary>
        private Dictionary<string, DictionaryValue>[] ItemsList = new Dictionary<string, DictionaryValue>[64];

        /// <summary>
        /// Infos about dictionaries.
        /// </summary>
        private DictionaryInfo[] ItemsListInfo;

        /// <summary>
        /// Minimal interval between cache cleanups.
        /// </summary>
        private readonly double CleanupsIntervalMinutes = 5.0;
        
        /// <summary>
        /// 
        /// </summary>
        public StorageMemory()
        {
            ItemsListInfo = new DictionaryInfo[ItemsList.Length];

            for (int i = 0; i < ItemsList.Length; ++i)
            {
                ItemsList[i] = new Dictionary<string, DictionaryValue>();
                ItemsListInfo[i] = new DictionaryInfo();
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CleanupsIntervalMinutes">Minimal interval between cache cleanups.</param>
        private StorageMemory(double CleanupsIntervalMinutes)
            :this()
        {
            this.CleanupsIntervalMinutes = CleanupsIntervalMinutes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override object GetItem(string key, StorageBase.ComputeItemMethod method)
        {
            long index = (uint)key.GetHashCode() % ItemsList.Length;
            var items = ItemsList[index];

            DateTime now = DateTime.Now;

            lock (items)
            {
                DictionaryValue item;
                if (!items.TryGetValue(key, out item) || item.ExpirationTime <= now)
                {
                    DateTime expiration;

                    item = new DictionaryValue() { Value = method(out expiration), ExpirationTime = expiration };

                    if (expiration > now)   // item can be added into the cache
                        items[key] = item;

                    // update DictionaryInfo
                    DictionaryInfo info = ItemsListInfo[index];

                    if (expiration < info.MinExpirationTime)
                        info.MinExpirationTime = expiration;

                    // check some other items for expiration
                    if (info.MinExpirationTime < now && // some items expired
                        info.LastCheck < now.AddMinutes(-CleanupsIntervalMinutes))  // interval between cleanups
                    {
                        RemoveExpired(items, info);
                    }
                }

                return item.Value;
            }
        }

        /// <summary>
        /// Clean the dictionary. Remove expired items.
        /// </summary>
        /// <param name="lockedItems">Dictionary, must be locked.</param>
        private void RemoveExpired(Dictionary<string, DictionaryValue> lockedItems, DictionaryInfo info)
        {
            DateTime now = DateTime.Now;
            List<string> keysToremove = new List<string>();
            
            DateTime minExpiration = DateTime.MaxValue;
            
            // collect expired items, get minExpiration time
            foreach (var x in lockedItems)
            {
                if (x.Value.ExpirationTime <= now)
                {
                    keysToremove.Add(x.Key);
                }
                else
                {
                    if (x.Value.ExpirationTime < minExpiration)
                        minExpiration = x.Value.ExpirationTime;
                }
            }

            // remove expired items
            foreach (var x in keysToremove)
            {
                lockedItems.Remove(x);
            }

            // update info
            info.LastCheck = now;
            info.MinExpirationTime = minExpiration;
        }
    }
}
