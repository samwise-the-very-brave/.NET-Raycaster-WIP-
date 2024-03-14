using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DDA
{
    class Palette
    {
        Color[] colors;

        public Palette(string path)
        {
            Bitmap source = new Bitmap(Image.FromFile(path));

            byte[] colorData = Texture.BitmapToArray(source);
            colors = new Color[colorData.Length / 4];

            for (int i = 0, j = 0; i < colors.Length; i++, j += 4)
            {
                colors[i] = Color.FromArgb(colorData[j], colorData[j + 1], colorData[j + 2]);
            }
        }

        public Color GetClosestTo(Color c)
        {
            float min = colors.Min(x => MathHelper.Distance(x, c));
            return colors.First(x => MathHelper.Distance(x, c) == min);
        }
    }
}
