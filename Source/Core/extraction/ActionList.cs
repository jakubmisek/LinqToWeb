using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// The list of actions.
    /// </summary>
    public class ActionList
    {
        /// <summary>
        /// ActionItems in linked list (add/remove first element).
        /// </summary>
        internal LinkedList<ActionItem> Items = new LinkedList<ActionItem>();

        /// <summary>
        /// References to linked list of ActionItems by the specific ActionItem.
        /// </summary>
        internal Dictionary<ActionItem, LinkedListNode<ActionItem>> ItemsByAction = new Dictionary<ActionItem, LinkedListNode<ActionItem>>();
        
        /// <summary>
        /// Empty ActionList.
        /// </summary>
        public ActionList()
        {
            
        }

        /// <summary>
        /// Copy of another ActionList.
        /// </summary>
        /// <param name="collection"></param>
        public ActionList(ActionList collection)
        {
            foreach (var x in collection.Items)
            {
                ItemsByAction[x] = Items.AddLast(x);
            }
        }

        /// <summary>
        /// Insert an action to do at the beginning of the list.
        /// </summary>
        /// <param name="action">ActionItem to insert.</param>
        virtual public void AddAction(ActionItem action)
        {
            lock (this)
            {
                // must be the First ! (or check the ExtractionListEnumerator.CheckForNewActionsToDo())
                ItemsByAction[action] = Items.AddFirst(action);
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
                LinkedListNode<ActionItem>  node;
                if (ItemsByAction.TryGetValue(action, out node))
                {
                    Items.Remove(node);
                    ItemsByAction.Remove(action);
                }
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
                return ItemsByAction.ContainsKey(action);
            }
        }

        /// <summary>
        /// Returns next action to do.
        /// TODO: get next action using some priority or restrictions
        /// </summary>
        /// <returns></returns>
        virtual public ActionItem GetNextAction()
        {
            lock(this)
            {
                if (Items.First != null)
                {
                    return Items.First.Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
