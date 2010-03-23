/*
 * Generated LinqToWeb context
 * 24.3.2010 0:25:30
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
    public partial class GoogleSearcher : ExtractionContext
    {
        #region Public classes declaration
        public partial class QueryState : ExtractionObjectBase
        {
            #region Properties
            // string info
            public string info{get{while(_info==null){if (!DoNextAction<object>(null))throw new NotExtractedDataException("info cannot reach any data.");} return _info;}set{_info=value;}}
            private string _info = null;
            
            #endregion
            #region Constructors
            public QueryState():this(null){}
            public QueryState(ExtractionObjectBase parent):base(parent)
            {
            }
            #endregion
        }
        public partial class Result : ExtractionObjectBase
        {
            #region Properties
            // string title
            public string title{get{while(_title==null){if (!DoNextAction<object>(null))throw new NotExtractedDataException("title cannot reach any data.");} return _title;}set{_title=value;}}
            private string _title = null;
            
            // string url
            public string url{get{while(_url==null){if (!DoNextAction<object>(null))throw new NotExtractedDataException("url cannot reach any data.");} return _url;}set{_url=value;}}
            private string _url = null;
            
            #endregion
            #region Constructors
            public Result():this(null){}
            public Result(ExtractionObjectBase parent):base(parent)
            {
            }
            #endregion
        }
        #endregion

        #region Private extraction methods
        // urlencode
        private static string urlencode_20_0(string str)
        {
        
            return System.Web.HttpUtility.UrlEncode(str);
        
        }
        // htmldecode
        private static string htmldecode_23_0(string str)
        {
        
            return System.Web.HttpUtility.HtmlDecode(str);
        
        }
        // main
        private static void main_40_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<Result> GoogleResults = (ExtractionListBase<Result>)_parameters["GoogleResults"];
            QueryState state = (QueryState)_parameters["state"];
            string searchQuery = (string)_parameters["searchQuery"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                {
                    __l.Push(__l.context.OpenContextDynamic("open", new object[] {(("http://www.google.com/search?hl=en&q=")+(urlencode_20_0(searchQuery)))}), null);
                    ActionItem.AddAction( new ActionItem.ExtractionMethod[]{googlepage_50_0}, __l.context, new LocalVariables() {
                        {"items", GoogleResults} }.SetCannotAddAction(new Dictionary<string,bool>(){{"items",_parameters.CannotAddActionForVariable("GoogleResults")}}));
                    ActionItem.AddAction( new ActionItem.ExtractionMethod[]{searchinfo_65_0}, __l.context, new LocalVariables() {
                        {"state", state} }.SetCannotAddAction(new Dictionary<string,bool>(){{"state",_parameters.CannotAddActionForVariable("state")}}));
                    __l.Pop();
                }

            }
        }

        // googlepage
        private static void googlepage_50_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<Result> items = (ExtractionListBase<Result>)_parameters["items"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                foreach(var __fe52_4 in ForeachMethods.xmlmatch(__l.context, "\r\n        <h3 class=\"r\">\r\n            <a href=\"~@rhref@~\" class=\"l\">~@rtitle@~</a>\r\n        </h3>\r\n    "))
                {
                    __l.Push(null,__fe52_4);
                    {
                        items.AddElement((new Result(){ url = htmldecode_23_0((__l["rhref"].ToString())), title = (__l["rtitle"].ToString()) }));
                    }
                    __l.Pop();
                }

                foreach(var __fe61_4 in ForeachMethods.xmlmatch(__l.context, "<a href=\"~@rhref@~\"><span class=\"csb ch\" style=\"background-position:-76px 0;margin-right:34px;width:66px\" />Next</a>"))
                {
                    __l.Push(null,__fe61_4);
                    {
                        __l.Push(__l.context.OpenContextDynamic("open", new object[] {htmldecode_23_0((__l["rhref"].ToString()))}), null);
                        ActionItem.AddAction( new ActionItem.ExtractionMethod[]{googlepage_50_0}, __l.context, new LocalVariables() {
                            {"items", items} }.SetCannotAddAction(new Dictionary<string,bool>(){{"items",_parameters.CannotAddActionForVariable("items")}}));
                        __l.Pop();
                    }
                    __l.Pop();
                }

            }
        }

        // searchinfo
        private static void searchinfo_65_0(DataContext _datacontext, LocalVariables _parameters)
        {
            QueryState state = (QueryState)_parameters["state"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                foreach(var __fe67_4 in ForeachMethods.match(__l.context, "<p id=resultStats>&nbsp;~@strstat@~&nbsp;</div>"))
                {
                    __l.Push(null,__fe67_4);
                    state.info = (__l["strstat"].ToString());                    __l.Pop();
                }

            }
        }

        #endregion

        #region Public extracted data
        // Result[] GoogleResults
        public readonly ExtractionList<Result> GoogleResults = new ExtractionList<Result>();
        // QueryState state
        public readonly QueryState state = new QueryState();
        // string searchQuery
        #endregion

        #region Context construct
        private void InitActionsToDo(string searchQuery)
        {
            ActionItem.AddAction(main_40_0, InitialDataContext, new LocalVariables(){
                {"GoogleResults", GoogleResults},
                {"state", state},
                {"searchQuery", searchQuery}});
        }
        #region Constructors
        public GoogleSearcher(string searchQuery):base(){InitActionsToDo(searchQuery);}
        public GoogleSearcher(string searchQuery, StorageBase cache):base(cache){InitActionsToDo(searchQuery);}
        #endregion
        #endregion

    }
}
