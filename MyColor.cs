using UnityEngine;

namespace PerlinNoiseGenerator
{
    public readonly struct MyColor
    {
        public readonly float Level;
        public readonly Color32 Color;

        public MyColor(float level, Color32 color)
        {
            Level = level;
            Color = color;
        }
    }
}