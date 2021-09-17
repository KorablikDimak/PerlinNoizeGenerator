namespace PerlinNoiseGenerator.RenderMap.Shader
{
    public abstract class ShaderDecorator : ShaderCreator
    {
        protected readonly ShaderCreator ShaderCreator;

        protected ShaderDecorator(ShaderCreator shaderCreator)
        {
            ShaderCreator = shaderCreator;
        }
    }
}