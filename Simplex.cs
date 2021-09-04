using UnityEngine;

namespace PerlinNoiseGenerator
{
    public static class Simplex
    {
        public static float[,] GenerateNoiseMap(int mapSizeX, int mapSizeY, float scale, int octaves, int seed)
        {
            var cubeMap = new float[mapSizeX * 4, mapSizeY * 4];

            var randSeed = new System.Random(seed);
            var octavesOffset = new Vector3Int[octaves];
            for (int i = 0; i < octaves; i++)
            {
                int offsetX = randSeed.Next(-mapSizeX, mapSizeX);
                int offsetY = randSeed.Next(-mapSizeY, mapSizeY);
                int offsetZ = randSeed.Next(-mapSizeX, mapSizeX);
                octavesOffset[i] = new Vector3Int(offsetX, offsetY, offsetZ);
            }

            //Z STATIC
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int x = 0; x < mapSizeX * 2; x++)
                {
                    for (int oct = 0; oct < octaves; oct++)
                    {
                        //Generates FRONT
                        if (x < mapSizeX)
                        {
                            cubeMap[mapSizeX + x, mapSizeY + y] +=
                                Mathf.Pow(2, -oct) *
                                (SimplexNoise.Noise.CalcPixel3D(x + octavesOffset[oct].x, y + octavesOffset[oct].y, 0 + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) / 255 - 0.25f);
                        }
                        //Generates BACK
                        else
                        {
                            cubeMap[mapSizeX * 3 + (x - mapSizeX), mapSizeY + y] +=
                                Mathf.Pow(2, -oct) * (SimplexNoise.Noise.CalcPixel3D(mapSizeX - (x - mapSizeX) + octavesOffset[oct].x, y + octavesOffset[oct].y,
                                    mapSizeY + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) / 255 - 0.25f);
                        }
                    }
                }
            }

            //X STATIC
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int x = 0; x < mapSizeX * 2; x++)
                {
                    for (int oct = 0; oct < octaves; oct++)
                    {
                        //Generates LEFT
                        if (x < mapSizeX)
                        {
                            cubeMap[x, mapSizeY + y] +=
                                Mathf.Pow(2, -oct) *
                                (SimplexNoise.Noise.CalcPixel3D(0 + octavesOffset[oct].x, y + octavesOffset[oct].y, mapSizeX - x + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) / 255 -
                                 0.25f);
                        }
                        //Generates RIGHT
                        else
                        {
                            cubeMap[mapSizeX * 2 + (x - mapSizeX), mapSizeY + y] +=
                                Mathf.Pow(2, -oct) *
                                (SimplexNoise.Noise.CalcPixel3D(mapSizeX + octavesOffset[oct].x, y + octavesOffset[oct].y, x - mapSizeX + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) /
                                    255 - 0.25f);
                        }
                    }
                }
            }

            //Y STATIC
            for (int y = 0; y < mapSizeY * 2; y++)
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    for (int oct = 0; oct < octaves; oct++)
                    {
                        //Generates TOP
                        if (y < mapSizeY)
                        {
                            cubeMap[mapSizeX + x, y] +=
                                Mathf.Pow(2, -oct) *
                                (SimplexNoise.Noise.CalcPixel3D(x + octavesOffset[oct].x, 0 + octavesOffset[oct].y, mapSizeY - y + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) / 255 -
                                 0.25f);
                        }
                        //Generates BOTTOM
                        else
                        {
                            cubeMap[mapSizeX + x, mapSizeY * 2 + (y - mapSizeY)] +=
                                Mathf.Pow(2, -oct) *
                                (SimplexNoise.Noise.CalcPixel3D(x + octavesOffset[oct].x, mapSizeY + octavesOffset[oct].y, y - mapSizeY + octavesOffset[oct].z, scale * Mathf.Pow(2, oct)) /
                                    255 - 0.25f);
                        }
                    }
                }
            }

            return cubeMap;
        }
    }
}