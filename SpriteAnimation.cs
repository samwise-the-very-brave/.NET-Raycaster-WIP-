using System;
using System.Collections.Generic;
using System.Drawing;

namespace DDA
{
    public class SpriteAnimation
    {
        public IReadOnlyCollection<Frame> AnimationFrames { get => frames; }
        public string Name { get; init; }
        public bool CanAnimate { get; private set; }
        public bool Loop { get; init; }

        Frame[] frames;
        int currentFrame = 0;
        float millisecs = 0;

        SpriteAnimation(Frame[] frames, string name, bool canLoop)
        {
            this.frames = frames;
            Name = name;
            CanAnimate = true;
            Loop = canLoop;
        }

        public static SpriteAnimation FromSpriteDataChunk(string spriteFileAnimationChunk, string spriteName)
        {
            if (!spriteFileAnimationChunk.StartsWith("#anim"))
            {
                throw new ArgumentException("Argument is not valid.", spriteFileAnimationChunk);
            }

            string[] split = spriteFileAnimationChunk.Replace("#anim", string.Empty).Split(',');

            string name = split[0].Trim();

            Frame[] outputFrames = new Frame[split.Length - 1];

            for (int i = 1; i < split.Length; i++)
            {
                string[] currentFrameData = split[i].Split('-');

                if (currentFrameData.Length == 1)
                    throw new ArgumentException("Argument is not valid.", spriteFileAnimationChunk);

                outputFrames[i - 1] = new Frame(Image.FromFile(@$"..\..\..\sprites\{spriteName}\{currentFrameData[0].Trim()}"), float.Parse(currentFrameData[1].Trim()));
            }

            return new SpriteAnimation(outputFrames, name, false);
        }
        public void Reset() => CanAnimate = true;
        public void StopAnimation() => CanAnimate = false;
        public void Animate(ref Image frame, out int frameIndex)
        {
            if (!CanAnimate)
            {
                frameIndex = 0;
                return;
            }

            millisecs += 100f / Main.targetFPS;

            if (millisecs >= frames[currentFrame].Delay * 100)
            {
                currentFrame++;

                if (currentFrame >= frames.Length)
                {
                    currentFrame = 0;

                    if (!Loop)
                    {
                        CanAnimate = false;
                    }
                }

                millisecs = 0;
                frame = frames[currentFrame].FrameImage;
            }

            frameIndex = currentFrame;
        }

        public struct Frame
        {
            public Image FrameImage { get; set; }
            public float Delay { get; set; }

            public Frame(Image img, float delay)
            {
                FrameImage = img;
                Delay = delay;
            }
        }
    }
}
