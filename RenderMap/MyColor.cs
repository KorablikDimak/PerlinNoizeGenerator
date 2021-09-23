using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap
{
    public readonly struct MyColor
    {
        public float Level { get; }
        public Color32 Color { get; }

        /// <summary>
        /// Characteristic of each individual pixel of noise generated on the map.
        /// </summary>
        /// <param name="level">the level below which this color will appear</param>
        /// <param name="color">the color of the pixel created on the map</param>
        public MyColor(float level, Color32 color)
        {
            Level = level;
            Color = color;
        }
    }
}