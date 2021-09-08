using UnityEngine;

namespace PerlinNoiseGenerator.MapGen
{
    public static class TransformSphereMap
    {
        public static float[,] TransformNoiseMap(float[,] noiseMap, int mapSizeX, int mapSizeY)
        {
            var sourceNoiseMap = new float[mapSizeX * 4, mapSizeY * 3];
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = mapSizeY; y < height; y++)
                {
                    sourceNoiseMap[x, y - mapSizeY] = noiseMap[x, y - mapSizeY];
                }
            }
            
            var equiNoiseMap = new float[mapSizeX, mapSizeY];
            float u, v;
            float phi, theta;
            
            int cubeFaceWidth = mapSizeX; //4 horizontal faces
            int cubeFaceHeight = mapSizeY;

            width = sourceNoiseMap.GetLength(0);
            height = sourceNoiseMap.GetLength(1);
            
            for (int j = 0; j < mapSizeY; j++)
            {

                //Rows start from the bottom
                v = 1 - ((float) j / mapSizeY);
                theta = v * Mathf.PI;

                for (int i = 0; i < mapSizeX; i++)
                {
                    //Columns start from the left
                    u = ((float) i / mapSizeX);
                    phi = u * 2 * Mathf.PI;

                    float x, y, z; //Unit vector
                    x = Mathf.Sin(phi) * Mathf.Sin(theta) * -1;
                    y = Mathf.Cos(theta);
                    z = Mathf.Cos(phi) * Mathf.Sin(theta) * -1;

                    float xa, ya, za;
                    float a;

                    a = Mathf.Max(new float[3] {Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z)});

                    //Vector Parallel to the unit vector that lies on one of the cube faces
                    xa = x / a;
                    ya = y / a;
                    za = z / a;

                    int xPixel, yPixel;
                    int xOffset, yOffset;

                    if (xa == 1)
                    {
                        //Right
                        xPixel = (int) ((((za + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 2 * cubeFaceWidth; //Offset
                        yPixel = (int) ((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight; //Offset
                    }
                    else if (xa == -1)
                    {
                        //Left
                        xPixel = (int) ((((za + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = 0;
                        yPixel = (int) ((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (ya == 1)
                    {
                        //Up
                        xPixel = (int) ((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int) ((((za + 1f) / 2f) - 1f) * cubeFaceHeight);
                        yOffset = 2 * cubeFaceHeight;
                    }
                    else if (ya == -1)
                    {
                        //Down
                        xPixel = (int) ((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int) ((((za + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = 0;
                    }
                    else if (za == 1)
                    {
                        //Front
                        xPixel = (int) ((((xa + 1f) / 2f)) * cubeFaceWidth);
                        xOffset = cubeFaceWidth;
                        yPixel = (int) ((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else if (za == -1)
                    {
                        //Back
                        xPixel = (int) ((((xa + 1f) / 2f) - 1f) * cubeFaceWidth);
                        xOffset = 3 * cubeFaceWidth;
                        yPixel = (int) ((((ya + 1f) / 2f)) * cubeFaceHeight);
                        yOffset = cubeFaceHeight;
                    }
                    else
                    {
                        Debug.LogWarning("Unknown face, something went wrong");
                        xPixel = 0;
                        yPixel = 0;
                        xOffset = 0;
                        yOffset = 0;
                    }

                    xPixel = Mathf.Abs(xPixel);
                    yPixel = Mathf.Abs(yPixel);

                    xPixel += xOffset;
                    yPixel += yOffset;

                    if (xPixel >= width || yPixel >= height)
                    {
                        continue;
                    }
                    equiNoiseMap[i, j] = sourceNoiseMap[xPixel, yPixel];
                }
            }

            return equiNoiseMap;
        }
    }
}