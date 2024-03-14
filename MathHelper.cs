using System;
using System.Drawing;

namespace DDA
{
    static class MathHelper
    {
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return MathF.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static float Distance(Color a, Color b)
        {
            return MathF.Sqrt((b.R - a.R) * (b.R - a.R) + (b.G - a.G) * (b.G - a.G) + (b.B - a.B) * (b.B - a.B));
        }
    }
}
