namespace PerlyNoizeGenerator
{
    public struct NoiseRenderConfig
    {
        public float UpAll { get; set; }
        public float DownCoast { get; set; }
        public float Speed { get; set; }

        public NoiseRenderConfig(float upAll, float downCoast, float speed)
        {
            UpAll = upAll;
            DownCoast = downCoast;
            Speed = speed;
        }
    }
}