using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.MapGenerator
{
    public interface IMapManagerDelegate
    {
        object OnCreateChunk(int chunkX, int chunkZ, float[,] hmp, ExtraMapData extra);
        void OnDestroyChunk(object attachedObject);
    }
}
