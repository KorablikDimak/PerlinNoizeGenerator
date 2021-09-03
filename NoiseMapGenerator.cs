using System.Threading;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        //player will not change this:
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        private float[,] _noiseMap;
        private float[,] _weightMap;
        private float[,] _riversMap;
        
        //config:
        [Range(0, 0.025f)] [SerializeField] private float scale = 0.012f;
        private const int Octaves = 8;
        private const float Persistance = 0;
        private const float Lacunarity = 0.72f;
        private int _mapSizeX;
        private int _mapSizeY;

        //player will change this:
        public bool autoUpdate;
        public int seed;
        public MapSize mapSize;

        public enum MapSize
        {
            Small,
            Medium,
            Large,
            Giant
        }

        private void Update()
        {
            if (autoUpdate && _mapSizeX * _mapSizeY <= 300 * 300 && !noiseMapRenderer.rivers)
            {
                GenerateMap();
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                GenerateMap();
            }
        }
        
        private void GenerateMap()
        {
            switch (mapSize)
            {
                case MapSize.Small:
                    _mapSizeX = 150;
                    _mapSizeY = 150;
                    break;
                case MapSize.Medium:
                    _mapSizeX = 300;
                    _mapSizeY = 300;
                    break;
                case MapSize.Large:
                    _mapSizeX = 500;
                    _mapSizeY = 500;
                    break;
                case MapSize.Giant:
                    _mapSizeX = 1000;
                    _mapSizeY = 1000;
                    break;
                default:
                    return;
            }

            var thread1 = new Thread(NoiseMapCreate);
            thread1.Start();
            var thread2 = new Thread(WeightMapCreate);
            thread2.Start();
            var thread3 = new Thread(RiversMapCreate);
            thread3.Start();
            while (thread1.ThreadState != ThreadState.Stopped ||
                   thread2.ThreadState != ThreadState.Stopped ||
                   thread3.ThreadState != ThreadState.Stopped) { }
            NoiseMapCreate();
            noiseMapRenderer.RendererNoiseMap(_noiseMap, _weightMap, _riversMap);
        }

        private void NoiseMapCreate()
        {
            _noiseMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _noiseMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, scale);
        }

        private void WeightMapCreate()
        {
            _weightMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _weightMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, scale);
        }

        private void RiversMapCreate()
        {
            if (!noiseMapRenderer.rivers) return;
            _riversMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _riversMap = Noise.GenerateNoiseMap(_mapSizeX * 4, _mapSizeY * 4, scale + 5, seed, Octaves + 10, Persistance, Lacunarity);
        }
    }
}