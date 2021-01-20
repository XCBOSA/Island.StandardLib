namespace Island.StandardLib.MapGenerator.Biome
{
    public sealed class BiomeFlat : IBiomeGeneratorDelegate
    {
        public float GenerateHeightmap(float x, float z, double seedf, MapGen handle)
        {
            return PerlinNoise.Noise(x * 0.05f, z * 0.05f, (float)seedf * 20f) + 80f;
        }

        public string GetBiomeName()
        {
            return "Flat";
        }
    }
}
