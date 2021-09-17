namespace PerlinNoiseGenerator.MapGen
{
    public struct NoiseGenConfig
    {
        //this is optimal but you can change this if you want
        public const int Octaves = 10;
        public const float Lacunarity = 0.72f;
        public const float Persistance = 1;
    }
}