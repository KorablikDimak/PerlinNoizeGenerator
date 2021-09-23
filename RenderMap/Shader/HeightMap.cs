using System.Threading.Tasks;
using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    /// <summary>
    /// Sets the heightmap to the shader.
    /// </summary>
    public class HeightMap : ShaderDecorator
    {
        /// <summary>
        /// Adding new functionality for the shader.
        /// </summary>
        /// <param name="shaderCreator">shader which will use on material</param>
        /// <param name="texture">texture which need to set on shader</param>
        /// <param name="material">main material of game object</param>
        /// <param name="noiseMap">2D array of float in the range from 0 to 1</param>
        public HeightMap(ShaderCreator shaderCreator, Texture2D texture, Material material, float[,] noiseMap) : base(shaderCreator)
        {
            Texture = texture;
            Material = material;
            NoiseMap = noiseMap;
        }
        
        private void SetHeightMap()
        {
            int mapSizeX = NoiseMap.GetLength(0);
            int mapSizeY = NoiseMap.GetLength(1);
            
            Material.EnableKeyword("_PARALLAXMAP");
            var colorsMap = new Color[mapSizeX * mapSizeY];
            Parallel.For(0, mapSizeX, x =>
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    colorsMap[y * mapSizeY + x] = Color.Lerp(Color.black, Color.white, NoiseMap[x, y]);
                }
            });
            Texture.SetPixels(colorsMap);
            Texture.Apply();
            Material.SetFloat("_Parallax", NoiseMapRendererConfig.HeightScale);
            Material.SetTexture("_ParallaxMap", Texture);
        }

        
        public override void SetTexture()
        {
            ShaderCreator.SetTexture();
            SetHeightMap();
        }
    }
}