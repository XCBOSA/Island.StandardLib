namespace Island.StandardLib.MapGenerator
{
    public interface IBiomeGeneratorDelegate
    {
        string GetBiomeName();
        float GenerateHeightmap(float x, float z, double seedf, MapGen handle);
    }
}
