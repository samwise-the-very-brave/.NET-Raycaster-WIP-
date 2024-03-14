using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DDA
{
    class Renderer
    {
        public static int HorizontalResolution { get; set; } = 2;

        Pen pen = new Pen(Color.Black);
        BufferedGraphics bufferedGraphics;
        Buffer buffer;

        readonly Font debugFont = new Font("Courier New", 10);
        readonly Font uiFont = new Font("Courier New", 25);

        readonly int xOffset, yOffset;
        readonly int width, height;
        readonly int scale;

        public Renderer(GameWindow target, int width, int height, int scale, int xOffset, int yOffset)
        {
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(target.CreateGraphics(), new Rectangle(xOffset, yOffset, width * scale, height * scale));
            bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            buffer = new Buffer(new Bitmap(width, height), bufferedGraphics.Graphics, xOffset, yOffset, scale);

            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.width = width;
            this.height = height;
            this.scale = scale;
        }

        public void RenderBuffer() => buffer.Render();
        public void RenderSelf()
        {
            try
            {
                bufferedGraphics.Render();
            }
            catch (Exception) { }
        }
        public void Clear()
        {
            buffer.Clear();
            bufferedGraphics.Graphics.Clear(Color.Black);
        }
        public void SetBufferPixel(int x, int y, Color c) => buffer.SetPixel(x, y, c);
        public void ModifyBufferPixel(int x, int y, Color c) => buffer.SetTransparencyBufferPixel(x, y, c);
        public void DrawDebugLogs()
        {
            pen.Color = Color.White;
            int debugY = 0;

            foreach (DebugLog dl in Debug.Logs)
            {
                string separator = ": ";

                if (dl.Caption == string.Empty)
                    separator = "";

                Print($"{dl.Caption}{separator}{dl.Value}", 10, debugY, Color.White, debugFont);

                debugY += 15;
            }
        }
        public void DrawUI()
        {
            FontManager.AllText["ammo"].Content = PlayerResourceManager.Ammo[PlayerResourceManager.GetWeapon().AmmoType.ToString()].CurrentAmmo.ToString();
            FontManager.AllText["health"].Content = PlayerResourceManager.Health.ToString();
            FontManager.AllText["armor"].Content = PlayerResourceManager.Armor.ToString();

            foreach (KeyValuePair<string, Text> t in FontManager.AllText)
            {
                int xOff = 0;

                foreach (char c in t.Value.Content)
                {
                    bufferedGraphics.Graphics.DrawImage(FontManager.Characters[c], t.Value.ScreenX + xOff + xOffset, t.Value.ScreenY + yOffset, FontManager.FontWidth * t.Value.Scale, FontManager.FontHeight * t.Value.Scale);

                    xOff += t.Value.Spacing;
                }
            }
        }
        public void DrawScreenSprite(Image sprite, float x, float y, float width, float height)
        {
            bufferedGraphics.Graphics.DrawImage(sprite, x, y, width, height);
        }
        public void FillScreen(Color c)
        {
            pen.Color = c;
            bufferedGraphics.Graphics.FillRectangle(pen.Brush, new Rectangle(xOffset, yOffset, width * scale, height * scale));
        }
        public void Screenshot()
        {
            using (Bitmap screenshot = new Bitmap(width * scale, height * scale))
            {
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(new Point(xOffset - 10, yOffset - 10), new Point(0, 0), new Size(width * scale, height * scale));
                }

                Clipboard.SetImage(screenshot);
            }
        }
        public void Print(string text, int x, int y, Color c, Font f)
        {
            pen.Color = c;
            bufferedGraphics.Graphics.DrawString(text, f, pen.Brush, x + xOffset, y + yOffset);
        }
    }
}
