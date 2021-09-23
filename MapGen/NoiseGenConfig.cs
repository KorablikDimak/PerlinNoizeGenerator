namespace PerlinNoiseGenerator.MapGen
{
    /// <summary>
    /// Global constants for the generator.
    /// The values are selected optimal so it is not recommended to change them in the source code.
    /// </summary>
    public struct NoiseGenConfig
    {
        public const int OctavesForSimplex = 10;
        public const int OctavesForNoise = 20;
        public const float Lacunarity = 0.72f;
        public const float Persistance = 1;
        public const float PersistanceForRivers = 0;
        public const float ScaleForNoise = 5;
    }
}