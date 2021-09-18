namespace PerlinNoiseGenerator.MapGen
{
    public struct NoiseGenConfig
    {
        //this is optimal but you can change this if you want
        public const int OctavesForSimplex = 10;
        public const int OctavesForNoise = 20;
        public const float Lacunarity = 0.72f;
        public const float Persistance = 1;
        public const float PersistanceForRivers = 0;
        public const float ScaleForNoise = 5;
    }
}