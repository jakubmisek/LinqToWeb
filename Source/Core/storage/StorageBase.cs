using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.storage
{
    /// <summary>
    /// Base class for the storage mechanism in the linqtoweb extraction processes.
    /// </summary>
    public abstract class StorageBase
    {
        public delegate object ComputeItemMethod(out DateTime expiration);

        public abstract object GetItem(string key, ComputeItemMethod method);
    }
}
