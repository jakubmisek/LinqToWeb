using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.Core.extraction
{
    #region exceptions

    /// <summary>
    /// An exception that is thrown when the content of property was not extracted. There was no reachable action that extracts required data.
    /// </summary>
    public class NotExtractedDataException : Exception
    {
        /// <summary>
        /// Exception initialization.
        /// </summary>
        /// <param name="message"></param>
        public NotExtractedDataException()
        {

        }

        /// <summary>
        /// Exception initialization.
        /// </summary>
        /// <param name="message"></param>
        public NotExtractedDataException(string message)
            : base(message)
        {

        }
    }

    #endregion

    /// <summary>
    /// Base object containing extracted data.
    /// Contains list of actions to do, to extract the object properties value.
    /// </summary>
    public class ExtractionObjectBase
    {
        #region ActionsToDo

        /// <summary>
        /// Priority list of action to do, to obtain new data. Can be null.
        /// </summary>
        internal readonly ActionList ActionsToDo;

        /// <summary>
        /// Add an action into the ActionsToDO list properly.
        /// </summary>
        /// <param name="action"></param>
        internal virtual void AddActionToDo(ActionItem action)
        {
            if (ActionsToDo != null)
                ActionsToDo.AddAction(action);
        }

        /// <summary>
        /// Remove the action from the ActionsToDO list properly.
        /// </summary>
        /// <param name="action"></param>
        internal virtual void RemoveActionToDo(ActionItem action)
        {
            if (ActionsToDo != null)
                ActionsToDo.RemoveAction(action);
        }

        #endregion

        /// <summary>
        /// Parent object. Can contain another ActionsToDo, that may be needed to get some data.
        /// </summary>
        internal readonly ExtractionObjectBase Parent;

        /// <summary>
        /// Default ctor.
        /// </summary>
        public ExtractionObjectBase()
            :this(null, new ActionList())
        {
        }

        /// <summary>
        /// Construct an object with given parent object reference.
        /// </summary>
        /// <param name="parent">Parent object in object hierarchy. Used for calling next action, that may result in modifying its child objects.</param>
        public ExtractionObjectBase(ExtractionObjectBase parent)
            : this(parent, new ActionList())
        {

        }
        
        /// <summary>
        /// Initialization with initial action list.
        /// </summary>
        /// <param name="initialActions">Initial ActionList.</param>
        public ExtractionObjectBase(ExtractionObjectBase parent, ActionList initialActions)
        {
            this.Parent = parent;
            this.ActionsToDo = initialActions;   // initial actions
        }

        /// <summary>
        /// Execute next action in the ActionsToDo list.
        /// TODO: Can execute more actions at once asynchronously, method waits until some action is done.
        /// </summary>
        /// <param name="callerEnumerator">List enumerator that requested this method call.</param>
        /// <returns>False if there are no actions, and no action can be executed.
        /// True if an action finished since last DoNextAction call (and some new data might be here).</returns>
        protected virtual bool DoNextAction<S>(ExtractionListEnumerator<S> callerEnumerator)
        {
            Debug.Assert(ActionsToDo != null, "Object using DoNextAction() must initialize its ActionsToDo list.");

            // TODO: lock DoNextAction, CallAction on separated thread, wait for some thread to finish
            // another thread can try to process the same action in the same time
            
            ActionItem action = ActionsToDo.GetNextAction();

            if (action != null)
            {
                action.CallAction(callerEnumerator);

                return true;
            }
            else
            {
                if (Parent != null) // maybe parent object's action can initiate this object's properties
                    return Parent.DoNextAction(callerEnumerator);
                else
                    return false;
            }
        }

        /// <summary>
        /// This method returns an object that will be passed to the extraction method.
        /// Used for lists; list is passed to extraction method only if the list is just enumerated.
        /// </summary>
        /// <param name="hasAction">True if this object contains the called action. Otherwise the argument can be transformed to a ghost which will be lost, because this object already processed the action.</param>
        /// <param name="callerEnumerator">ListEnumerator that invoked this action call. If this is its listContainer, it should be transformed to the callerEnumerator object.</param>
        /// <returns>An object that has to be passed to extraction method. Typically it returns this.</returns>
        internal virtual ExtractionObjectBase TransformArgument<S>(bool hasAction, ExtractionListEnumerator<S> callerEnumerator)
        {
            return this;
        }

        /*#region sample object property

        private string _sampleProperty = null;

        public string SampleProperty
        {
            get
            {
                while (_sampleProperty == null)
                {
                    if (!DoNextAction<object>(null))
                        throw new NotExtractedDataException("SampleProperty cannot reach any data.");
                }

                return _sampleProperty;
            }
            set
            {
                _sampleProperty = value;
            }
        }

        #endregion*/
    }

}
