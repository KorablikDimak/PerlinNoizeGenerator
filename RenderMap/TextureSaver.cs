using System.IO;
using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap
{
    public static class TextureSaver
    {
        public static void SaveTexture(Texture2D texture2D, string name)
        {
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + $"/../{name}.png", bytes);
        }
    }
}