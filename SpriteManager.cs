using System.Collections.Generic;
using System.Numerics;

namespace DDA
{
    static class SpriteManager
    {
        public static IReadOnlyList<Sprite> WorldSprites { get => worldSprites.AsReadOnly(); }
        public static IReadOnlyList<Sprite> ScreenSprites { get => screenSprites.AsReadOnly(); }

        static List<Sprite> worldSprites = new List<Sprite>();
        static List<Sprite> screenSprites = new List<Sprite>();
        static List<Sprite> destroyedSprites = new List<Sprite>();
        
        public static void AddWorldSprite(Sprite sprite) => worldSprites.Add(sprite);
        public static void AddScreenSprite(Sprite sprite) => screenSprites.Add(sprite);
        public static void DepthSort(Vector2 playerPos)
        {
            worldSprites.Sort((Sprite s1, Sprite s2) => MathHelper.Distance(playerPos.X, playerPos.Y, s1.Position.X, s1.Position.Y).CompareTo(MathHelper.Distance(playerPos.X, playerPos.Y, s2.Position.X, s2.Position.Y)));
            worldSprites.Reverse();
        }
        public static Vector2 Project(Vector2 spritePos, Vector2 playerPos, Vector2 playerDir, Vector2 planeDir)
        {
            Vector2 relativePos = spritePos - playerPos;

            float invDet = 1 / (planeDir.X * playerDir.Y - playerDir.X * planeDir.Y);
            float transformX = invDet * (playerDir.Y * relativePos.X - playerDir.X * relativePos.Y);
            float transformY = invDet * (-planeDir.Y * relativePos.X + planeDir.X * relativePos.Y);
            
            return new Vector2(transformX, transformY);
        }
        public static void Destroy(Sprite sprite)
        {
            destroyedSprites.Add(sprite);
        }
        public static void RemoveDestroyed()
        {
            foreach (Sprite destroyed in destroyedSprites)
            {
                worldSprites.Remove(destroyed);
            }

            destroyedSprites.Clear();
        }
    }
}
