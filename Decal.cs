using System.Drawing;

namespace DDA
{
    class Decal
    {
        public int MapX { get; init; }
        public int MapY { get; init; }
        public float WallXStart { get; init; }
        public float WallXEnd { get; init; }
        public Color[][] Texture { get; init; }

        public Decal(string name, int mapX, int mapY, float wallXStart, float wallXEnd)
        {
            MapX = mapX;
            MapY = mapY;
            WallXStart = wallXStart;
            WallXEnd = wallXEnd;

            Texture = GetVerticalStripes(name);

            DecalManager.Add(this);
        }

        static Color[][] GetVerticalStripes(string decalName)
        {
            Bitmap texture = new Bitmap(Image.FromFile(@$"..\..\..\decals\{decalName}.png"));

            Color[][] output = new Color[texture.Width][];

            for (int x = 0; x < texture.Width; x++)
            {
                Color[] pixelColumn = new Color[texture.Height];

                for (int y = 0; y < texture.Height; y++)
                {
                    Color c = texture.GetPixel(x, y);
                    pixelColumn[y] = c;
                }

                output[x] = pixelColumn;
            }

            return output;
        }
    }
}
