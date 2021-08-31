namespace PerlyNoizeGenerator
{ 
    public struct NoiseGenConfig
    {
        public float Persistance { get; }
        public float Lacunarity { get; }
        public int Octaves { get; }
        public float Scale { get; }
        public int MapSizeX { get; }
        public int MapSizeY { get; }

        public NoiseGenConfig(float persistance, float lacunarity, int octaves, float scale, int mapSizeX, int mapSizeY)
        {
            Persistance = persistance;
            Lacunarity = lacunarity;
            Octaves = octaves;
            Scale = scale;
            MapSizeX = mapSizeX;
            MapSizeY = mapSizeY;
        }
    }
}