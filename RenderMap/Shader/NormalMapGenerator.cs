using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    /// <summary>
    /// Include method for normalise height map.
    /// </summary>
    public static class NormalMapGenerator
    {
        /// <summary>
        /// Creates a normal map based on a noise map and a height map.
        /// </summary>
        /// <param name="normalTexture">the texture to be normalized</param>
        /// <param name="noiseMap">2D array of float in the range from 0 to 1</param>
        /// <param name="mapSizeX">horizontal size in pixels</param>
        /// <param name="mapSizeY">vertical size in pixels</param>
        /// <returns>normalised texture</returns>
        public static Texture2D CreateNormalMap(Texture2D normalTexture, float[,] noiseMap, int mapSizeX, int mapSizeY)
        {
            var normalVector = new Vector3 {z = 1};

            var noiseMapWithFrame = new float[mapSizeX + 2][];
            for (int index = 0; index < mapSizeX + 2; index++)
            {
                noiseMapWithFrame[index] = new float[mapSizeY + 2];
            }

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    noiseMapWithFrame[x + 1][y + 1] = noiseMap[x, y];
                }
            }
            for (int x = 0; x < mapSizeX; x++)
            {
                noiseMapWithFrame[x + 1][0] = noiseMap[x, 0];
                noiseMapWithFrame[x + 1][mapSizeY + 1] = noiseMap[x, mapSizeY - 1];
            }
            for (int y = 0; y < mapSizeY; y++)
            {
                noiseMapWithFrame[0][y + 1] = noiseMap[0, y];
                noiseMapWithFrame[mapSizeX + 1][y + 1] = noiseMap[mapSizeX - 1, y];
            }

            noiseMapWithFrame[0][0] = noiseMap[mapSizeX - 1, 0];
            noiseMapWithFrame[0][mapSizeY + 1] = noiseMap[mapSizeX - 1, mapSizeY - 1];
            noiseMapWithFrame[mapSizeX + 1][0] = noiseMap[0, 0];
            noiseMapWithFrame[mapSizeX + 1][mapSizeY + 1] = noiseMap[0, mapSizeY - 1];

            for (int x = 1; x < mapSizeX + 1; x++)
            {
                for (int y = 1; y < mapSizeY + 1; y++)
                {
                    var topLeft = noiseMapWithFrame[x - 1][y + 1];
                    var top = noiseMapWithFrame[x][y + 1];
                    var topRight = noiseMapWithFrame[x + 1][y + 1];
                    var right = noiseMapWithFrame[x + 1][y];
                    var downRight = noiseMapWithFrame[x + 1][y - 1];
                    var down = noiseMapWithFrame[x][y - 1];
                    var downLeft = noiseMapWithFrame[x - 1][y - 1];
                    var left = noiseMapWithFrame[x - 1][y];

                    normalVector.x = -(downRight - downLeft + 2 * (right - left) + topRight - topLeft);
                    normalVector.y = -(topLeft - downLeft + 2 * (top - down) + topRight - downRight);
                    normalVector.Normalize();
                    normalTexture.SetPixel(x - 1, y - 1, new Color(normalVector.x, normalVector.y, normalVector.z));
                }
            }
            
            return normalTexture;
        }
    }
}