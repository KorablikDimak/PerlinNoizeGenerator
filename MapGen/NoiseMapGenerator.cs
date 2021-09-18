using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PerlinNoiseGenerator.RenderMap;
using PerlinNoiseGenerator.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator.MapGen
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private LoadIndicator loadIndicator;
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        [SerializeField] private Renderer planeRenderer;
        [SerializeField] private Renderer sphereRenderer;
        [SerializeField] private Rotator rotator;
        [SerializeField] private List<Renderer> renderersForPlane;
        [SerializeField] private Toggle isRiversOn;
        private IMapsGenerator _mapsGenerator;
        private float _scale;
        private int _mapSizeX;
        private int _mapSizeY;
        
        public int Seed { get; set; }
        public MapSize CurrentMapSize { get; set; }
        public TypeOfRenderer CurrentTypeOfRenderer { get; set; }
        
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
            sphereRenderer.enabled = false;
            planeRenderer.enabled = false;
            foreach (var rendererForPlane in renderersForPlane)
            {
                rendererForPlane.enabled = false;
            }
            rotator.ThisTransform = noiseMapRenderer.transform;
            
            loadIndicator.RemoveProgress();
            loadIndicator.SetDefaultSprite();
            StartCoroutine(loadIndicator.AsyncLoadingImage());
        }

        public IEnumerator GenerateAll()
        {
            loadIndicator.RemoveProgress();
            loadIndicator.SetText("генерация карт высот");
            loadIndicator.SetDefaultSprite();
            rotator.StopRotation();
            yield return null;
            
            SwitchMapSize();
            yield return null;
            
            SwitchMapsGenerator();
            yield return null;

            var thread1 = new Thread(_mapsGenerator.CreateNoiseMap);
            thread1.Start();
            var thread2 = new Thread(_mapsGenerator.CreateWeightMap);
            thread2.Start();
            var thread3 = new Thread(_mapsGenerator.CreateRiversMap);
            thread3.Start();
            loadIndicator.AddProgress(0.1f);
            yield return null;
            
            while (thread1.ThreadState != ThreadState.Stopped ||
                   thread2.ThreadState != ThreadState.Stopped ||
                   thread3.ThreadState != ThreadState.Stopped)
            {
                yield return null;
            }
            loadIndicator.AddProgress(0.4f);
            loadIndicator.SetText("создание текстур");
            yield return null;

            RenderMap();
            yield return null;
            
            loadIndicator.SetCheckSprite();
            loadIndicator.SetText("готово");
            yield return null;
        }

        private void SwitchMapSize()
        {
            switch (CurrentMapSize)
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
        }

        private void SwitchMapsGenerator()
        {
            switch (CurrentTypeOfRenderer)
            {
                case TypeOfRenderer.Sphere:
                    foreach (var rendererForPlane in renderersForPlane)
                    {
                        rendererForPlane.enabled = false;
                    }
                    planeRenderer.enabled = false;
                    sphereRenderer.enabled = true;
                    rotator.ThisTransform = noiseMapRenderer.transform;
                    _mapsGenerator = new SphereMapsGenerator(_mapSizeX, _mapSizeY, _scale, Seed, isRiversOn.isOn);
                    break;
                case TypeOfRenderer.Plane:
                    foreach (var rendererForPlane in renderersForPlane)
                    {
                        rendererForPlane.enabled = true;
                    }
                    planeRenderer.enabled = true;
                    sphereRenderer.enabled = false;
                    rotator.ThisTransform = planeRenderer.transform;
                    _mapsGenerator = new PlaneMapsGenerator(_mapSizeX, _mapSizeY, Seed, isRiversOn.isOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RenderMap()
        {
            StartCoroutine(noiseMapRenderer.RenderNoiseMap(_mapsGenerator.NoiseMap, _mapsGenerator.WeightMap, _mapsGenerator.RiversMap));
            rotator.StartRotation();
        }
    }
}