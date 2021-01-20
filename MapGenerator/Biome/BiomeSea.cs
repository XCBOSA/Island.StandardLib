namespace Island.StandardLib.MapGenerator.Biome
{
    public sealed class BiomeSea : IBiomeGeneratorDelegate
    {
        public float GenerateHeightmap(float x, float z, double seedf, MapGen handle)
        {
            return PerlinNoise.Noise(x * 0.05f, z * 0.05f, (float)seedf * 38f) + 10f;
        }

        public string GetBiomeName()
        {
            return "Sea";
        }
    }
}
