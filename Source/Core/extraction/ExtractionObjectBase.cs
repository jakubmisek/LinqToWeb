using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Base object containing extracted data.
    /// Contains list of actions to do, to extract the object properties value.
    /// </summary>
    class ExtractionObjectBase
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
        /// Priority list of action to do, to obtain new data.
        /// </summary>
        internal readonly ActionList ActionsToDo;

        /// <summary>
        /// Default ctor.
        /// </summary>
        public ExtractionObjectBase()
        {
            ActionsToDo = new ActionList(); // empty list
        }

        /// <summary>
        /// Initialization with initial action list.
        /// </summary>
        /// <param name="initialActions">Initial ActionList.</param>
        public ExtractionObjectBase(ActionList initialActions)
        {
            ActionsToDo = initialActions;   // initial actions
        }

        /// <summary>
        /// Execute next action in the ActionsToDo list.
        /// TODO: Can execute more actions at once asynchronously, method waits until some action is done.
        /// </summary>
        /// <param name="enumeratedList">Parameters transformation map.</param>
        /// <returns>False if there are no actions, and no action can be executed.
        /// True if an action finished since last DoNextAction call (and some new data might be here).</returns>
        internal virtual bool DoNextAction(Dictionary<object, object> parametersTransform)
        {
            Debug.Assert(ActionsToDo != null, "Objects using DoNextAction() must initialize ActionsToDo list.");

            ActionItem action = ActionsToDo.GetNextAction();

            if (action != null)
            {
                action.CallAction(parametersTransform);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method returns an object that will be passed to the extraction method.
        /// Used for lists; list is passed to extraction method only if the list is just enumerated.
        /// </summary>
        /// <returns>An object that has to be passed to extraction method. Typically it returns this.</returns>
        internal virtual object TransformParameter()
        {
            return this;
        }

        #region sample object property

        private string _sampleProperty = null;

        public string SampleProperty
        {
            get
            {
                while (_sampleProperty == null)
                {
                    if (!DoNextAction(null))
                        throw new NotExtractedDataException("SampleProperty cannot reach any data.");
                }

                return _sampleProperty;
            }
            set
            {
                _sampleProperty = value;
            }
        }

        #endregion
    }

}
