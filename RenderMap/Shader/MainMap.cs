using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap.Shader
{
    public class MainMap : ShaderDecorator
    {
        private void SetMainMap()
        {
            Texture.Apply();
            Material.mainTexture = Texture;
        }

        public MainMap(ShaderCreator shaderCreator, Texture2D texture, Material material) : base(shaderCreator)
        {
            Texture = texture;
            Material = material;
        }

        public override void SetTexture()
        {
            ShaderCreator.SetTexture();
            SetMainMap();
        }
    }
}