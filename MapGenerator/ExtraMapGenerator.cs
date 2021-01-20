using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.MapGenerator
{
    public interface IExtraMapGenerator
    {
        ExtraMapData GenExtra(Chunk chunk);
    }

    public abstract class ExtraMapData
    {
        public T As<T>() where T : ExtraMapData => this as T;
    }
}
