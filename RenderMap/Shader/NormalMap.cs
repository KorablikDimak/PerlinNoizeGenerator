using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    public class NormalMap : ShaderDecorator
    {
        public NormalMap(ShaderCreator shaderCreator, Texture2D texture, Material material, float[,] noiseMap) : base(shaderCreator)
        {
            Texture = texture;
            Material = material;
            NoiseMap = noiseMap;
        }
        
        private void SetNormalMap()
        {
            int mapSizeX = NoiseMap.GetLength(0);
            int mapSizeY = NoiseMap.GetLength(1);
            
            Material.EnableKeyword("_NORMALMAP");
            Texture = NormalMapGenerator.CreateNormalMap(Texture, NoiseMap, mapSizeX, mapSizeY);
            Texture.Apply();
            Material.SetFloat("_BumpScale", NoiseMapRendererConfig.NormalScale);
            Material.SetTexture("_BumpMap", Texture);
        }

        public override void SetTexture()
        {
            ShaderCreator.SetTexture();
            SetNormalMap();
        }
    }
}