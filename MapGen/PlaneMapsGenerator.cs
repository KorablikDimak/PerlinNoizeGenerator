using System.Threading;

namespace PerlinNoiseGenerator.MapGen
{
    public class PlaneMapsGenerator : IMapsGenerator
    {
        public int MapSizeX { get; }
        public int MapSizeY { get; }
        public float Scale { get; }
        public int Seed { get; }
        public bool Rivers { get; }
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }

        public PlaneMapsGenerator(int mapSizeX, int mapSizeY, float scale, int seed, bool rivers)
        {
            MapSizeX = mapSizeX;
            MapSizeY = mapSizeY;
            Scale = scale;
            Seed = seed;
            Rivers = rivers;
        }

        public void CreateNoiseMap()
        {
            NoiseMap = new float[MapSizeX, MapSizeY];
            NoiseMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 5, Seed,
                    NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        public void CreateWeightMap()
        {
            WeightMap = new float[MapSizeX, MapSizeY];
            WeightMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 5, Seed + 1,
                    NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        public void CreateRiversMap()
        {
            if (!Rivers) return;
            RiversMap = new float[MapSizeX, MapSizeY];
            RiversMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 3, Seed + 2, NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance - 1, NoiseGenConfig.Lacunarity);
        }
    }
}