using System;
using System.Threading;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        private int _mapSizeX;
        private int _mapSizeY;
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
        private const float Persistance = 1;

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

            switch (noiseMapRenderer.typeOfRenderer)
            {
                case NoiseMapRenderer.TypeOfRenderer.Plane:
                    var thread1 = new Thread(NoiseMapCreateSphere);
                    thread1.Start();
                    var thread2 = new Thread(WeightMapCreateSphere);
                    thread2.Start();
                    var thread3 = new Thread(RiversMapCreate);
                    thread3.Start();
                    while (thread1.ThreadState != ThreadState.Stopped ||
                           thread2.ThreadState != ThreadState.Stopped ||
                           thread3.ThreadState != ThreadState.Stopped)
                    {
                        Thread.Sleep(100);
                    }
                    break;
                case NoiseMapRenderer.TypeOfRenderer.Sphere:
                    var thread11 = new Thread(NoiseMapCreatePlane);
                    thread11.Start();
                    var thread22 = new Thread(WeightMapCreatePlane);
                    thread22.Start();
                    var thread33 = new Thread(RiversMapCreate);
                    thread33.Start();
                    while (thread11.ThreadState != ThreadState.Stopped ||
                           thread22.ThreadState != ThreadState.Stopped ||
                           thread33.ThreadState != ThreadState.Stopped)
                    {
                        Thread.Sleep(100);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            noiseMapRenderer.RendererNoiseMap(_transformedNoiseMap, _transformedweightMap, _transformedriversMap);
        }

        private void NoiseMapCreateSphere()
        {
            _noiseMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _noiseMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, noiseMapRenderer.seed);
            _transformedNoiseMap = new float[_mapSizeX, _mapSizeY];
            _transformedNoiseMap = TransformSphereMap.TransformNoiseMap(_noiseMap, _mapSizeX, _mapSizeY);
        }

        private void NoiseMapCreatePlane()
        {
            _transformedNoiseMap = new float[_mapSizeX, _mapSizeY];
            _transformedNoiseMap = 
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale + 5, noiseMapRenderer.seed,
                Octaves + 12, Persistance, Lacunarity);
        }

        private void WeightMapCreateSphere()
        {
            _weightMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _weightMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, noiseMapRenderer.seed + 1);
            _transformedweightMap = new float[_mapSizeX, _mapSizeY];
            _transformedweightMap = TransformSphereMap.TransformNoiseMap(_weightMap, _mapSizeX, _mapSizeY);
        }
        
        private void WeightMapCreatePlane()
        {
            _transformedweightMap = new float[_mapSizeX, _mapSizeY];
            _transformedweightMap = 
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale + 5, noiseMapRenderer.seed + 1,
                    Octaves + 12, Persistance, Lacunarity);
        }

        private void RiversMapCreate()
        {
            if (!noiseMapRenderer.rivers) return;
            _riversMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _riversMap = Noise.GenerateNoiseMap(_mapSizeX * 4, _mapSizeY * 4, _scale + 3, noiseMapRenderer.seed + 2, Octaves + 12, Persistance - 1, Lacunarity);
            _transformedriversMap = new float[_mapSizeX, _mapSizeY];
            _transformedriversMap = TransformSphereMap.TransformNoiseMap(_riversMap, _mapSizeX, _mapSizeY);
        }
    }
}