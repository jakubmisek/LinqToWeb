/* Generated LinqToWeb context
 * Date: 15.2.2010 14:39:23
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using linqtoweb.Core.datacontext;
using linqtoweb.Core.extraction;
using linqtoweb.Core.methods;
using linqtoweb.Core.storage;

namespace Example1
{
    public partial class WebContext : ExtractionContext
    {
        #region Public classes declaration
        public partial class XXX : ExtractionObjectBase
        {
            #region Properties
            // string text
            public string text{get{while(_text==null){if (!DoNextAction<object>(null))throw new NotExtractedDataException("text cannot reach any data.");} return _text;}set{_text=value;}}
            private string _text = null;
            
            // DateTime time
            public DateTime time{get{while(_time==DateTime.MinValue){if (!DoNextAction<object>(null))throw new NotExtractedDataException("time cannot reach any data.");} return _time;}set{_time=value;}}
            private DateTime _time = DateTime.MinValue;
            
            #endregion
            #region Constructors
            public XXX():this(null){}
            public XXX(ExtractionObjectBase parent):base(parent)
            {
            }
            #endregion
        }
        #endregion

        #region Private extraction methods
        // 
        private static void _7_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<XXX> list = (ExtractionListBase<XXX>)_parameters["list"];
            XXX content = (XXX)_parameters["content"];
            string key = (string)_parameters["key"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                content.text = "";
                {
                    __l.Push(__l.context.OpenContextDynamic("open", new object[] {key}), null);
                    foreach(var __fe12_1 in ForeachMethods.regexp(__l.context, "(?<word>\\w+)"))
                    {
                        __l.Push(null,__fe12_1);
                        {
                            list.AddElement((new XXX(){ text = (__l["word"].ToString()), time = (DateTime.Parse("1/2/2010")) }));
                            content.text = ((((content.text)+(" ")))+((__l["word"].ToString())));
                        }
                        __l.Pop();
                    }

                    __l.Pop();
                }

            }
        }

        #endregion

        #region Public extracted data
        // XXX[] list
        public readonly ExtractionList<XXX> list = new ExtractionList<XXX>();
        // XXX content
        public readonly XXX content = new XXX();
        // string key
        #endregion

        #region Context construct
        private void InitActionsToDo(string key)
        {
            ActionItem.AddAction(_7_0, InitialDataContext, new LocalVariables(){
                {"list", list},
                {"content", content},
                {"key", key}});
        }
        #region Constructors
        public WebContext(string key):base(){InitActionsToDo(key);}
        public WebContext(StorageBase cache, string key):base(cache){InitActionsToDo(key);}
        #endregion
        #endregion

    }
}
