using System.Threading.Tasks;
using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    public class HeightMap : ShaderDecorator
    {
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

        public HeightMap(ShaderCreator shaderCreator, Texture2D texture, Material material, float[,] noiseMap) : base(shaderCreator)
        {
            Texture = texture;
            Material = material;
            NoiseMap = noiseMap;
        }

        public override void SetTexture()
        {
            ShaderCreator.SetTexture();
            SetHeightMap();
        }
    }
}