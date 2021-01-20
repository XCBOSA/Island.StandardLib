using Island.StandardLib.MapGenerator.ThreadTask;
using Island.StandardLib.Math;
using System.Threading;
using System.Threading.Tasks;

namespace Island.StandardLib.MapGenerator
{
    public class Chunk
    {
        public int ChunkSize { get; private set; }
        public int ChunkPositionX { get; private set; }
        public int ChunkPositionZ { get; private set; }
        public MapGen GeneratorRef { get; private set; }
        internal object AttachedObject { get; set; }
        public Percentage[,] BiomeMap { get; private set; }
        public float[,] HeightMap { get; private set; }
        public ExtraMapData ExtraMap { get; private set; }
        public ChunkStatus Status { get; internal set; }

        public float GetSteepness(int x, int z)
        {
            //float dst = 0f;
            //if (x > 0)
            //    dst += HeightMap[x - 1, z];
            //if (z > 0)
            //    dst += HeightMap[x, z - 1];
            //dst += HeightMap[x + 1, z];
            //dst += HeightMap[x, z + 1];
            //dst /= 4;
            //return HeightMap[x, z] / dst;
            float dv = HeightMap[x + 1, z + 1] - HeightMap[x, z];
            if (dv < 0) dv = -dv;
            return dv > 1 ? 1 : dv;
        }

        public bool Is(int x, int z) => x == ChunkPositionX && z == ChunkPositionZ;

        internal Chunk(int size, int x, int z, MapGen generator)
        {
            Status = ChunkStatus.Idle;
            ChunkSize = size;
            ChunkPositionX = x;
            ChunkPositionZ = z;
            GeneratorRef = generator;
        }

        public int DistanceOf(int chunkPosX, int chunkPosZ)
        {
            return (int)System.Math.Sqrt(System.Math.Pow(chunkPosX - ChunkPositionX, 2) + System.Math.Pow(chunkPosZ - ChunkPositionZ, 2));
        }

        public float DistanceOfFloat(float chunkPosX, float chunkPosZ)
        {
            return (float)System.Math.Sqrt(System.Math.Pow(chunkPosX - ChunkPositionX, 2) + System.Math.Pow(chunkPosZ - ChunkPositionZ, 2));
        }

        public int[] TransformXZ(int chunkX, int chunkZ)
        {
            return new int[2] { ChunkPositionX * ChunkSize + chunkX, ChunkPositionZ * ChunkSize + chunkZ };
        }

        public void NonGC_TransformXZ(int[] result_arr_2l, int chunkX, int chunkZ)
        {
            result_arr_2l[0] = ChunkPositionX * ChunkSize + chunkX;
            result_arr_2l[1] = ChunkPositionZ * ChunkSize + chunkZ;
        }

        bool thr_stopflag;

        void GenerateHeightmap(MapManager manager)
        {
            Status = ChunkStatus.Generating;
            float[,] hmp = new float[ChunkSize + 1, ChunkSize + 1];
            Percentage[,] biomeMap = new Percentage[ChunkSize + 1, ChunkSize + 1];
            int[] xztmp = new int[2];
            for (int z = 0; z < ChunkSize + 1; z++)
            {
                for (int x = 0; x < ChunkSize + 1; x++)
                {
                    if (thr_stopflag)
                    {
                        Status = ChunkStatus.Idle;
                        return;
                    }
                    NonGC_TransformXZ(xztmp, x, z);
                    biomeMap[z, x] = GeneratorRef.GetBiome(xztmp[1], xztmp[0]);
                    hmp[z, x] = GeneratorRef.Gen01_KnownBiome(xztmp[1], xztmp[0], biomeMap[z, x]);
                    Thread.Sleep(0);
                }
            }
            HeightMap = hmp;
            BiomeMap = biomeMap;
            GenExtramap(manager);
            Status = ChunkStatus.Prepared;
        }

        void GenExtramap(MapManager manager)
        {
            if (manager.ExtraMapGenerator == null) return;
            ExtraMap = manager.ExtraMapGenerator.GenExtra(this);
        }

        public void Stop(MapManager manager)
        {
            thr_stopflag = true;
            if (AttachedObject != null)
            {
                manager.@delegate.OnDestroyChunk(AttachedObject);
                AttachedObject = null;
            }
        }

        public void BeginGenerate(MapManager manager, float order)
        {
            if (HeightMap != null) return;
            OrderedThreadQueue.QueueTask(() => GenerateHeightmap(manager), order);
        }

        public enum ChunkStatus
        {
            Idle,
            Generating,
            Prepared,
            PreparedAndNoticed
        }
    }
}
