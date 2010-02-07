using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.storage
{
    class StorageNull : StorageBase
    {
        public override object GetItem(string key, StorageBase.ComputeItemMethod method)
        {
            DateTime expiration;
            return method(out expiration);
        }
    }
}
