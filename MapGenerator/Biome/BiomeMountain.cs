using Island.StandardLib.Math;

namespace Island.StandardLib.MapGenerator.Biome
{
    public sealed class BiomeMountain : IBiomeGeneratorDelegate
    {
        public float GenerateHeightmap(float x, float z, double seedf, MapGen handle)
        {
            Percentage p = handle.GetChildBiomeWithScale(x, z, 20, 3);
            float mountain7 = (float)(PerlinNoiseDouble.NoiseOctave(x * 0.005d, z * 0.005d, seedf, 7) * 500d) + 100f;
            float mountain5 = (float)(PerlinNoiseDouble.NoiseOctave(x * 0.005d, z * 0.005d, seedf, 5) * 500d) + 100f;
            return (1 - p[0]) * mountain7 + p[0] * mountain5;
        }

        public string GetBiomeName()
        {
            return "Mountain";
        }
    }
}
