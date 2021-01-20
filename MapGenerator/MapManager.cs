using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.MapGenerator
{
    public class MapManager
    {
        public int ChunkSize { get; private set; }
        public bool Prepared { get; set; }
        public IMapManagerDelegate @delegate { get; set; }
        public IExtraMapGenerator ExtraMapGenerator;

        MapGen generator;
        List<Chunk> existChunks;

        public MapManager(int seed, int chunkSize = 512)
        {
            ChunkSize = chunkSize;
            generator = new MapGen(seed);
            existChunks = new List<Chunk>();
        }

        public void UpdateChunks(float[] realPos, int updateRadius = 3, int maxUpdatePerFrame = 1)
        {
            int[] cpos = realPos.Do(v => (int)v / ChunkSize);
            List<int[]> requestChunks = SearchNearbyChunks(cpos, updateRadius);
            List<Chunk> needDestroyed = new List<Chunk>(existChunks);
            List<Chunk> needCreated = new List<Chunk>();
            for (int i = 0, ct = 0; i < existChunks.Count && ct < maxUpdatePerFrame; i++)
            {
                if (existChunks[i].Status == Chunk.ChunkStatus.Prepared)
                {
                    ct++;
                    existChunks[i].AttachedObject = @delegate.OnCreateChunk(existChunks[i].ChunkPositionX, existChunks[i].ChunkPositionZ, existChunks[i].HeightMap, existChunks[i].ExtraMap);
                    existChunks[i].Status = Chunk.ChunkStatus.PreparedAndNoticed;
                }
            }
            foreach (var requestChunk in requestChunks)
            {
                bool founded = false;
                for (int i = 0; i < existChunks.Count; i++)
                {
                    if (existChunks[i].Is(requestChunk[0], requestChunk[1]))
                    {
                        if (needDestroyed.Contains(existChunks[i]))
                            needDestroyed.Remove(existChunks[i]);
                        founded = true;
                        break;
                    }
                }
                if (!founded) needCreated.Add(new Chunk(ChunkSize, requestChunk[0], requestChunk[1], generator));
            }
            foreach (Chunk chunk in needDestroyed)
            {
                if (chunk.DistanceOf(cpos[0], cpos[1]) > updateRadius + 5)
                {
                    existChunks.Remove(chunk);
                    chunk.Stop(this);
                }
            }
            needDestroyed.Clear();
            foreach (Chunk chunk in needCreated)
            {
                chunk.BeginGenerate(this, chunk.DistanceOfFloat(realPos[0], realPos[1]));
                existChunks.Add(chunk);
            }
            needCreated.Clear();
            if (existChunks.Count != 0 && !Prepared)
            {
                bool isOK = true;
                foreach (Chunk chunk in existChunks)
                    isOK &= chunk.Status == Chunk.ChunkStatus.PreparedAndNoticed;
                if (isOK) Prepared = true;
            }
        }

        public List<int[]> SearchNearbyChunks(int[] currentChunk, int radius)
        {
            var result = new List<int[]>();
            for (var zCircle = -radius; zCircle <= radius; zCircle++)
            {
                for (var xCircle = -radius; xCircle <= radius; xCircle++)
                {
                    if (xCircle * xCircle + zCircle * zCircle < radius * radius)
                        result.Add(new int[] { currentChunk[0] + xCircle, currentChunk[1] + zCircle });
                }
            }
            return result;
        }

        public void StopAllGenerateProcess()
        {
            foreach (Chunk chunk in existChunks) chunk.Stop(this);
            existChunks.Clear();
        }

        public void Dispose()
        {
            StopAllGenerateProcess();
            existChunks.Clear();
            existChunks = null;
            generator = null;
        }

        ~MapManager()
        {
            StopAllGenerateProcess();
        }
    }
}
