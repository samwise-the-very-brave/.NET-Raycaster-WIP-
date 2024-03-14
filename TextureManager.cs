using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DDA
{
    public static class TextureManager
    {
        public static IReadOnlyDictionary<int, Texture> AllTextures { get => textures; }

        static Dictionary<int, Texture> textures = new();

        public static void IncludeTextures(string directory, int key)
        {
            string[] textureNames = Directory.GetFiles(directory);

            foreach (string fileName in textureNames)
            {
                IncludeTexture(fileName, key);
            }
        }
        public static void IncludeTexture(string filePath, int key)
        {
            textures.Add(key, new Texture(filePath));
        }
        static string GetFileName(string filePath)
        {
            return filePath.Split(@"\").Last().Split(".")[0];
        }
    }
}
