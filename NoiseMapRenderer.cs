using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace PerlinNoiseGenerator
{
    public class NoiseMapRenderer : MonoBehaviour
    {
        [SerializeField] private LoadIndicator loadIndicator;
        [SerializeField] private Button saveButton;
        [SerializeField] private Canvas saveCanvas;
        [SerializeField] private Material mainMaterial;
        [SerializeField] private Toggle isRiversOn;
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
        private Texture2D _normalTexture2D;
        private Texture2D _heightTexture2D;
        private delegate Color32 SetColor(int x, int y);
        private delegate void SetNoise(int x, int y);

        //player will change this:
        public float Temperature { get; set; }
        public float Weight { get; set; }
        public TypeOfMap CurrentTypeOfMap { get; set; }
        public float GroundLevel { get; set; }
        public float WaterLevel { get; set; }

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
            saveButton.onClick.AddListener(SaveButtonClicked);
            saveCanvas.enabled = false;
            
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

        private void SaveButtonClicked()
        {
            TextureSaver.SaveTexture(_texture2D, "main texture");
            TextureSaver.SaveTexture(_heightTexture2D, "height texture");
            TextureSaver.SaveTexture(_normalTexture2D, "normal texture");
            saveCanvas.enabled = true;
        }

        public IEnumerator RenderNoiseMap(float[,] noiseMap, float[,] weightMap, float[,] riversMap)
        {
            _mapSizeX = noiseMap.GetLength(0);
            _mapSizeY = noiseMap.GetLength(1);
            _texture2D = new Texture2D(_mapSizeX, _mapSizeY);
            _temperatureMap = new float[_mapSizeX, _mapSizeY];
            _weightMap = new float[_mapSizeX, _mapSizeY];
            _noiseMap = new float[_mapSizeX, _mapSizeY];
            if (isRiversOn.isOn)
            {
                _riversMap = new float[_mapSizeX, _mapSizeY];
            }

            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    _noiseMap[x, y] = noiseMap[x, y] + GroundLevel;
                    _temperatureMap[x, y] = CreateTemperatureNoise(x, y);
                    _weightMap[x, y] = weightMap[x, y] + Weight / 2 + WaterLevel / 2;
                    if (isRiversOn.isOn)
                    {
                        _riversMap[x, y] = riversMap[x, y];
                    }
                }
            });
            
            loadIndicator.SetText("генерация рек и ландщафта");
            loadIndicator.AddProgress(0.3f);
            yield return null;
            
            SwitchTypeOfMap();
            loadIndicator.SetText("установка шейдеров");
            loadIndicator.AddProgress(0.2f);
            yield return null;
            
            SetShaders();
            yield return null;
        }

        private void SwitchTypeOfMap()
        {
            SetColor setColor;
            SetNoise setNoise;
            
            switch (CurrentTypeOfMap)
            {
                case TypeOfMap.Island:
                    setColor = RenderIsland;
                    setNoise = CreateIsland;
                    SetPixelsToTexture(setColor, setNoise);
                    break;
                case TypeOfMap.Temperature:
                    setColor = RenderTemperature;
                    SetPixelsToTexture(setColor);
                    break;
                case TypeOfMap.Coast:
                    setColor = RenderCoast;
                    setNoise = CreateCoast;
                    SetPixelsToTexture(setColor, setNoise);
                    break;
                case TypeOfMap.Simple:
                    setColor = RenderSimple;
                    setNoise = CreateSimple;
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
                    setColor = RenderHeight;
                    SetPixelsToTexture(setColor);
                    break;
                case TypeOfMap.Weight:
                    setColor = RenderWeight;
                    SetPixelsToTexture(setColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetShaders()
        {
            mainMaterial.EnableKeyword("_NORMALMAP");
            mainMaterial.EnableKeyword("_PARALLAXMAP");
            
            SetMainTexture();
            SetHeightMap();
            SetNormalMap();
        }

        private void SetMainTexture()
        {
            _texture2D.Apply();
            mainMaterial.mainTexture = _texture2D;
        }

        private void SetHeightMap()
        {
            //heights map for shader
            var colorsMap = new Color[_mapSizeX * _mapSizeY];
            Parallel.For(0, _mapSizeX, x =>
            {
                for (int y = 0; y < _mapSizeY; y++)
                {
                    colorsMap[y * _mapSizeY + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
                }
            });
            _heightTexture2D = new Texture2D(_mapSizeX, _mapSizeY);
            _heightTexture2D.SetPixels(colorsMap);
            _heightTexture2D.Apply();
            mainMaterial.SetFloat("_Parallax", NoiseMapRendererConfig.HeightScale);
            mainMaterial.SetTexture("_ParallaxMap", _heightTexture2D);
        }

        private void SetNormalMap()
        {
            //normals map for shader
            _normalTexture2D = NormalMapGenerator.CreateNormalMap(_noiseMap, _mapSizeX, _mapSizeY);
            _normalTexture2D.Apply();
            mainMaterial.SetFloat("_BumpScale", NoiseMapRendererConfig.NormalScale);
            mainMaterial.SetTexture("_BumpMap", _normalTexture2D);
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
                        !(_weightMap[x, y] > 0.6f) || 
                        !(_riversMap[x, y] > 0.85f)) continue;
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
            
            if (isRiversOn.isOn)
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
            if (isRiversOn.isOn)
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

            if (!isRiversOn.isOn) return;
            foreach (var position in riversList.SelectMany(river => river.Positions))
            {
                _texture2D.SetPixel(position.x, position.y, _myColorArray[1].Color);
            }
        }
        
        private float CreateTemperatureNoise(int x, int y)
        {
            float distanceFromEquator =
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, y)) /
                Vector2.Distance(new Vector2Int(0, _mapSizeY / 2), new Vector2Int(0, 0));

            return Mathf.Sqrt(Mathf.Pow(_myColorArray[2].Level - _noiseMap[x, y], 2)) * Temperature + distanceFromEquator * Temperature;
        }

        //empty method for delegate
        private void CreateSimple(int x, int y) { }

        private Color32 RenderSimple(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y] + _temperatureMap[x, y] / 2);
        }

        private void CreateIsland(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX / 2, _mapSizeY / 2), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + NoiseMapRendererConfig.UpAll) * (1 - NoiseMapRendererConfig.DownCoast * Mathf.Pow(distanceToCenter, NoiseMapRendererConfig.Speed));
        }

        private Color32 RenderIsland(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private void CreateCoast(int x, int y)
        {
            float distanceToCenter = 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(x, y)) / 
                Vector2Int.Distance(new Vector2Int(_mapSizeX, _mapSizeY), new Vector2Int(0, 0));
                
            _noiseMap[x, y] = (_noiseMap[x, y] + NoiseMapRendererConfig.UpAll) * (1 - NoiseMapRendererConfig.DownCoast * Mathf.Pow(distanceToCenter, NoiseMapRendererConfig.Speed));
        }

        private Color32 RenderCoast(int x, int y)
        {
            return GetPixelColor(_noiseMap[x, y], _weightMap[x, y]);
        }

        private Color32 RenderWeight(int x, int y)
        {
            return GetPixelColor(_weightMap[x, y] + _temperatureMap[x, y] / 2, _myColorWeightArray);
        }

        private Color32 RenderTemperature(int x, int y)
        {
            return GetPixelColor(_temperatureMap[x, y], _myColorTemperatureArray);
        }

        private Color32 RenderHeight(int x, int y)
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
            if (noise < _myColorArray[0].Level + WaterLevel)
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

            if (noise < _myColorArray[1].Level + WaterLevel)
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