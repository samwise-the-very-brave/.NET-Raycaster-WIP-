using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

namespace DDA
{
    class Sprite : IComparable<Sprite>
    {
        public string Name { get; init; }
        public Vector2 Position { get; set; }
        public Size Size { get; set; }
        public Image Img { get; set; }
        protected bool IsAWorldObject { get; init; }
        public SpriteScalingMode ScalingMode { get; init; }
        public string Layer { get; set; }
        public IReadOnlyList<SpriteAnimation> Animations { get => animations; }
        public bool Visible { get; set; } = true;
        protected int currentAnimationIndex = 0;
        public SpriteAnimation CurrentAnimation
        {
            get
            {
                if (Animations.Count > 0)
                    return Animations.ElementAt(currentAnimationIndex);
                else
                    return null;
            }
        }
        public int TargetScaleWidth { get; set; }
        public SizeF HitboxSize { get; set; }

        List<List<Color[][]>> colorMaps = new List<List<Color[][]>>();
        Image b;
        int frameIndex = 0;

        List<SpriteAnimation> animations = new List<SpriteAnimation>();

        public Sprite(string spriteName, bool isAWorldObject)
        {
            ReadSpriteData(spriteName);
            Name = spriteName;
            Position = Vector2.Zero;
            IsAWorldObject = isAWorldObject;

            if (isAWorldObject)
            {
                SpriteManager.AddWorldSprite(this);
            }
            else
            {
                SpriteManager.AddScreenSprite(this);
            }

            Layer = "Default";
            Size = new Size(Img.Width, Img.Height);
            b = Img.Clone() as Image;

            Start();
        }

        public Sprite(string spriteName, float x, float y, bool isAWorldObject)
        {
            ReadSpriteData(spriteName);
            Name = spriteName;
            Position = new Vector2(x, y);
            IsAWorldObject = isAWorldObject;

            if (isAWorldObject)
            {
                SpriteManager.AddWorldSprite(this);
            }
            else
            {
                SpriteManager.AddScreenSprite(this);
            }

            Layer = "Default";
            Size = new Size(Img.Width, Img.Height);
            b = Img.Clone() as Image;

            Start();
        }

        public int CompareTo(Sprite other)
        {
            return other.CompareTo(this);
        }
        public void Tick()
        {
            Update();

            if (new RectangleF(Position.X - HitboxSize.Width / 2, Position.Y - HitboxSize.Height / 2, HitboxSize.Width, HitboxSize.Height).IntersectsWith(new RectangleF(Main.playerPos.X, Main.playerPos.Y, Main.playerHitboxSize.Width, Main.playerHitboxSize.Height)))
            {
                OnCollisionWithPlayer();
            }

            if (CurrentAnimation is not null)
            {
                CurrentAnimation.Animate(ref b, out frameIndex);
                Img = b;
            }
        }
        public Color[][] GetColorMap()
        {
            if (colorMaps.Count > 0)
            {
                return colorMaps[currentAnimationIndex][frameIndex];
            }
            else
            {
                return GetVerticalStripes(new Bitmap(Img));
            }
        }
        string GetFileName(string filePath)
        {
            return filePath.Split(@"\").Last().Split(".")[0];
        }
        Color[][] GetVerticalStripes(Bitmap source)
        {
            Color[][] output = new Color[source.Width][];

            for (int x = 0; x < source.Width; x++)
            {
                Color[] pixelColumn = new Color[source.Height];

                for (int y = 0; y < source.Height; y++)
                {
                    Color c = source.GetPixel(x, y);
                    pixelColumn[y] = c;
                }

                output[x] = pixelColumn;
            }

            return output;
        }
        void ReadSpriteData(string spriteName)
        {
            string[] lines = File.ReadAllLines(@$"..\..\..\sprites\{spriteName}\{spriteName}.txt");

            Img = Image.FromFile(@$"..\..\..\sprites\{spriteName}\{lines[0].Split(" ")[1]}");

            if (lines.Length == 1)
                return;

            for (int i = 1; i < lines.Length; i++)
            {
                animations.Add(SpriteAnimation.FromSpriteDataChunk(lines[i], spriteName));
                FramesToColorMaps(animations[i - 1], i - 1);
            }
        }
        void FramesToColorMaps(SpriteAnimation source, int animationIndex)
        {
            colorMaps.Add(new List<Color[][]>());

            for (int i = 0; i < source.AnimationFrames.Count; i++)
            {
                colorMaps[animationIndex].Add(GetVerticalStripes((Bitmap)source.AnimationFrames.ElementAt(i).FrameImage));
            }
        }
        public virtual void Start()
        {
            return;
        }
        public virtual void Update()
        {
            return;
        }
        public virtual void OnCollisionWithPlayer()
        {
            return;
        }
    }

    enum SpriteScalingMode
    {
        DoNotScale = 0,
        ScaleToScreen = 1
    }
}
