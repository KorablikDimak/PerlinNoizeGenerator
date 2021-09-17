namespace PerlinNoiseGenerator.MapGen
{
    public interface IMapsGenerator
    {
        public int MapSizeX { get; }
        public int MapSizeY { get; }
        public float Scale { get; }
        public int Seed { get; }
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }
        public bool Rivers { get; }
        
        void CreateNoiseMap();
        void CreateWeightMap();
        void CreateRiversMap();
    }
}