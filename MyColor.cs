using UnityEngine;

namespace PerlyNoizeGenerator
{
    public readonly struct MyColor
    {
        public float Level { get; }
        public Color32 Color { get; }
        
        public MyColor(float level, Color32 color)
        {
            Level = level;
            Color = color;
        }
    }
}