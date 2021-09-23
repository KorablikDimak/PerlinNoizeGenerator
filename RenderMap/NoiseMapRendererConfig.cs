namespace PerlinNoiseGenerator.RenderMap
{
    /// <summary>
    /// Global constants for the generator.
    /// The values are selected optimal so it is not recommended to change them in the source code.
    /// </summary>
    public struct NoiseMapRendererConfig
    {
        public const float NormalScale = 0.62f;
        public const float HeightScale = 0.03f;
        public const float UpAll = 0.05f;
        public const float DownCoast = 1.6f;
        public const float Speed = 2.5f;
    }
}