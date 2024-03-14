using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace DDA
{
    class Text
    {
        public string Content { get; set; }
        public int Scale { get; set; }
        public int Spacing { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public string Tag { get; private set; }

        Dictionary<char, Image> font;

        public Text(string text, int scale, int spacing, int screenX, int screenY, string tag)
        {
            font = (Dictionary<char, Image>)FontManager.Characters;
            Content = text;
            Scale = scale;
            Spacing = spacing;
            ScreenX = screenX;
            ScreenY = screenY;
            Tag = tag;

            SetSize(scale);

            FontManager.Add(this);
        }

        //thank you, Stack Overflow
        Bitmap ResizeImage(ref Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public void SetSize(int scale)
        {
            for (int i = 0; i < font.Count; i++)
            {
                Image currentChar = font.ElementAt(i).Value;
                ResizeImage(ref currentChar, FontManager.FontWidth * scale, FontManager.FontHeight * scale);
            }
        }

    }
}
