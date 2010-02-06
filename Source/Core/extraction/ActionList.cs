using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// The list of actions.
    /// </summary>
    public class ActionList : IEnumerable<ActionItem>
    {
        /// <summary>
        /// Set of items to do. No ordering.
        /// </summary>
        internal readonly HashSet<ActionItem> ItemsSet;

        /// <summary>
        /// Empty ActionList.
        /// </summary>
        public ActionList()
            : this(null)
        {
            
        }

        /// <summary>
        /// Copy of another ActionList.
        /// </summary>
        /// <param name="collection"></param>
        public ActionList(IEnumerable<ActionItem> collection)
        {
            if (collection == null)
                ItemsSet = new HashSet<ActionItem>();
            else
                ItemsSet = new HashSet<ActionItem>(collection);
        }

        /// <summary>
        /// Insert an action to do.
        /// </summary>
        /// <param name="action">ActionItem to insert.</param>
        virtual public void AddAction(ActionItem action)
        {
            lock (this)
            {
                ItemsSet.Add(action);
            }
        }

        /// <summary>
        /// Remove specified action.
        /// </summary>
        /// <param name="action"></param>
        virtual public void RemoveAction(ActionItem action)
        {
            lock(this)
            {
                ItemsSet.Remove(action);    // O(1)
            }
        }

        /// <summary>
        /// Checks if the collection contains given action item.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        virtual public bool ContainsAction(ActionItem action)
        {
            lock (this)
            {
                return ItemsSet.Contains(action);   // O(1)
            }
        }

        /// <summary>
        /// Returns next action to do.
        /// TODO: get next action using some priority (shortest way to leaves) or restrictions (e.g. "I need to know this ..." or known filtering restrictions(Where condition))
        /// </summary>
        /// <returns></returns>
        virtual public ActionItem GetNextAction()
        {
            lock(this)
            {
                if (ItemsSet.Count > 0)
                    return ItemsSet.First();    // first item using HashSet enumerator, O(1)
                else
                    return null;
            }
        }

        /// <summary>
        /// Amount of items.
        /// </summary>
        public int Count
        {
            get
            {
                return ItemsSet.Count;
            }
        }

        #region IEnumerable<ActionItem> Members

        public IEnumerator<ActionItem> GetEnumerator()
        {
            return ItemsSet.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ItemsSet.GetEnumerator();
        }

        #endregion
    }
}
