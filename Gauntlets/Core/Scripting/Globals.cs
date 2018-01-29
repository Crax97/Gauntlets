using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Scripting
{
    [MoonSharpUserData]
    class Globals
    {


        private Dictionary<string, DynValue> globalValues = null;

        private Globals()
        {
            globalValues = new Dictionary<string, DynValue>();
        }
        private static Globals _globalsInstance = null;

        public static Globals Instance
        {
            get
            {
                if (_globalsInstance == null)
                    _globalsInstance = new Globals();
                return _globalsInstance;
            }
        }

        public void Set(string key, DynValue value)
        {
            if (globalValues.ContainsKey(key))
            {
                globalValues[key] = value;
                return;
            }

            globalValues.Add(key, value);
        }

        public DynValue Get(string key)
        {
            if (!globalValues.ContainsKey(key)) return DynValue.Nil;
            return globalValues[key];
        }

        public DynValue this[string index] {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }
        

    }
}
