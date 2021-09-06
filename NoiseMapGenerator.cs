using System.Threading;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        private float[,] _noiseMap;
        private float[,] _weightMap;
        private float[,] _riversMap;
        private float[,] _transformedNoiseMap;
        private float[,] _transformedweightMap;
        private float[,] _transformedriversMap;
        
        //config:
        private float _scale = 0.012f;
        private const int Octaves = 10;
        private const float Lacunarity = 0.72f;
        private int _mapSizeX;
        private int _mapSizeY;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.G)) return;
            StopAllCoroutines();
            GenerateMap();
        }
        
        private void GenerateMap()
        {
            switch (noiseMapRenderer.mapSize)
            {
                case NoiseMapRenderer.MapSize.Small:
                    _mapSizeX = 150;
                    _mapSizeY = 150;
                    _scale = 0.006f;
                    break;
                case NoiseMapRenderer.MapSize.Medium:
                    _mapSizeX = 300;
                    _mapSizeY = 300;
                    _scale = 0.005f;
                    break;
                case NoiseMapRenderer.MapSize.Large:
                    _mapSizeX = 500;
                    _mapSizeY = 500;
                    _scale = 0.004f;
                    break;
                case NoiseMapRenderer.MapSize.Giant:
                    _mapSizeX = 1000;
                    _mapSizeY = 1000;
                    _scale = 0.0033f;
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
                   thread3.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(100);
            }
            noiseMapRenderer.RendererNoiseMap(_transformedNoiseMap, _transformedweightMap, _transformedriversMap);
        }

        private void NoiseMapCreate()
        {
            _noiseMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _noiseMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, noiseMapRenderer.seed);
            _transformedNoiseMap = new float[_mapSizeX, _mapSizeY];
            _transformedNoiseMap = TransformSphereMap.TransformNoiseMap(_noiseMap, _mapSizeX, _mapSizeY);
        }

        private void WeightMapCreate()
        {
            _weightMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _weightMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, noiseMapRenderer.seed + 1);
            _transformedweightMap = new float[_mapSizeX, _mapSizeY];
            _transformedweightMap = TransformSphereMap.TransformNoiseMap(_weightMap, _mapSizeX, _mapSizeY);
        }

        private void RiversMapCreate()
        {
            if (!noiseMapRenderer.rivers) return;
            _riversMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _riversMap = Noise.GenerateNoiseMap(_mapSizeX * 4, _mapSizeY * 4, 3, noiseMapRenderer.seed + 2, 10, 0, Lacunarity);
            _transformedriversMap = new float[_mapSizeX, _mapSizeY];
            _transformedriversMap = TransformSphereMap.TransformNoiseMap(_riversMap, _mapSizeX, _mapSizeY);
        }
    }
}