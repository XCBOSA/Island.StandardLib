using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib
{
    public abstract class SingleInstance<T> where T : SingleInstance<T>
    {
        public static T instance;
        public SingleInstance() => instance = (T)this;
    }
}
