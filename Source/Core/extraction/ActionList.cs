using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// The list of actions.
    /// TODO: The object is able to get actions by a priority of specified query.
    /// </summary>
    public class ActionList : LinkedList<ActionItem>
    {
        public ActionList()
        {

        }

        public ActionList(IEnumerable<ActionItem> collection)
            : base(collection)
        {

        }

        /// <summary>
        /// Insert an action to do.
        /// </summary>
        /// <param name="action"></param>
        virtual public void AddAction(ActionItem action)
        {
            lock (this)
            {
                this.AddFirst(action);
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
                this.Remove(action);
            }
        }

        /// <summary>
        /// Returns next action to do.
        /// </summary>
        /// <returns></returns>
        virtual public ActionItem GetNextAction()
        {
            if (this.First != null)
            {
                return this.First.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
