using UnityEngine;

namespace PerlyNoizeGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseMapRenderer noiseMapRenderer;
        
        //config:
        private const float Scale = 4.5f;
        private const float Persistance = 1;
        private const float Lacunarity = 0.72f;
        private const int Octaves = 20;
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
            if (autoUpdate && _mapSizeX * _mapSizeY <= 300 * 300 && !noiseMapRenderer.rivers && !noiseMapRenderer.render3D)
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
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, Scale, seed, Octaves, Persistance, Lacunarity);
            float[,] weightMap =
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, Scale, seed + 1, Octaves, Persistance, Lacunarity);
            float[,] riversMap =
                Noise.GenerateNoiseMap(_mapSizeX, _mapSizeY, Scale, seed + 2, Octaves, 0, Lacunarity);

            noiseMapRenderer.RendererNoiseMap(noiseMap, weightMap, riversMap);
        }
    }
}