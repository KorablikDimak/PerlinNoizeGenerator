using UnityEngine;

namespace PerlinNoiseGenerator.MapGen
{
    /// <summary>
    /// Includes a perlin noise generation algorithm.
    /// </summary>
    public static class Noise
    {
        /// <summary>
        /// Creates a 2D array which, when interpolated from black to white, can be perceived as perlin noise.
        /// </summary>
        /// <param name="mapSizeX">horizontal size in pixels</param>
        /// <param name="mapSizeY">vertical size in pixels</param>
        /// <param name="scale">number that determines at what distance to view the noisemap</param>
        /// <param name="seed">random value for creating different cards</param>
        /// <param name="octaves">the number of levels of detail you want you perlin noise to have</param>
        /// <param name="persistance">number that determines how much each octave contributes to the overall shape</param>
        /// <param name="lacunarity">number that determines how much detail is added or removed at each octave</param>
        /// <returns>2D array of float in the range from 0 to 1</returns>
        public static float[,] GenerateNoiseMap
            (int mapSizeX, int mapSizeY, float scale, int seed, int octaves, float persistance, float lacunarity)
        {
            var noiseMap = new float[mapSizeX, mapSizeY];
            var randSeed = new System.Random(seed);
            
            var octavesOffset = new Vector2[octaves];

            for(int i = 0; i < octaves; i++)
            {
                int offsetX = randSeed.Next(-100000, 100000);
                int offsetY = randSeed.Next(-100000, 100000);
                octavesOffset[i] = new Vector2(offsetX, offsetY);
            }
            
            if (scale <= 0)
            {
                scale = 0.00001f;
            }

            float halfMapSizeX = mapSizeX / 2f;
            float halfMapSizeY = mapSizeY / 2f;
            
            var maxNoiseSize = float.MinValue;
            var minNoiseSize = float.MaxValue;

            for(int x = 0; x < mapSizeX; x++)
            {
                for (var y = 0; y < mapSizeY; y++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseSize = 0;

                    for (var oct = 0; oct < octaves; oct++)
                    {
                        float scaledX = (x - halfMapSizeX) / scale * frequency + octavesOffset[oct].x;
                        float scaledY = (y - halfMapSizeY) / scale * frequency + octavesOffset[oct].y;

                        float perlinValue = Mathf.PerlinNoise(scaledX, scaledY) * 2 - 1;

                        noiseSize += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseSize > maxNoiseSize)
                    {
                        maxNoiseSize = noiseSize;
                    }
                    else if (noiseSize < minNoiseSize)
                    {
                        minNoiseSize = noiseSize;
                    }

                    noiseMap[x, y] = noiseSize;
                }
            }

            for(int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseSize, maxNoiseSize, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}