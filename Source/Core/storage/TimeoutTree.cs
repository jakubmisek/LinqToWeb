using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace linqtoweb.Core.storage
{
    /// <summary>
    /// Timeout Tree, BST containing objects with given expiration time.
    /// Expired objects are automatically disposed.
    /// </summary>
    public class TimeoutTree<TKey,TValue>
    {
        protected class TTNode : LinkedList<KeyValuePair<TKey,TValue>>
        {
            /// <summary>
            /// The node expire in time.
            /// </summary>
            public readonly DateTime ExpirationTime;

            /// <summary>
            /// The node subtrees.
            /// </summary>
            internal TTNode
                lNode = null,   // nodes that expire earlier
                rNode = null;   // nodes that expire later

            /// <summary>
            /// .ctor
            /// </summary>
            /// <param name="expirationTime"></param>
            public TTNode( DateTime expirationTime )
            {
                this.ExpirationTime = expirationTime;
                
            }
        }

        /// <summary>
        /// The tree root node.
        /// </summary>
        private TTNode rootNode = null;

        /// <summary>
        /// 
        /// </summary>
        private struct DictionaryValue
        {
            public TTNode treeNode;
            public LinkedListNode<KeyValuePair<TKey,TValue>> listNode;
        }

        /// <summary>
        /// Items by key.
        /// </summary>
        private Dictionary<TKey, DictionaryValue> Items = new Dictionary<TKey, DictionaryValue>();

        /// <summary>
        /// Try to get a value by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                CheckExpirations(DateTime.Now);

                DictionaryValue value;
                if (Items.TryGetValue(key, out value))
                    return value.listNode.Value.Value;

                return default(TValue);
            }
        }

        /// <summary>
        /// Checks if the structure contains the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        /// <summary>
        /// Add or update the item.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public void Add(TKey key, TValue value, DateTime expiration)
        {
            RemoveFromNodes(key);// remove only from nodes, keep in the dictionary - will be replaced immediately

            Add(key, value, expiration, ref rootNode, this);
        }

        /// <summary>
        /// Add the value into the specified sub-tree.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <param name="node"></param>
        private void Add(TKey key, TValue value, DateTime expiration, ref TTNode node, object parentLocker)
        {
            if (node == null)
            {
                lock (parentLocker)
                    if (node == null)
                        node = new TTNode(expiration);
            }

            if (expiration < node.ExpirationTime)
            {
                Add(key, value, expiration, ref node.lNode, node);
            }
            else if (expiration > node.ExpirationTime)
            {
                Add(key, value, expiration, ref node.rNode, node);
            }
            else
            {
                lock (node)
                {
                    // add the item in to the current node
                    Items[key] = new DictionaryValue() { treeNode = node, listNode = node.AddFirst(new KeyValuePair<TKey, TValue>(key, value)) };
                }
            }
        }

        /// <summary>
        /// Remove item by given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool RemoveFromNodes(TKey key)
        {
            DictionaryValue value;
            if (Items.TryGetValue(key, out value))
            {
                lock (value.treeNode)
                {
                    value.treeNode.Remove(value.listNode);
                    //Items.Remove(key);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
                
        /// <summary>
        /// Removes expired items.
        /// </summary>
        /// <param name="now"></param>
        private void CheckExpirations(DateTime now)
        {
            CheckExpirations(now, ref rootNode);
        }

        private void CheckExpirations(DateTime now, ref TTNode node)
        {
            if (node == null)
                return;

            if (node.ExpirationTime <= now)
            {
                // remove node and its left subtree
                TTNode toRemove = node;

                // put the right subtree into its place
                node = node.rNode;
                CheckExpirations(now, ref node);


                //
                ClearNode(toRemove, false);
                ClearNode(toRemove.lNode, true);
            }
            else
            {
                CheckExpirations(now, ref node.lNode);
            }
        }

        /// <summary>
        /// Properly removes all items from the given tree node.
        /// </summary>
        /// <param name="node"></param>
        private void ClearNode( TTNode node, bool wholeSubtree )
        {
            if (node == null)
                return;

            foreach (var x in node)
            {
                Items.Remove(x.Key);
            }

            if (wholeSubtree)
            {
                ClearNode(node.lNode,true);
                ClearNode(node.rNode,true);
            }
        }
        
    }
}
