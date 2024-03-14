using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace DDA
{
    static class PaletteManager
    {
        public static Palette MainPalette { get; private set; }
        static Palette shadePalette;

        public static void Init()
        {
            MainPalette = new Palette(@"..\..\..\palettes\main.png");
            shadePalette = new Palette(@"..\..\..\palettes\shade.png");
        }
    }
}
