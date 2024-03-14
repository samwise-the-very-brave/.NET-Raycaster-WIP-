using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DDA
{
    public class Texture
    {
        TextureFrame[] frames;

        int loops = 0;

        public Texture(string path)
        {
            LoadFrom(path);
        }

        public TextureFrame GetFrame()
        {
            int currentIndex = (int)(Main.FrameStartTime * 30) - (frames.Length * loops);

            if (currentIndex >= frames.Length)
            {
                loops += currentIndex;
                currentIndex = 0;
            }

            if (currentIndex < 0)
                currentIndex = 0;
            System.Diagnostics.Debug.Assert(currentIndex >= 0 && currentIndex < frames.Length);
            return frames[currentIndex];
        }
        void LoadFrom(string path)
        {
            Bitmap source = new Bitmap(Image.FromFile(path));
            int frameCount = source.Width / 64;
            frames = new TextureFrame[frameCount];
            Bitmap[] bmpFrames = SplitBitmap(source);

            for (int i = 0; i < frameCount; i++)
            {
                if (bmpFrames[i] is not null)
                    frames[i] = TextureFrame.GetFrom(bmpFrames[i]);
            }
        }
        static Bitmap[] SplitBitmap(Bitmap source)
        {
            int frameCount = source.Width / 64;
            byte[][] outputData = new byte[frameCount][];

            byte[] sourceData = BitmapToArray(source);

            for (int y = 0; y < source.Height; y++)
            {
                int bx = 0;

                for (int x = 0; x < source.Width; x++)
                {
                    int bmpIndex = x / 64;

                    if (outputData[bmpIndex] is null)
                        outputData[bmpIndex] = new byte[16384];

                    if (x % 64 == 0)
                        bx = 0;

                    int sourceIndex = (source.Width * y + x) * 4;
                    byte r = sourceData[sourceIndex + 2];
                    byte g = sourceData[sourceIndex + 1];
                    byte b = sourceData[sourceIndex];

                    //Color correspondingPaletteColor = PaletteManager.MainPalette.GetClosestTo(Color.FromArgb(r, g, b));
                    
                    int outputIndex = (64 * y + bx) * 4;
                    outputData[bmpIndex][outputIndex] = b;
                    outputData[bmpIndex][outputIndex + 1] = g;
                    outputData[bmpIndex][outputIndex + 2] = r;
                    outputData[bmpIndex][outputIndex + 3] = 255;

                    bx++;
                }
            }

            Bitmap[] output = new Bitmap[outputData.Length];

            for (int i = 0; i < outputData.Length; i++)
            {
                output[i] = ArrayToBitmap(outputData[i]);
            }

            return output;
        }
        public static Bitmap ArrayToBitmap(byte[] array)
        {
            Bitmap bmp = new Bitmap(64, 64);

            Rectangle copyRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(copyRect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr bPointer = data.Scan0;
            Marshal.Copy(array, 0, bPointer, array.Length);
            bmp.UnlockBits(data);

            return bmp;
        }
        public static byte[] BitmapToArray(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr pointer = bData.Scan0;
            int bytes = Math.Abs(bData.Stride) * bmp.Height;
            byte[] sourceData = new byte[bytes];
            Marshal.Copy(pointer, sourceData, 0, bytes);
            bmp.UnlockBits(bData);

            return sourceData;
        }
        public Color this[int x, int y] => GetFrame().GetPixel(x, y);
    }
}
