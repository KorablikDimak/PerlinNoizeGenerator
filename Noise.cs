using System.Threading.Tasks;
using UnityEngine;

namespace PerlyNoizeGenerator
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap
            (int mapSizeX, int mapSizeY, float scale, int seed, int octaves, float persistance, float lacunarity)
        {
            float[,] noiseMap = new float[mapSizeX, mapSizeY];
            var randSeed = new System.Random(seed);
            
            Vector2[] octavesOffset = new Vector2[octaves];

            Parallel.For(0, octaves, i =>
            {
                int offsetX = randSeed.Next(-100000, 100000);
                int offsetY = randSeed.Next(-100000, 100000);
                octavesOffset[i] = new Vector2(offsetX, offsetY);
            });
        
            if (scale <= 0)
            {
                scale = 0.00001f;
            }

            float halfMapSizeX = mapSizeX / 2f;
            float halfMapSizeY = mapSizeY / 2f;
            
            float maxNoiseSize = float.MinValue;
            float minNoiseSize = float.MaxValue;

            Parallel.For(0, mapSizeX, x =>
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

                        float perlyValue = Mathf.PerlinNoise(scaledX, scaledY) * 2 - 1;

                        noiseSize += perlyValue * amplitude;
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
            });

            Parallel.For(0, mapSizeX, x =>
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseSize, maxNoiseSize, noiseMap[x, y]);
                }
            });

            return noiseMap;
        }
    }
}