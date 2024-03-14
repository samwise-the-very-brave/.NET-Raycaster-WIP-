using System.Collections.Generic;
using System.Drawing;

namespace DDA
{
    static class FontManager
    {
        public static IReadOnlyDictionary<char, Image> Characters { get => chars; }
        public const int FontWidth = 8;
        public const int FontHeight = 16;
        public static IReadOnlyDictionary<string, Text> AllText { get => text; }

        static Dictionary<string, Text> text = new Dictionary<string, Text>();
        static Dictionary<char, Image> chars = new Dictionary<char, Image>();

        public static void LoadFont()
        {
            char key = (char)32;

            Bitmap fontBmp = new Bitmap(Image.FromFile(@"..\..\..\font\mogfont.png"));

            int bitmapX = 0, bitmapY = 0;

            for (int i = 0; key < 126; i++, key++)
            {
                Bitmap cropped = CropBitmap(ref fontBmp, bitmapX, bitmapY, FontWidth, FontHeight);

                chars.Add(key, cropped);

                bitmapX += FontWidth;

                if (bitmapX >= fontBmp.Width - 1)
                {
                    bitmapX = 0;
                    bitmapY += FontHeight;
                }
            }

            //fontBmp.Save(@"..\..\..\font\koksa.png");
        }
        public static void Add(Text t)
        {
            text.Add(t.Tag, t);
        }
        static Bitmap CropBitmap(ref Bitmap source, int x, int y, int cropWidth, int cropHeight)
        {
            Bitmap cropped = new Bitmap(cropWidth, cropHeight);

            for (int iy = 0; iy < cropped.Height && iy + y < source.Height; iy++)
            {
                for (int ix = 0; ix < cropped.Width && ix + x < source.Width; ix++)
                {
                    if (source.GetPixel(ix + x, iy + y) == Color.FromArgb(63, 63, 63))
                    {
                        cropped.SetPixel(ix, iy, Color.Transparent);
                    }
                    /*if (*//*ix == 0 ||*//* ix == cropWidth - 1 *//*|| iy == 0 *//*|| iy == cropHeight - 1)
                    {
                        source.SetPixel(ix + x, iy + y, Color.Red);
                    }*/
                    else
                    {
                        cropped.SetPixel(ix, iy, source.GetPixel(ix + x, iy + y));
                    }
                }
            }

            return cropped;
        }
    }
}
