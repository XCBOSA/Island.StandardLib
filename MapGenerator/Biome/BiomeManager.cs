namespace Island.StandardLib.MapGenerator.Biome
{
    public static class BiomeManager
    {
        public static readonly IBiomeGeneratorDelegate[] Biomes =
        {
            new BiomeFlat(),
            new BiomeMountain(),
            new BiomeSea()
        };

        public static string GetName(int index) => Biomes[index].GetBiomeName();
    }
}
