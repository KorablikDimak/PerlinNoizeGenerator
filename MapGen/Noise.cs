using UnityEngine;

namespace PerlinNoiseGenerator.MapGen
{
    public static class Noise
    {
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