using UnityEngine;

namespace PerlyNoizeGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        
        //config:
        [SerializeField] private float scale;
        [Range(0, 1)][SerializeField] private float persistance;
        [SerializeField] private float lacunarity;
        [SerializeField] private int octaves;
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
                    _mapSizeX = 100;
                    _mapSizeY = 100;
                    break;
                case MapSize.Medium:
                    _mapSizeX = 300;
                    _mapSizeY = 300;
                    scale = 5;
                    octaves = 20;
                    lacunarity = 0.72f;
                    persistance = 1;
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
            
            float[,] noiseMap = 
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, scale, seed, octaves, persistance, lacunarity);
            float[,] weightMap =
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, scale, seed + 1, octaves, persistance, lacunarity);
            float[,] riversMap =
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, scale, seed + 2, octaves, 0, lacunarity);

            noiseMapRenderer.RendererNoiseMap(noiseMap, weightMap, riversMap);
        }
    }
}