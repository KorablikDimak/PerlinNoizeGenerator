using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerlinNoiseGenerator
{
    public class NoiseMapRenderer : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        private readonly MyColor[] _myColorArray = new MyColor[7];
        private readonly MyColor[] _myColorWeightArray = new MyColor[7];
        private readonly MyColor[] _myColorTemperatureArray = new MyColor[7];
        private readonly MyColor[] _myColorHeightArray = new MyColor[7];
        private int _mapSizeX;
        private int _mapSizeY;
        private float[,] _temperatureMap;
        private float[,] _weightMap;
        private float[,] _noiseMap;
        private float[,] _riversMap;
        private Texture2D _texture2D;
        private delegate Color32 SetColor(int x, int y);
        private delegate void SetNoise(int x, int y);
        
        //config:
        private const float NormalScale = 0.62f;
        private const float HeightScale = 0.03f;
        private const float UpAll = 0.05f;
        private const float DownCoast = 1.6f;
        private const float Speed = 2.5f;

        //player will change this:
        [Range(0, 1)] public float rotateSpeed;
        [Range(0, 2)] public float temperature;
        [Range(0, 1)] public float weight;
        public TypeOfMap typeOfMap;
        [Range(-0.5f, 0.5f)] public float groundLevel;
        [Range(0, 0.5f)] public float waterLevel;
        public bool rivers;
        public int seed;
        public MapSize mapSize;
        public TypeOfRenderer typeOfRenderer;
        
        public enum TypeOfRenderer
        {
            Plane,
            Sphere
        }

        public enum TypeOfMap
        {
            Island,
            Temperature,
            Coast,
            Simple,
            Height,
            Weight
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
            _myColorArray[0] = new MyColor(0.301f, new Color32(49, 49, 159, 255));
            _myColorArray[1] = new MyColor(0.34f, new Color32(0, 224, 255, 255));
            _myColorArray[2] = new MyColor(0.401f, new Color32(246, 255, 0, 255));
            _myColorArray[3] = new MyColor(0.547f, new Color32(0, 255, 0, 255));
            _myColorArray[4] = new MyColor(0.703f, new Color32(23, 137, 16, 255));
            _myColorArray[5] = new MyColor(0.914f, new Color32(152, 152, 152, 255));
            _myColorArray[6] = new MyColor(1f, new Color32(255, 255, 255, 255));
            
            _myColorWeightArray[0] = new MyColor(0.314f, new Color32(255, 0, 0, 255));
            _myColorWeightArray[1] = new MyColor(0.409f, new Color32(255, 144, 0, 255));
            _myColorWeightArray[2] = new MyColor(0.529f, new Color32(224, 255, 0, 255));
            _myColorWeightArray[3] = new MyColor(0.691f, new Color32(0, 255, 242, 255));
            _myColorWeightArray[4] = new MyColor(0.789f, new Color32(0, 145, 255, 255));
            _myColorWeightArray[5] = new MyColor(0.896f, new Color32(0, 100, 255, 255));
            _myColorWeightArray[6] = new MyColor(1f, new Color32(0, 0, 255, 255));
            
            _myColorTemperatureArray[0] = new MyColor(0.1f, new Color32(255, 0, 0, 255));
            _myColorTemperatureArray[1] = new MyColor(0.18f, new Color32(255, 75, 0, 255));
            _myColorTemperatureArray[2] = new MyColor(0.29f, new Color32(255, 150, 0, 255));
            _myColorTemperatureArray[3] = new MyColor(0.38f, new Color32(255, 255, 0, 255));
            _myColorTemperatureArray[4] = new MyColor(0.47f, new Color32(255, 255, 75, 255));
            _myColorTemperatureArray[5] = new MyColor(0.55f, new Color32(255, 255, 150, 255));
            _myColorTemperatureArray[6] = new MyColor(1f, new Color32(255, 255, 255, 255));
            
            _myColorHeightArray[0] = new MyColor(0.301f, new Color32(0, 0, 0, 255));
            _myColorHeightArray[1] = new MyColor(0.34f, new Color32(30, 30, 30, 255));
            _myColorHeightArray[2] = new MyColor(0.401f, new Color32(60, 60, 60, 255));
            _myColorHeightArray[3] = new MyColor(0.547f, new Color32(100, 100, 100, 255));
            _myColorHeightArray[4] = new MyColor(0.703f, new Color32(150, 150, 150, 255));
            _myColorHeightArray[5] = new MyColor(0.914f, new Color32(200, 200, 200, 255));
            _myColorHeightArray[6] = new MyColor(1f, new Color32(255, 255, 255, 255));
        }

        private IEnumerator RotateSphere()
        {
            while (true)
            {
                transform.Rotate(0, 0.1f * rotateSpeed, 0);
                yield return new WaitForSeconds(1f / 30f);
            }
        }

        public void RendererNoiseMap(float[,] noiseMap, float[,] weightMap, float[,] riversMap)
        {
            _mapSizeX = noiseMap.GetLength(0);
            _mapSizeY = noiseMap.GetLength(1);
            _texture2D = new Texture2D(_mapSizeX, _mapSizeY);
            _temperatureMap = new float[_mapSizeX, _mapSizeY];
            _weightMap = new float[_mapSizeX, _mapSizeY];
            _noiseMap = new float[_mapSizeX, _mapSizeY];
            if (rivers)
            {
                _riversMap = new float[_mapSizeX, _mapSizeY];
            }
            SetColor setColor;
            SetNoise setNoise;

            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    _noiseMap[x, y] = noiseMap[x, y] + groundLevel;
                    _temperatureMap[x, y] = TemperatureNoiseCreate(x, y);
                    _weightMap[x, y] = weightMap[x, y] + weight / 2 + waterLevel / 2;
                    if (rivers)
                    {
                        _riversMap[x, y] = riversMap[x, y];
                    }
                }
            });

            switch (typeOfMap)
            {
                case TypeOfMap.Island:
                    setColor = IslandRender;
                    setNoise = IslandCreate;
                    SetPixelsToTexture(setColor, setNoise);
                    break;
                case TypeOfMap.Temperature:
                    setColor = TemperatureRender;
                    SetPixelsToTexture(setColor);
                    break;
                case TypeOfMap.Coast:
                    setColor = CoastRender;
                    setNoise = CoastCreate;
                    SetPixelsToTexture(setColor, setNoise);
                    break;
                case TypeOfMap.Simple:
                    setColor = SimpleRender;
                    setNoise = SimpleCreate;
                    SetPixelsToTexture(setColor, setNoise);
                    //create polar hats
                    for (int x = 0; x < _mapSizeX; x++)
                    {
                        for (int y = 0; y < _mapSizeY; y++)
                        {
                            if (_temperatureMap[x, y] > 0.5)
                            {
                                _texture2D.SetPixel(x, y, _myColorArray[6].Color);
                            }
                        }
                    }
                    break;
                case TypeOfMap.Height:
                    setColor = HeightRender;
                    SetPixelsToTexture(setColor);
                    break;
                case TypeOfMap.Weight:
                    setColor = WeightRender;
                    SetPixelsToTexture(setColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetShader();
            StartCoroutine(RotateSphere());
        }

        private void SetShader()
        {
            renderer.sharedMaterial.EnableKeyword("_NORMALMAP");
            renderer.sharedMaterial.EnableKeyword("_PARALLAXMAP");
                
            _texture2D.Apply();
            renderer.sharedMaterial.mainTexture = _texture2D;
            
            //heights map for shader
            var colorsMap = new Color[_mapSizeX * _mapSizeY];
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    colorsMap[y * _mapSizeY + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
                }
            });
            var heightTexture = new Texture2D(_mapSizeX, _mapSizeY);
            heightTexture.SetPixels(colorsMap);
            heightTexture.Apply();
            renderer.sharedMaterial.SetFloat("_Parallax", HeightScale);
            renderer.sharedMaterial.SetTexture("_ParallaxMap", heightTexture);

            //normals map for shader
            Texture2D normalTexture = NormalMapGenerator.CreateNormalMap(_noiseMap, _mapSizeX, _mapSizeY);
            normalTexture.Apply();
            renderer.sharedMaterial.SetFloat("_BumpScale", NormalScale);
            renderer.sharedMaterial.SetTexture("_BumpMap", normalTexture);
        }

        private List<River> CreateRivers()
        {
            var riversList = new List<River>();

            for (int x = 0; x < _mapSizeX; x++)
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    //possible points to start rivers
                    if (!(_noiseMap[x, y] > _myColorArray[4].Level) || 
                        !(_noiseMap[x, y] < _myColorArray[5].Level) || 
                        !(_weightMap[x, y] > 0.65f) || 
                        !(_riversMap[x, y] > 0.9f)) continue;
                    if (riversList.Count < _mapSizeX && riversList.Count < 400)
                    {
                        var riverToAdd = new River();
                        if (riverToAdd.RiverGen(new Vector2Int(x, y), _noiseMap, _myColorArray[1].Level))
                        {
                            riversList.Add(riverToAdd);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            List<RiversGroup> riversGroups = RiversGroup.BuildRiversGroups(riversList, _mapSizeX, _mapSizeY);
            var riversListOut = new List<River>();

            foreach (var riversGroup in riversGroups)
            {
                //only the longest rivers in group were spawn
                var maxCount = int.MinValue;
                River longestRiver = riversGroup.GroupOfRivers[0];

                foreach (var river in riversGroup.GroupOfRivers.Where(river => river.Positions.Count > maxCount))
                {
                    maxCount = river.Positions.Count;
                    longestRiver = river;
                }
                
                riversListOut.Add(longestRiver);

                for (int i = 0; i < longestRiver.Positions.Count - 1; i++)
                {
                    UpWeightByRivers(longestRiver.Positions[i], longestRiver.Positions[i + 1]);
                }

                River.DigRiver(longestRiver);
            }
            
            return riversListOut;
        }

        private void UpWeightByRivers(Vector2Int position, Vector2Int nexPosition)
        {
            Vector2Int direction = position - nexPosition;

            if (direction == Vector2Int.down || direction == Vector2Int.up)
            {
                for (int x = -_mapSizeX / 10; x < _mapSizeX / 10; x++)
                {
                    if (position.x + x < 0 || position.x + x >= _mapSizeX)
                    {
                        continue;
                    }
                    if (x == 0)
                    {
                        _weightMap[position.x + x, position.y] += 0.5f;
                    }
                    _weightMap[position.x + x, position.y] += Mathf.Abs(0.4f / x);
                }
            }

            if (direction != Vector2Int.left && direction != Vector2Int.right) return;
            for (int y = -_mapSizeY / 10; y < _mapSizeY / 10; y++)
            {
                if (position.y + y < 0 || position.y + y >= _mapSizeY)
                {
                    continue;
                }
                if (y == 0)
                {
                    _weightMap[position.x, position.y + y] += 0.5f;
                }
                _weightMap[position.x, position.y + y] += Mathf.Abs(0.4f / y);
            }
        }

        private void SetPixelsToTexture(SetColor setColor)
        {
            var colorsMap = new Color[_mapSizeX * _mapSizeY];
            
            if (rivers)
            {
                CreateRivers();
            }
            
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    colorsMap[y * _mapSizeY + x] = setColor(x, y);
                }
            });

            _texture2D.SetPixels(colorsMap);
        }
        
        private void SetPixelsToTexture(SetColor setColor, SetNoise setNoise)
        {
            var colorsMap = new Color[_mapSizeX * _mapSizeY];
            
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    setNoise(x, y);
                }
            });

            List<River> riversList = new List<River>();
            if (rivers)
            {
                riversList = CreateRivers();
            }
            
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    colorsMap[y * _mapSizeY + x] = setColor(x, y);
                }
            });

            _texture2D.SetPixels(colorsMap);

            if (!rivers) return;
            foreach (var position in riversList.SelectMany(river => river.Positions))
            {
                _texture2D.SetPixel(position.x, position.y, _myColorArray[1].Color);
            }
        }
        
        private float TemperatureNoiseCreate(int x, int y)
        {
            float distanceFromEquator =
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, y)) /
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, 0));

            return Mathf.Sqrt(Mathf.Pow(_myColorArray[2].Level - _noiseMap[x, y], 2)) * temperature + distanceFromEquator * temperature;
        }

        //empty method for delegate
        private void SimpleCreate(int x, int y) { }

        private Color32 SimpleRender(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y] + _temperatureMap[x, y] / 2);
        }

        private void IslandCreate(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + UpAll) * (1 - DownCoast * Mathf.Pow(distanceToCenter, Speed));
        }

        private Color32 IslandRender(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private void CoastCreate(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + UpAll) * (1 - DownCoast * Mathf.Pow(distanceToCenter, Speed));
        }

        private Color32 CoastRender(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private Color32 WeightRender(int x, int y)
        {
            return GetPixelColor(_weightMap[x, y] + _temperatureMap[x, y] / 2, _myColorWeightArray);
        }

        private Color32 TemperatureRender(int x, int y)
        {
            return GetPixelColor(_temperatureMap[x, y], _myColorTemperatureArray);
        }

        private Color32 HeightRender(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _myColorHeightArray);
        }

        private Color32 GetPixelColor(float noise, IReadOnlyList<MyColor> myColors)
        {
            foreach (var myColor in myColors)
            {
                if (noise < myColor.Level)
                {
                    return myColor.Color;
                }
            }

            return myColors[6].Color;
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
                if (weightNoise < 0.7)
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