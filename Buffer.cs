using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DDA
{
    class Buffer
    {
        public int Width { get; init; }
        public int Height { get; init; }

        Graphics graphics;
        byte[] buffer;
        byte[] transparencyBuffer;
        Bitmap imageBuffer;
        Bitmap transparencyImageBuffer;
        readonly int xOffset;
        readonly int yOffset;
        readonly int renderingScale;

        public Buffer(Image image, Graphics context, int xOffset, int yOffset, int renderingScale)
        {
            graphics = context;
            buffer = new byte[image.Width * image.Height * 4];
            transparencyBuffer = new byte[image.Width * image.Height * 4];

            Width = image.Width;
            Height = image.Height;

            imageBuffer = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            transparencyImageBuffer = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            this.renderingScale = renderingScale;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        public void SetPixel(int x, int y, Color c)
        {            
            for (int i = 0; i < Renderer.HorizontalResolution; i++)
            {
                int index = (Width * y + x + i) * 4;
                SetPixel(index, c, ref buffer);
            }
        }
        
        public void AlphaBlendPixel(int x, int y, Color colorA)
        {
            for (int i = 0; i < Renderer.HorizontalResolution; i++)
            {                
                int index = (Width * y + x + i) * 4;
                Color colorB = Color.FromArgb(buffer[index + 3], buffer[index + 2], buffer[index + 1], buffer[index]);

                float colorA_Alpha = (float)colorA.A / 255;
                
                int newR = (int)(colorB.R * (1 - colorA_Alpha) + colorA.R * colorA_Alpha);
                int newG = (int)(colorB.G * (1 - colorA_Alpha) + colorA.G * colorA_Alpha);
                int newB = (int)(colorB.B * (1 - colorA_Alpha) + colorA.B * colorA_Alpha);

                Color newC = Color.FromArgb(newR, newG, newB);

                SetPixel(index, newC, ref buffer);
            }
        }
        public void SetTransparencyBufferPixel(int x, int y, Color c)
        {
            for (int i = 0; i < Renderer.HorizontalResolution; i++)
            {
                int index = (Width * y + x + i) * 4;
                SetPixel(index, c, ref transparencyBuffer);
            }
        }
        public void Render()
        {
            SetImageBuffers();

            graphics.DrawImage(imageBuffer, xOffset, yOffset, imageBuffer.Width * renderingScale, imageBuffer.Height * renderingScale);
            graphics.DrawImage(transparencyImageBuffer, xOffset, yOffset, transparencyImageBuffer.Width * renderingScale, transparencyImageBuffer.Height * renderingScale);
        }
        public void Clear()
        {
            buffer = new byte[Width * Height * 4];
            transparencyBuffer = new byte[Width * Height * 4];
        }
        void SetImageBuffers()
        {
            Rectangle copyRect = new Rectangle(0, 0, Width, Height);
            BitmapData data = imageBuffer.LockBits(copyRect, ImageLockMode.ReadWrite, imageBuffer.PixelFormat);
            IntPtr bPointer = data.Scan0;
            Marshal.Copy(buffer, 0, bPointer, buffer.Length);
            imageBuffer.UnlockBits(data);

            copyRect = new Rectangle(0, 0, Width, Height);
            data = transparencyImageBuffer.LockBits(copyRect, ImageLockMode.ReadWrite, imageBuffer.PixelFormat);
            bPointer = data.Scan0;
            Marshal.Copy(transparencyBuffer, 0, bPointer, transparencyBuffer.Length);
            transparencyImageBuffer.UnlockBits(data);
        }
        void SetPixel(int index, Color c, ref byte[] buff)
        {
            buff[index] = c.B;
            buff[index + 1] = c.G;
            buff[index + 2] = c.R;
            buff[index + 3] = c.A;
        }
    }
}