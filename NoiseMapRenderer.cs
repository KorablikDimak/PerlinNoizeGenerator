using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace PerlyNoizeGenerator
{
    public class NoiseMapRenderer : MonoBehaviour
    {
        //player will not change this:
        [SerializeField] private new Renderer renderer;
        private readonly MyColor[] _myColorArray = new MyColor[7];
        private int _mapSizeX;
        private int _mapSizeY;
        private float[,] _temperatureMap;
        private float[,] _weightMap;
        private float[,] _noiseMap;
        private delegate Color32 SetColor(int x, int y);
        
        //config:
        [SerializeField] private float upAll;
        [SerializeField] private float downCoast;
        [SerializeField] private float speed;

        //player will change this:
        [Range(0, 2)] public float temperature;
        [Range(0, 1)] public float weight;
        public TypeOfMap typeOfMap;
        [Range(-0.5f, 0.5f)] public float groundLevel;
        [Range(0, 0.5f)] public float waterLevel;
        public bool rivers;

        public enum TypeOfMap
        {
            Island,
            Temperature,
            Coast,
            Simple,
            Height,
            Weight
        }

        private void Start()
        {
            _myColorArray[0] = new MyColor(0.321f, new Color32(49, 49, 159, 255));
            _myColorArray[1] = new MyColor(0.37f, new Color32(0, 224, 255, 255));
            _myColorArray[2] = new MyColor(0.427f, new Color32(246, 255, 0, 255));
            _myColorArray[3] = new MyColor(0.557f, new Color32(0, 255, 0, 255));
            _myColorArray[4] = new MyColor(0.686f, new Color32(23, 137, 16, 255));
            _myColorArray[5] = new MyColor(0.839f, new Color32(152, 152, 152, 255));
            _myColorArray[6] = new MyColor(1f, new Color32(255, 255, 255, 255));
        }

        public void RendererNoiseMap(float[,] noiseMap, float[,] weightMap, float[,] riversMap)
        {
            _mapSizeX = noiseMap.GetLength(0);
            _mapSizeY = noiseMap.GetLength(1);
            _temperatureMap = new float[_mapSizeX, _mapSizeY];
            _weightMap = new float[_mapSizeX, _mapSizeY];
            _noiseMap = new float[_mapSizeX, _mapSizeY];
            Texture2D texture2D = new Texture2D(_mapSizeX, _mapSizeY);
            SetColor setColor;

            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    _noiseMap[x, y] = noiseMap[x, y] + groundLevel;
                    _weightMap[x, y] = weightMap[x, y] + weight / 2 + waterLevel / 2;
                    _temperatureMap[x, y] = TemperatureNoiseCreate(x, y);
                }
            });

            switch (typeOfMap)
            {
                case TypeOfMap.Island:
                    setColor = IslandCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                case TypeOfMap.Temperature:
                    setColor = TemperatureCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                case TypeOfMap.Coast:
                    setColor = CoastCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                case TypeOfMap.Simple:
                    setColor = SimpleCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                case TypeOfMap.Height:
                    setColor = HeightCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                case TypeOfMap.Weight:
                    setColor = WeightCreate;
                    SetPixelsToTexture(setColor, texture2D);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (rivers)
            {
                List<River> riversList = new List<River>();
                List<Vector2Int> riverPoints = new List<Vector2Int>();

                for (int x = 0; x < _mapSizeX; x++)
                {
                    for (int y = 0; y < _mapSizeY; y++)
                    {
                        if (!(_noiseMap[x, y] > _myColorArray[4].Level) || 
                            !(riversMap[x, y] > 0.7f) ||
                            !(_noiseMap[x, y] < _myColorArray[5].Level) || 
                            !(_weightMap[x, y] > 0.7f)) continue;
                        riverPoints.Add(new Vector2Int(x, y));
                    }
                }

                for (int i = 0; i < Random.Range(0, riverPoints.Count); i++)
                {
                    River riverToAdd = new River();
                    if (riverToAdd.RiverGen(riverPoints[Random.Range(0, riverPoints.Count)], _noiseMap, _myColorArray[1].Level))
                    {
                        riversList.Add(riverToAdd);
                    }
                }

                List<RiversGroup> riversGroups = RiversGroup.BuildRiversGroups(riversList);

                foreach (var riversGroup in riversGroups)
                {
                    int maxCount = int.MinValue;
                    River longestRiver = riversGroup.GroupOfRivers[0];
                        
                    foreach (var river in riversGroup.GroupOfRivers.Where(river => river.Positions.Count > maxCount))
                    {
                        maxCount = river.Positions.Count;
                        longestRiver = river;
                    }

                    foreach (var position in longestRiver.Positions)
                    {
                        texture2D.SetPixel(position.x, position.y, _myColorArray[1].Color);
                    }
                }
            }

            texture2D.Apply();
            renderer.sharedMaterial.mainTexture = texture2D;
            renderer.transform.localScale = new Vector3(_mapSizeX, 0, _mapSizeY);
        }

        private void SetPixelsToTexture(SetColor setColor, Texture2D texture2D)
        {
            Color[] colorsMap = new Color[_mapSizeX * _mapSizeY];
            
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    colorsMap[y * _mapSizeY + x] = setColor(x, y);
                }
            });
            
            texture2D.SetPixels(colorsMap);
        }
        
        private float TemperatureNoiseCreate(int x, int y)
        {
            float distanceFromEquator =
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, y)) /
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, 0));

            return Mathf.Sqrt(Mathf.Pow(_myColorArray[2].Level - _noiseMap[x, y], 2)) * temperature + distanceFromEquator * temperature;
        }

        private Color32 SimpleCreate(int x, int y)
        {
            return _temperatureMap[x, y] > 0.5 ? _myColorArray[_myColorArray.Length - 1].Color : GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private Color32 IslandCreate(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + upAll) * (1 - downCoast * Mathf.Pow(distanceToCenter, speed));

            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private Color32 CoastCreate(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + upAll) * (1 - downCoast * Mathf.Pow(distanceToCenter, speed));

            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private Color32 WeightCreate(int x, int y)
        {
            return Color32.Lerp(Color.white, Color.blue, _weightMap[x, y]);
        }

        private Color32 TemperatureCreate(int x, int y)
        {
            return Color32.Lerp(Color.red, Color.white, _temperatureMap[x, y]);
        }

        private Color32 HeightCreate(int x, int y)
        {
            return Color32.Lerp(Color.black, Color.white, _noiseMap[x, y]);
        }

        private Color32 GetPixelColor(float noise, float weightNoise)
        {
            if (noise < _myColorArray[0].Level + waterLevel)
            {
                if (weightNoise < 0.3)
                {
                    return _myColorArray[2].Color;
                }

                if (weightNoise < 0.5)
                {
                    return _myColorArray[1].Color;
                }

                return _myColorArray[0].Color;
            }

            if (noise < _myColorArray[1].Level + waterLevel)
            {
                if (weightNoise < 0.5)
                {
                    return _myColorArray[2].Color;
                }

                return _myColorArray[1].Color;
            }
            
            if (noise < _myColorArray[2].Level)
            {
                if (weightNoise < 0.8)
                {
                    return _myColorArray[2].Color;
                }

                return _myColorArray[3].Color;
            }
            
            if (noise < _myColorArray[3].Level)
            {
                if (weightNoise < 0.5)
                {
                    return _myColorArray[2].Color;
                }
                return _myColorArray[3].Color;
            }
            
            if (noise < _myColorArray[4].Level)
            {
                if (weightNoise < 0.5)
                {
                    return _myColorArray[3].Color;
                }

                if (weightNoise < 0.3)
                {
                    return _myColorArray[2].Color;
                }
                return _myColorArray[4].Color;
            }
            
            if (noise < _myColorArray[5].Level)
            {
                return _myColorArray[5].Color;
            }
            
            if (noise < _myColorArray[6].Level)
            {
                if (weightNoise < 0.5)
                {
                    return _myColorArray[5].Color;
                }
                return _myColorArray[6].Color;
            }
            
            return _myColorArray[6].Color;
        }
    }
}