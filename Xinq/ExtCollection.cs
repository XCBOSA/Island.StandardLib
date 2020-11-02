using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Xinq
{
    public static class ExtCollection
    {
        public static T WhereFirst<T>(this ICollection<T> ts, Func<T, bool> where)
        {
            foreach (T t in ts)
                if (where(t)) return t;
            return default;
        }
    }
}
