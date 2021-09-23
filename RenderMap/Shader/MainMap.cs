using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    /// <summary>
    /// Sets the albedo to the shader.
    /// </summary>
    public class MainMap : ShaderDecorator
    {
        /// <summary>
        /// Adding new functionality for the shader.
        /// </summary>
        /// <param name="shaderCreator">shader which will use on material</param>
        /// <param name="texture">texture which need to set on shader</param>
        /// <param name="material">main material of game object</param>
        public MainMap(ShaderCreator shaderCreator, Texture2D texture, Material material) : base(shaderCreator)
        {
            Texture = texture;
            Material = material;
        }
        
        private void SetMainMap()
        {
            Texture.Apply();
            Material.mainTexture = Texture;
        }
        
        public override void SetTexture()
        {
            ShaderCreator.SetTexture();
            SetMainMap();
        }
    }
}