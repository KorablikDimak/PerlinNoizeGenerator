using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
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
        private Coroutine _rotateCoroutine;
        private float _scale = 0.012f;
        private int _mapSizeX;
        private int _mapSizeY;
        
        //player will change this:
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
            sphereRenderer.enabled = false;
            planeRenderer.enabled = false;
            foreach (var rendererForPlane in renderersForPlane)
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
            
            StopRotation();
            SwitchMapSize();
            GenerateMaps();
            loadIndicator.AddProgress(0.5f);
            loadIndicator.SetText("создание текстур");
            yield return null;

            RenderMap();
            yield return null;
            
            loadIndicator.SetCheckSprite();
            loadIndicator.SetText("готово");
            yield return null;
        }

        private void StopRotation()
        {
            if (_rotateCoroutine != null)
            {
                StopCoroutine(_rotateCoroutine);
            }

            noiseMapRenderer.transform.rotation = new Quaternion();
            planeRenderer.transform.rotation = new Quaternion();
        }
        
        private void SwitchMapSize()
        {
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
        }

        private void GenerateMaps()
        {
            switch (typeOfRenderer)
            {
                case TypeOfRenderer.Sphere:
                    foreach (var rendererForPlane in renderersForPlane)
                    {
                        rendererForPlane.enabled = false;
                    }
                    planeRenderer.enabled = false;
                    sphereRenderer.enabled = true;
                    rotator.thisTransform = noiseMapRenderer.transform;
                    _mapsGenerator = new SphereMapsGenerator(_mapSizeX, _mapSizeY, _scale, seed, isRiversOn.isOn);
                    _mapsGenerator.GenerateMaps();
                    break;
                case TypeOfRenderer.Plane:
                    foreach (var rendererForPlane in renderersForPlane)
                    {
                        rendererForPlane.enabled = true;
                    }
                    planeRenderer.enabled = true;
                    sphereRenderer.enabled = false;
                    rotator.thisTransform = planeRenderer.transform;
                    _mapsGenerator = new PlaneMapsGenerator(_mapSizeX, _mapSizeY, _scale, seed, isRiversOn.isOn);
                    _mapsGenerator.GenerateMaps();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RenderMap()
        {
            StartCoroutine(noiseMapRenderer.RenderNoiseMap(_mapsGenerator.NoiseMap, _mapsGenerator.WeightMap, _mapsGenerator.RiversMap));
            _rotateCoroutine = StartCoroutine(rotator.RotateTransform());
        }
    }
}