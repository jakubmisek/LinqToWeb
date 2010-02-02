using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using linqtoweb.Core.datacontext;
using linqtoweb.Core.extraction;
using linqtoweb.Core.methods;
namespace linqtoweb.Example
{
    public partial class WebContext : ExtractionContext
    {
        public partial class XXX : ExtractionObjectBase
        {
            private string _str = null;
            public string str{get{while(_str==null){if (!DoNextAction(null))throw new NotExtractedDataException("str cannot reach any data.");} return _str;}set{_str=value;}}
            
            public readonly ExtractionList<XXX> xxxs = new ExtractionList<XXX>();
            
        }
        private static void _7_0(DataContext datacontext, LocalVariables parameters)
        {
            ExtractionListBase<XXX> sampleList = (ExtractionListBase<XXX>)parameters["sampleList"];
            ScopesStack __l = new ScopesStack(datacontext, null);
            {
                {
                    __l.Push(__l.context.OpenContextDynamic("open", new object[] {"http://www.freesutra.cz/"}), null);
                    foreach(var __fe10_1 in ForeachMethods.regexp(__l.context, "(?<x>ahoj)"))
                    {
                        __l.Push(null,__fe10_1);
                        {
                            ActionItem.AddAction( new ActionItem.ExtractionMethod[]{addel_16_0}, __l.context, new LocalVariables(new Dictionary<string, object>() {
                                {"l", sampleList},
                                {"val", ((string)__l["x"])} }));
                        }
                        __l.Pop();
                    }

                    __l.Pop();
                }

            }
        }
        private static void addel_16_0(DataContext datacontext, LocalVariables parameters)
        {
            ExtractionListBase<XXX> l = (ExtractionListBase<XXX>)parameters["l"];
            string val = (string)parameters["val"];
            ScopesStack __l = new ScopesStack(datacontext, null);
            {
                l.AddElement((new XXX(){ str = val }));
                XXX f = (new XXX(){ str = val });

                AddElementAction(__l.context, f.xxxs, (new XXX(){ str = "TEST" }));
                f = (new XXX(){ str = "xxx" });
            }
        }
        public readonly ExtractionList<XXX> sampleList = new ExtractionList<XXX>();
        protected override void InitActionsToDo()
        {
            base.InitActionsToDo();
            ActionItem.AddAction(_7_0, InitialDataContext, new LocalVariables(new Dictionary<string, object>() {
                {"sampleList", sampleList}
                }));
        }
    }
}
