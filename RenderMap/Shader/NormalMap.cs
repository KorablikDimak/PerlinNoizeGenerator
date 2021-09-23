using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    /// <summary>
    /// Sets the normal texture to the shader.
    /// </summary>
    public class NormalMap : ShaderDecorator
    {
        /// <summary>
        /// Adding new functionality for the shader.
        /// </summary>
        /// <param name="shaderCreator">shader which will use on material</param>
        /// <param name="texture">texture which need to set on shader</param>
        /// <param name="material">main material of game object</param>
        /// <param name="noiseMap">2D array of float in the range from 0 to 1</param>
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