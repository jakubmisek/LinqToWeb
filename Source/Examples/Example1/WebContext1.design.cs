/* Generated LinqToWeb context
 * Date: 21.3.2010 13:38:22
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
    public partial class WebContext1 : ExtractionContext
    {
        #region Public classes declaration
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
        private static string urlencode_7_0(string str)
        {
        
        	return System.Web.HttpUtility.UrlEncode(str);
        
        }
        // htmldecode
        private static string htmldecode_10_0(string str)
        {
        
        	return System.Web.HttpUtility.HtmlDecode(str);
        
        }
        // main
        private static void main_15_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<Result> GoogleResults = (ExtractionListBase<Result>)_parameters["GoogleResults"];
            string query = (string)_parameters["query"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                {
                    __l.Push(__l.context.OpenContextDynamic("open", new object[] {(("http://www.google.com/search?q=")+(urlencode_7_0(query)))}), null);
                    ActionItem.AddAction( new ActionItem.ExtractionMethod[]{googlepage_22_0}, __l.context, new LocalVariables() {
                        {"items", GoogleResults} }.SetCannotAddAction(new Dictionary<string,bool>(){{"items",_parameters.CannotAddActionForVariable("GoogleResults")}}));
                    __l.Pop();
                }

            }
        }

        // googlepage
        private static void googlepage_22_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<Result> items = (ExtractionListBase<Result>)_parameters["items"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                foreach(var __fe24_1 in ForeachMethods.match(__l.context, "\\<h3\\sclass=r\\>\\<a\\shref=\\\"(?<rurl>[^\\\"]*)\\\"[^\\>]*\\>(?<rtitle>[^(.)]*)\\</a\\>\\</h3\\>"))
                {
                    __l.Push(null,__fe24_1);
                    {
                        items.AddElement((new Result(){ url = htmldecode_10_0((__l["rurl"].ToString())), title = (__l["rtitle"].ToString()) }));
                    }
                    __l.Pop();
                }

                foreach(var __fe29_1 in ForeachMethods.regexp(__l.context, "\\<a\\shref=\\\"(?<rurl>[^\\\"]*)\\\"[^\\>]*\\>\\<span\\sclass=\\\"csb\\sch\\\"\\sstyle=\\\"background-position:-76px"))
                {
                    __l.Push(null,__fe29_1);
                    {
                        __l.Push(__l.context.OpenContextDynamic("open", new object[] {htmldecode_10_0((__l["rurl"].ToString()))}), null);
                        ActionItem.AddAction( new ActionItem.ExtractionMethod[]{googlepage_22_0}, __l.context, new LocalVariables() {
                            {"items", items} }.SetCannotAddAction(new Dictionary<string,bool>(){{"items",_parameters.CannotAddActionForVariable("items")}}));
                        __l.Pop();
                    }
                    __l.Pop();
                }

            }
        }

        // main
        private static void main_34_0(DataContext _datacontext, LocalVariables _parameters)
        {
            ExtractionListBase<string> strs = (ExtractionListBase<string>)_parameters["strs"];
            ScopesStack __l = new ScopesStack(_datacontext, null);
            {
                {
                    __l.Push(__l.context.OpenContextDynamic("open", new object[] {"..\\..\\mshome.htm"}), null);
                    foreach(var __fe37_1 in ForeachMethods.xmlmatch(__l.context, "\r\n\t\t<submenu handle=\"~@handle@~\"><item label=\"~@lbl@~\" /></submenu>\r\n\t"))
                    {
                        __l.Push(null,__fe37_1);
                        {
                            strs.AddElement((((((("handle: ")+((__l["handle"].ToString()))))+(", label:")))+((__l["lbl"].ToString()))));
                        }
                        __l.Pop();
                    }

                    __l.Pop();
                }

            }
        }

        #endregion

        #region Public extracted data
        // Result[] GoogleResults
        public readonly ExtractionList<Result> GoogleResults = new ExtractionList<Result>();
        // string query
        // string[] strs
        public readonly ExtractionList<string> strs = new ExtractionList<string>();
        #endregion

        #region Context construct
        private void InitActionsToDo(string query)
        {
            ActionItem.AddAction(main_15_0, InitialDataContext, new LocalVariables(){
                {"GoogleResults", GoogleResults},
                {"query", query}});
            ActionItem.AddAction(main_34_0, InitialDataContext, new LocalVariables(){
                {"strs", strs}});
        }
        #region Constructors
        public WebContext1(string query):base(){InitActionsToDo(query);}
        public WebContext1(string query, StorageBase cache):base(cache){InitActionsToDo(query);}
        #endregion
        #endregion

    }
}
