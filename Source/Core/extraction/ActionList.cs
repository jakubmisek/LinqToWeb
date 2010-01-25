using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// The list of actions.
    /// TODO: The object is able to get actions by a priority of specified query.
    /// TODO: do not use linear list of actions (need of fast inserting and removing).
    /// </summary>
    public class ActionList : List<ActionItem>
    {
        public ActionList()
        {

        }

        public ActionList(int capacity)
            : base(capacity)
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
            this.Insert(0, action);
        }

        /// <summary>
        /// Returns next action to do.
        /// </summary>
        /// <returns></returns>
        virtual public ActionItem GetNextAction()
        {
            if (this.Count > 0)
            {
                return this[0];
            }
            else
            {
                return null;
            }
        }
    }
}
