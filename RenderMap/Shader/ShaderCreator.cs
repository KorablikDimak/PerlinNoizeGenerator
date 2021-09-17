using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    public abstract class ShaderCreator
    {
        protected Texture2D Texture { get; set; }
        protected Material Material { get; set; }
        protected float[,] NoiseMap { get; set; }
        public abstract void SetTexture();
    }
}