namespace PerlinNoiseGenerator.MapGen
{
    /// <summary>
    /// Includes methods for generating various maps that can be used to convert to textures.
    /// </summary>
    public class SphereMapsGenerator : IMapsGenerator
    {
        private readonly int _mapSizeX;
        private readonly int _mapSizeY;
        private readonly float _scale;
        private readonly int _seed;
        private readonly bool _rivers;
        public float[,] NoiseMap { get; set; }
        public float[,] WeightMap { get; set; }
        public float[,] RiversMap { get; set; }

        /// <summary>
        /// Configuring the Noise Algorithm.
        /// </summary>
        /// <param name="mapSizeX">horizontal size in pixels</param>
        /// <param name="mapSizeY">vertical size in pixels</param>
        /// /// <param name="scale">number that determines at what distance to view the noisemap</param>
        /// <param name="seed">random value for creating different cards</param>
        /// <param name="rivers">determines whether rivers will be generated or not</param>
        public SphereMapsGenerator(int mapSizeX, int mapSizeY, float scale, int seed, bool rivers)
        {
            _mapSizeX = mapSizeX;
            _mapSizeY = mapSizeY;
            _scale = scale;
            _seed = seed;
            _rivers = rivers;
        }

        /// <summary>
        /// Creates a 2D array based on simplex noise.
        /// </summary>
        public void CreateNoiseMap()
        {
            float[,] noiseMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, 
                NoiseGenConfig.OctavesForSimplex, _seed);
            NoiseMap = TransformSphereMap.TransformNoiseMap(noiseMap, _mapSizeX, _mapSizeY);
        }
        
        /// <summary>
        /// Creates a 2D array based on simplex noise.
        /// </summary>
        public void CreateWeightMap()
        {
            float[,] weightMap = Simplex.GenerateNoiseMap(_mapSizeX, _mapSizeY, _scale, 
                NoiseGenConfig.OctavesForSimplex, _seed + 1);
            WeightMap = TransformSphereMap.TransformNoiseMap(weightMap, _mapSizeX, _mapSizeY);
        }
        
        /// <summary>
        /// Creates a 2D array based on perlin noise.
        /// </summary>
        public void CreateRiversMap()
        {
            if (!_rivers) return;
            // we multiply the size of the map by 4 since we need to get a map of the cube sweep (as in simplex noise) but it does this by default
            float[,] riversMap = Noise.GenerateNoiseMap(_mapSizeX * 4, _mapSizeY * 4, NoiseGenConfig.ScaleForNoise, _seed + 2, 
                NoiseGenConfig.OctavesForNoise, NoiseGenConfig.PersistanceForRivers, NoiseGenConfig.Lacunarity);
            RiversMap = TransformSphereMap.TransformNoiseMap(riversMap, _mapSizeX, _mapSizeY);
        }
    }
}