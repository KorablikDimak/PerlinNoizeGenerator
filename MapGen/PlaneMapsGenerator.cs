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

        public void GenerateMaps()
        {
            var thread1 = new Thread(CreateNoiseMap);
            thread1.Start();
            var thread2 = new Thread(CreateWeightMap);
            thread2.Start();
            var thread3 = new Thread(CreateRiversMap);
            thread3.Start();
            while (thread1.ThreadState != ThreadState.Stopped ||
                   thread2.ThreadState != ThreadState.Stopped ||
                   thread3.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(50);
            }
        }
        
        private void CreateNoiseMap()
        {
            NoiseMap = new float[MapSizeX, MapSizeY];
            NoiseMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 5, Seed,
                    NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        private void CreateWeightMap()
        {
            WeightMap = new float[MapSizeX, MapSizeY];
            WeightMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 5, Seed + 1,
                    NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance, NoiseGenConfig.Lacunarity);
        }

        private void CreateRiversMap()
        {
            if (!Rivers) return;
            RiversMap = new float[MapSizeX, MapSizeY];
            RiversMap = Noise.GenerateNoiseMap(MapSizeX, MapSizeY, Scale + 3, Seed + 2, NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance - 1, NoiseGenConfig.Lacunarity);
        }
    }
}