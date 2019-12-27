using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathscript
{
    public static class PathscriptExt
    {
        public static object Eval(this object _this, string script)
            => PScript.Eval(_this, script);
        public static T Eval<T>(this object _this, string script)
            => (T)PScript.Eval(_this, script);
    }
}
