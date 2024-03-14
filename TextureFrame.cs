using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DDA
{
    public struct TextureFrame
    {
        Color[][] frame;

        TextureFrame(Color[][] frame)
        {
            this.frame = frame;
        }

        public static TextureFrame GetFrom(Bitmap frame)
        {
            Color[][] output = new Color[frame.Width][];
            
            for (int x = 0; x < frame.Width; x++)
            {
                Color[] pixelColumn = new Color[frame.Height];

                for (int y = 0; y < frame.Height; y++)
                {
                    Color c = frame.GetPixel(x, y);
                    pixelColumn[y] = c;
                }
                
                output[x] = pixelColumn;
            }

            return new TextureFrame(output);
        }
        public Color GetPixel(int x, int y)
        {
            return frame[x][y];
        }
    }
}
