namespace PerlinNoiseGenerator.MapGen
{
    public class PlaneMapsGenerator : IMapsGenerator
    {
        private readonly int _mapSizeX;
        private readonly int _mapSizeY;
        private readonly int _seed;
        private readonly bool _rivers;
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }

        public PlaneMapsGenerator(int mapSizeX, int mapSizeY, int seed, bool rivers)
        {
            _mapSizeX = mapSizeX;
            _mapSizeY = mapSizeY;
            _seed = seed;
            _rivers = rivers;
        }

        public void CreateNoiseMap()
        {
            NoiseMap = new float[_mapSizeX, _mapSizeY];
            NoiseMap = Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, NoiseGenConfig.ScaleForNoise, _seed,
                    NoiseGenConfig.OctavesForNoise, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        public void CreateWeightMap()
        {
            WeightMap = new float[_mapSizeX, _mapSizeY];
            WeightMap = Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, NoiseGenConfig.ScaleForNoise, _seed + 1, 
                    NoiseGenConfig.OctavesForNoise, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        public void CreateRiversMap()
        {
            if (!_rivers) return;
            RiversMap = new float[_mapSizeX, _mapSizeY];
            RiversMap = Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, NoiseGenConfig.ScaleForNoise, _seed + 2, 
                NoiseGenConfig.OctavesForNoise, NoiseGenConfig.PersistanceForRivers, NoiseGenConfig.Lacunarity);
        }
    }
}