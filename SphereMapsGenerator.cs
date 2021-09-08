using System.Threading;

namespace PerlinNoiseGenerator
{
    public class SphereMapsGenerator : IMapsGenerator
    {
        public int MapSizeX { get; }
        public int MapSizeY { get; }
        public float Scale { get; }
        public int Seed { get; }
        public bool Rivers { get; }
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }

        public SphereMapsGenerator(int mapSizeX, int mapSizeY, float scale, int seed, bool rivers)
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
                Thread.Sleep(100);
            }
        }
        
        private void CreateNoiseMap()
        {
            float[,] noiseMap = Simplex.GenerateNoiseMap(MapSizeX, MapSizeY, Scale, NoiseGenConfig.Octaves, Seed);
            NoiseMap = TransformSphereMap.TransformNoiseMap(noiseMap, MapSizeX, MapSizeY);
        }
        
        private void CreateWeightMap()
        {
            float[,] weightMap = Simplex.GenerateNoiseMap(MapSizeX, MapSizeY, Scale, NoiseGenConfig.Octaves, Seed + 1);
            WeightMap = TransformSphereMap.TransformNoiseMap(weightMap, MapSizeX, MapSizeY);
        }
        
        private void CreateRiversMap()
        {
            if (!Rivers) return;
            float[,] riversMap = Noise.GenerateNoiseMap(MapSizeX * 4, MapSizeY * 4, Scale + 3, Seed + 2, NoiseGenConfig.Octaves + 12, NoiseGenConfig.Persistance - 1, NoiseGenConfig.Lacunarity);
            RiversMap = TransformSphereMap.TransformNoiseMap(riversMap, MapSizeX, MapSizeY);
        }
    }
}