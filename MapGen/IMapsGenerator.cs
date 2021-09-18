namespace PerlinNoiseGenerator.MapGen
{
    public interface IMapsGenerator
    {
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }
        
        void CreateNoiseMap();
        void CreateWeightMap();
        void CreateRiversMap();
    }
}