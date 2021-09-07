using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PerlinNoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private LoadIndicator loadIndicator;
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        [SerializeField] private Renderer planeRenderer;
        [SerializeField] private Rotator rotator;
        [SerializeField] private List<Renderer> renderers;
        private Coroutine _rotateCoroutine;
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
        
        //player will change this:
        [Range(0, 0.1f)] public float rotateSpeed;
        public int seed;
        public MapSize mapSize;
        public TypeOfRenderer typeOfRenderer;
        
        public enum TypeOfRenderer
        {
            Plane,
            Sphere
        }
        
        public enum MapSize
        {
            Small,
            Medium,
            Large,
            Giant
        }

        private void Start()
        {
            noiseMapRenderer.renderer.enabled = false;
            planeRenderer.enabled = false;
            foreach (var rendererForPlane in renderers)
            {
                rendererForPlane.enabled = false;
            }
            rotator.thisTransform = noiseMapRenderer.transform;
            
            loadIndicator.RemoveProgress();
            loadIndicator.SetDefaultSprite();
            StartCoroutine(loadIndicator.AsyncLoadingImage());
        }

        public IEnumerator GenerateAll()
        {
            loadIndicator.RemoveProgress();
            loadIndicator.SetText("генерация карт высот");
            loadIndicator.SetDefaultSprite();
            yield return null;
            
            GenerateMaps();
            loadIndicator.AddProgress(0.3f);
            loadIndicator.SetText("сферическое преобразование");
            yield return null;
            
            switch (typeOfRenderer)
            {
                case TypeOfRenderer.Plane:
                    break;
                case TypeOfRenderer.Sphere:
                    TransformMaps();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            loadIndicator.AddProgress(0.2f);
            loadIndicator.SetText("создание текстур");
            yield return null;
            
            RenderMap();
            yield return null;
            
            loadIndicator.SetCheckSprite();
            loadIndicator.SetText("готово");
            yield return null;
        }
        
        private void GenerateMaps()
        {
            if (_rotateCoroutine != null)
            {
                StopCoroutine(_rotateCoroutine);
            }

            noiseMapRenderer.transform.rotation = new Quaternion(0, 0, 0, 0);
            planeRenderer.transform.rotation = new Quaternion(0, 0, 0, 0);
            
            switch (mapSize)
            {
                case MapSize.Small:
                    _mapSizeX = 150;
                    _mapSizeY = 150;
                    _scale = 0.006f;
                    break;
                case MapSize.Medium:
                    _mapSizeX = 300;
                    _mapSizeY = 300;
                    _scale = 0.005f;
                    break;
                case MapSize.Large:
                    _mapSizeX = 500;
                    _mapSizeY = 500;
                    _scale = 0.004f;
                    break;
                case MapSize.Giant:
                    _mapSizeX = 1000;
                    _mapSizeY = 1000;
                    _scale = 0.0033f;
                    break;
                default:
                    return;
            }

            switch (typeOfRenderer)
            {
                case TypeOfRenderer.Sphere:
                    foreach (var rendererForPlane in renderers)
                    {
                        rendererForPlane.enabled = false;
                    }
                    planeRenderer.enabled = false;
                    noiseMapRenderer.renderer.enabled = true;
                    rotator.thisTransform = noiseMapRenderer.transform;
                    var thread1 = new Thread(NoiseMapCreateSphere);
                    thread1.Start();
                    var thread2 = new Thread(WeightMapCreateSphere);
                    thread2.Start();
                    var thread3 = new Thread(RiversMapCreateSphere);
                    thread3.Start();
                    while (thread1.ThreadState != ThreadState.Stopped ||
                           thread2.ThreadState != ThreadState.Stopped ||
                           thread3.ThreadState != ThreadState.Stopped)
                    {
                        Thread.Sleep(100);
                    }
                    break;
                case TypeOfRenderer.Plane:
                    foreach (var rendererForPlane in renderers)
                    {
                        rendererForPlane.enabled = true;
                    }
                    planeRenderer.enabled = true;
                    noiseMapRenderer.renderer.enabled = false;
                    rotator.thisTransform = planeRenderer.transform;
                    var thread11 = new Thread(NoiseMapCreatePlane);
                    thread11.Start();
                    var thread22 = new Thread(WeightMapCreatePlane);
                    thread22.Start();
                    var thread33 = new Thread(RiversMapCreatePlane);
                    thread33.Start();
                    while (thread11.ThreadState != ThreadState.Stopped ||
                           thread22.ThreadState != ThreadState.Stopped ||
                           thread33.ThreadState != ThreadState.Stopped)
                    {
                        Thread.Sleep(50);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TransformMaps()
        {
            var thread1 = new Thread(NoiseMapTransform);
            thread1.Start();
            var thread2 = new Thread(WeightMapTransform);
            thread2.Start();
            var thread3 = new Thread(RiversMapTransform);
            thread3.Start();
            while (thread1.ThreadState != ThreadState.Stopped ||
                   thread2.ThreadState != ThreadState.Stopped ||
                   thread3.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(100);
            }
        }

        private void RenderMap()
        {
            StartCoroutine(noiseMapRenderer.RendererNoiseMap(_transformedNoiseMap, _transformedweightMap, _transformedriversMap));
            _rotateCoroutine = typeOfRenderer switch
            {
                TypeOfRenderer.Plane => StartCoroutine(RotateTransform(planeRenderer.transform)),
                TypeOfRenderer.Sphere => StartCoroutine(RotateTransform(noiseMapRenderer.transform)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void NoiseMapCreateSphere()
        {
            _noiseMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _noiseMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, seed);
        }

        private void NoiseMapTransform()
        {
            _transformedNoiseMap = new float[_mapSizeX, _mapSizeY];
            _transformedNoiseMap = TransformSphereMap.TransformNoiseMap(_noiseMap, _mapSizeX, _mapSizeY);
        }

        private void NoiseMapCreatePlane()
        {
            _transformedNoiseMap = new float[_mapSizeX, _mapSizeY];
            _transformedNoiseMap = 
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale + 5, seed,
                Octaves + 12, Persistance, Lacunarity);
        }

        private void WeightMapCreateSphere()
        {
            _weightMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _weightMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, Octaves, seed + 1);
        }

        private void WeightMapTransform()
        {
            _transformedweightMap = new float[_mapSizeX, _mapSizeY];
            _transformedweightMap = TransformSphereMap.TransformNoiseMap(_weightMap, _mapSizeX, _mapSizeY);
        }
        
        private void WeightMapCreatePlane()
        {
            _transformedweightMap = new float[_mapSizeX, _mapSizeY];
            _transformedweightMap = 
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale + 5, seed + 1,
                    Octaves + 12, Persistance, Lacunarity);
        }

        private void RiversMapCreateSphere()
        {
            if (!noiseMapRenderer.rivers) return;
            _riversMap = new float[_mapSizeX * 4, _mapSizeY * 4];
            _riversMap = Noise.GenerateNoiseMap(_mapSizeX * 4, _mapSizeY * 4, _scale + 3, seed + 2, Octaves + 12, Persistance - 1, Lacunarity);
        }

        private void RiversMapTransform()
        {
            if (!noiseMapRenderer.rivers) return;
            _transformedriversMap = new float[_mapSizeX, _mapSizeY];
            _transformedriversMap = TransformSphereMap.TransformNoiseMap(_riversMap, _mapSizeX, _mapSizeY);
        }
        
        private void RiversMapCreatePlane()
        {
            if (!noiseMapRenderer.rivers) return;
            _transformedriversMap = new float[_mapSizeX, _mapSizeY];
            _transformedriversMap = Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale + 3, seed + 2, Octaves + 12, Persistance - 1, Lacunarity);
        }
        
        private IEnumerator RotateTransform(Transform thisTransform)
        {
            while (true)
            {
                thisTransform.Rotate(0, rotateSpeed, 0);
                yield return new WaitForSeconds(1f / 30f);
            }
        }
    }
}