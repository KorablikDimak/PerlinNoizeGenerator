using System.IO;
using UnityEngine;

namespace PerlinNoiseGenerator.RenderMap
{
    /// <summary>
    /// Contains a method for saving generated textures.
    /// </summary>
    public static class TextureSaver
    {
        /// <summary>
        /// Saves the texture in png to the root directory of the application.
        /// </summary>
        /// <param name="texture2D">the texture to be preserved</param>
        /// <param name="name">name of the saved texture</param>
        public static void SaveTexture(Texture2D texture2D, string name)
        {
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + $"/../{name}.png", bytes);
        }
    }
}