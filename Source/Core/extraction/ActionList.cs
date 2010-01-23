using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    class ActionList
    {
        virtual public void Push(Action action)
        {

        }

        virtual public Action Pop()
        {
            return null;
        }

        public int Count
        {
            get { return 0; }
        }
    }
}
