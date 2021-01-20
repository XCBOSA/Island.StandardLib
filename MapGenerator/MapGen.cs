using Island.StandardLib.MapGenerator.Biome;
using Island.StandardLib.Math;
using System;

namespace Island.StandardLib.MapGenerator
{
    public class MapGen
    {
        Random rd;
        double useZ;
        double[] biomeSeeds;
        double[] childBiomeSeeds;
        public float[] TempGeneratedHeightmap;

        public MapGen(int seed)
        {
            TempGeneratedHeightmap = new float[BiomeManager.Biomes.Length];
            rd = new Random(seed);
            useZ = rd.NextDouble() * 1000f;
            biomeSeeds = new double[BiomeManager.Biomes.Length];
            childBiomeSeeds = new double[BiomeManager.Biomes.Length];
            for (int i = 0; i < biomeSeeds.Length; i++) biomeSeeds[i] = rd.NextDouble() * 1000f;
            for (int i = 0; i < childBiomeSeeds.Length; i++) childBiomeSeeds[i] = rd.NextDouble() * 1000f;
        }

        float GenerateHeightmap(float x, float y, int p) => BiomeManager.Biomes[p].GenerateHeightmap(x, y, useZ, this);

        public Percentage GetBiome(float x, float y)
        {
            float[] p = new float[BiomeManager.Biomes.Length];
            for (int i = 0; i < p.Length; i++) p[i] = (float)System.Math.Pow(1f / PerlinNoiseDouble.Noise(x * 0.0005d, y * 0.0005d, biomeSeeds[i]) - 1f, 5);
            return new Percentage(p);
        }

        public Percentage GetChildBiome(float x, float y, int split, double sharpness = 5d)
        {
            float[] p = new float[split];
            for (int i = 0; i < p.Length; i++) p[i] = (float)System.Math.Pow(1f / PerlinNoiseDouble.Noise(x * 0.0005d, y * 0.0005d, childBiomeSeeds[i]) - 1f, sharpness);
            return new Percentage(p);
        }

        public Percentage GetChildBiomeWithScale(float x, float y, float scale, int split, double sharpness = 5d)
        {
            float[] p = new float[split];
            for (int i = 0; i < p.Length; i++) p[i] = (float)System.Math.Pow(1f / PerlinNoiseDouble.Noise(x * 0.0005d * scale, y * 0.0005d * scale, childBiomeSeeds[i]) - 1f, sharpness);
            return new Percentage(p);
        }

        float Clamp0n(float inx) => inx < 0 ? 0 : inx;
        float Clampl1(float inx) => inx > 1 ? 1 : inx;

        public float Gen(float x, float y)
        {
            Percentage percentage = GetBiome(x, y);
            float val = 0f;
            for (int i = 0; i < percentage.KeyLength; i++)
            {
                float thisconn = percentage[i] * GenerateHeightmap(x, y, i);
                TempGeneratedHeightmap[i] = thisconn;
                val += thisconn;
            }
            return Clamp0n(val);
        }

        public float Gen01(float x, float y) => Clampl1(Gen(x, y) / 600f);

        public float Gen_KnownBiome(float x, float y, Percentage biome)
        {
            float val = 0f;
            for (int i = 0; i < biome.KeyLength; i++)
            {
                float thisconn = biome[i] * GenerateHeightmap(x, y, i);
                TempGeneratedHeightmap[i] = thisconn;
                val += thisconn;
            }
            return Clamp0n(val);
        }

        public float Gen01_KnownBiome(float x, float y, Percentage biome) => Clampl1(Gen_KnownBiome(x, y, biome) / 600f);
    }
}
