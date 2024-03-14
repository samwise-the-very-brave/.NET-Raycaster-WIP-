/*
- pozbyć się floatów z programu (choćby dla zasady, bo nic to nie poprawi)
- pozbyć się tylu ifów, ile to możliwe (z głównej pętli)
+ zrobić do końca przezroczyste ściany
- zrobić "cienkie ściany"
- zrobić drzwi
- push walle
- hitscany
- guziory
- ewentualnie "windy"
- poprawić sprajty
- zrobić jakieś interaktywne UI (menu itd)
- zrobić poważny edytor poziomów (W C albo C++, z SDL)
- sprawić, by renderer umiał mówić w trójkątach
- przerzucić frontend do WPF, które ma wsparcie dla DirectX
- przerzucić to, co trzeba, do oddzielnego projektu i zrobić z tego bibliotekę na wzór raycastliba
- przepisać bibliotekę w C
- ewentualnie zrobić w tym jakąś grę 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDA
{
    static class Main
    {
        public static GameWindow GameWindow { get; private set; }
        public static ulong Ticks { get; private set; } = 0;
        public static float DeltaTime { get; private set; } = 0;
        public static Map Map { get; private set; }
        public static double FrameStartTime { get; private set; }

        static Renderer renderer;

        public const int targetFPS = 60;
        public const int gameWindowWidth = 480;
        public const int gameWindowHeight = 320;
        public const int graphicsScale = 3;

        static Stopwatch frameStopwatch;
        static Stopwatch time;
        static Stopwatch animationStopwatch;

        const float degInRad = 0.017453f;

        static readonly int gameWindowXOffset = (int)Math.Floor(((float)Screen.PrimaryScreen.Bounds.Width - gameWindowWidth * graphicsScale) / 2);
        static readonly int gameWindowYOffset = (int)Math.Floor(((float)Screen.PrimaryScreen.Bounds.Height - gameWindowHeight * graphicsScale) / 2);
        const int scale = 64;
        const int rayCount = 480;
        const float fov = 66;
        const float darkerWallShade = 0.65f;
        const float lighterWallShade = 0.9f;
        const float wallHeightFactor = 1f;
        const int spritesOffset = 128;

        public static Vector2 playerPos = new Vector2(3, 3);
        public static Size playerHitboxSize = new Size(64, 64);
        static Vector2 playerDir = new Vector2(-1, 0);
        static Vector2 planeDir = new Vector2(0, fov / 100);

        static readonly float playerMoveSpeed = 0.25f;
        static float playerRotateSpeed = 220;

        static bool upPressed = false, downPressed = false, moveLeftPressed = false, moveRightPressed = false, rotateLeftPressed = false, rotateRightPressed = false;

        static float oldTime;
        static float newTime;
        static float flash = 0;
        static Color flashColor = Color.Transparent;

        static bool moving = false;

        static readonly int xScalingFactor = gameWindowWidth / rayCount;
        static readonly float[] distanceToWalls = new float[gameWindowWidth];
        static Point oldMousePos = Point.Empty;

        static double fpsRecord = 0;
        static double loadingTime = 0;

        public static void Init(GameWindow window)
        {
            Stopwatch loadingStopwatch = new Stopwatch();
            loadingStopwatch.Start();

            GameWindow = window;

            Cursor.Hide();

            renderer = new Renderer(window, gameWindowWidth, gameWindowHeight, graphicsScale, gameWindowXOffset, gameWindowYOffset);
            Renderer.HorizontalResolution = xScalingFactor;

            PaletteManager.Init();
            FontManager.LoadFont();
            PlayerResourceManager.Init();
            AudioManager.LoadSounds();
            AudioManager.LoadMusic();

            Map = new Map(@"..\..\..\textures\small_map.txt");

            frameStopwatch = new Stopwatch();
            frameStopwatch.Start();

            time = new Stopwatch();
            time.Start();

            animationStopwatch = new Stopwatch();

            playerPos = Map.PlayerStart;

            Text ammo = new Text("24", 2, 18, 32, gameWindowHeight * graphicsScale - 56, "ammo");
            Text health = new Text("100%", 2, 18, 32, gameWindowHeight * graphicsScale - 128, "health");
            Text armor = new Text("0%", 2, 18, 32, gameWindowHeight * graphicsScale - 94, "armor");

            //Frenchman frenchman = new Frenchman("kuktas_kajetan", 5, 12);
            //AmmoPickup ammoPickup = new AmmoPickup("ammo_pickup", 2, 8, 4, AmmoType.Shells);
            Weapon fists = new Weapon("fists", 0, AmmoType.None)
            {
                Layer = "Weapons",
                ScalingMode = SpriteScalingMode.ScaleToScreen,
                TargetScaleWidth = gameWindowWidth,
                Visible = false
            };

            Sprite crosshair = new Sprite("crosshair", gameWindowWidth * graphicsScale / 2 - 32, gameWindowHeight * graphicsScale / 2 - 32, false);
            crosshair.Size = new Size(32, 32);
            /*Sprite pistol = new Sprite("pistol", gameWindowWidth * graphicsScale / 2 - 128, gameWindowHeight * graphicsScale - 260 * graphicsScale, false);
            pistol.ScalingMode = SpriteScalingMode.ScaleToScreen;
            pistol.TargetScaleWidth = 256;
            pistol.Layer = "Weapons";*/

            Decal oskar = new Decal("oskar", 10, 15, 0.1f, 0.9f);
            Decal oskar2 = new Decal("oskar", 11, 15, 0.1f, 0.9f);
            Decal oskar3 = new Decal("oskar", 12, 15, 0.1f, 0.9f);
            Decal oskar4 = new Decal("oskar", 13, 15, 0.1f, 0.9f);
            Decal oskar5 = new Decal("oskar", 14, 15, 0.1f, 0.9f);

            //AudioManager.StartTrack("test_map", "bad_apple");
            loadingStopwatch.Stop();
            loadingTime = loadingStopwatch.Elapsed.TotalSeconds;

            animationStopwatch.Start();
        }
        public static void Tick()
        {
            if (!GameWindow.Focused)
                return;

            renderer.Clear();
            Debug.ClearLogs();
            SpriteManager.RemoveDestroyed();

            frameStopwatch.Restart();
            oldTime = (float)frameStopwatch.Elapsed.TotalSeconds;

            KeyInput();
            MouseInput();
            MovePlayer();
            moving = downPressed || moveLeftPressed || moveRightPressed || upPressed;
            Raycast();

            newTime = (float)frameStopwatch.Elapsed.TotalSeconds;
            double fps = Math.Round(1 / newTime - oldTime);

            if (fps > fpsRecord)
                fpsRecord = fps;

            Debug.Log(fps, "FPS");
            Debug.Log(fpsRecord, "Highest fps");
            Debug.Log(DeltaTime, "Delta time (seconds)");
            Debug.Log(loadingTime, "Loading time (seconds)");
            oldMousePos = new Point(Cursor.Position.X, Cursor.Position.Y);

            Render();
            DeltaTime = newTime - oldTime;
            Ticks++;
        }
        public static void ScreenFlash(float flashTime, Color c)
        {
            flash = flashTime;
            flashColor = c;
        }
        static void KeyInput()
        {
            if (Input.IsKeyPressed(Keys.W))
            {
                upPressed = true;
            }
            else
            {
                upPressed = false;
            }

            if (Input.IsKeyPressed(Keys.A))
            {
                moveLeftPressed = true;
            }
            else
            {
                moveLeftPressed = false;
            }

            if (Input.IsKeyPressed(Keys.S))
            {
                downPressed = true;
            }
            else
            {
                downPressed = false;
            }

            if (Input.IsKeyPressed(Keys.D))
            {
                moveRightPressed = true;
            }
            else
            {
                moveRightPressed = false;
            }

            if (Input.IsKeyPressed(Keys.Left))
            {
                rotateLeftPressed = true;
            }
            else
            {
                rotateLeftPressed = false;
            }

            if (Input.IsKeyPressed(Keys.Right))
            {
                rotateRightPressed = true;
            }
            else
            {
                rotateRightPressed = false;
            }

            if (Input.IsKeyPressed(Keys.Enter))
            {
                renderer.Screenshot();
            }

            if (Input.IsKeyPressed(Keys.E))
            {
                Door door = Map.GetFirstDoorInRadius(playerPos, 2);

                if (door is not null)
                {
                    door.Interact();
                }
            }
        }
        static void MouseInput()
        {
            if (Input.IsMousePressed(MouseButtons.Left))
            {
                Weapon playerWeapon = PlayerResourceManager.GetWeapon();
                playerWeapon.Fire();

                if (playerWeapon.AmmoType == AmmoType.None)
                {
                    RectangleF punchBox = new RectangleF(playerPos.X + 1 * playerDir.X, playerPos.Y + 1 * playerDir.Y, 1, 1);
                    Enemy hitEnemy = EnemyManager.Enemies.FirstOrDefault(e => new RectangleF(e.Position.X, e.Position.Y, e.HitboxSize.Width, e.HitboxSize.Height).IntersectsWith(punchBox));

                    if (hitEnemy is not null)
                    {
                        hitEnemy.TakeDamage(10);
                    }
                }
            }
        }
        static void MovePlayer()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            if (Cursor.Position.X == 0)
            {
                Cursor.Position = new Point(Cursor.Position.X + screenWidth, Cursor.Position.Y);
                oldMousePos = new Point(oldMousePos.X + screenWidth, Cursor.Position.Y);
            }
            else if (Cursor.Position.X == screenWidth - 1)
            {
                Cursor.Position = new Point(Cursor.Position.X - screenWidth, Cursor.Position.Y);
                oldMousePos = new Point(oldMousePos.X - screenWidth, Cursor.Position.Y);
            }

            float mouseXAxis = (oldMousePos.X - Cursor.Position.X) / (float)gameWindowWidth;

            playerDir = RotateVector(playerDir, degInRad * mouseXAxis * playerRotateSpeed);
            planeDir = RotateVector(planeDir, degInRad * mouseXAxis * playerRotateSpeed);

            /* if (rotateRightPressed)
            {
                float oldAngle = angle;
                angle += playerRotateSpeed;
                float beta = MathF.Abs(angle - oldAngle);

                playerDir = RotateVector(playerDir, degInRad * -beta);
                planeDir = RotateVector(planeDir, degInRad * -beta);
            }

            if (rotateLeftPressed)
            {
                float oldAngle = angle;
                angle -= playerRotateSpeed;
                float beta = MathF.Abs(angle - oldAngle);

                playerDir = RotateVector(playerDir, degInRad * beta);
                planeDir = RotateVector(planeDir, degInRad * beta);
            }*/

            if (upPressed)
            {
                if (Map.FloorsAndWalls[(int)(playerPos.X + playerDir.X * playerMoveSpeed), (int)(playerPos.Y)].TextureType < 1)
                    playerPos.X += playerDir.X * playerMoveSpeed;

                if (Map.FloorsAndWalls[(int)(playerPos.X), (int)(playerPos.Y + playerDir.Y * playerMoveSpeed)].TextureType < 1)
                    playerPos.Y += playerDir.Y * playerMoveSpeed;
            }

            if (downPressed)
            {
                if (Map.FloorsAndWalls[(int)(playerPos.X - playerDir.X * playerMoveSpeed), (int)(playerPos.Y)].TextureType < 1)
                    playerPos.X -= playerDir.X * playerMoveSpeed;

                if (Map.FloorsAndWalls[(int)(playerPos.X), (int)(playerPos.Y - playerDir.Y * playerMoveSpeed)].TextureType < 1)
                    playerPos.Y -= playerDir.Y * playerMoveSpeed;
            }

            if (moveLeftPressed)
            {
                if (Map.FloorsAndWalls[(int)(playerPos.X - planeDir.X * playerMoveSpeed), (int)(playerPos.Y)].TextureType < 1)
                    playerPos.X -= planeDir.X * playerMoveSpeed;

                if (Map.FloorsAndWalls[(int)(playerPos.X), (int)(playerPos.Y - planeDir.Y * playerMoveSpeed)].TextureType < 1)
                    playerPos.Y -= planeDir.Y * playerMoveSpeed;
            }

            if (moveRightPressed)
            {
                if (Map.FloorsAndWalls[(int)(playerPos.X + planeDir.X * playerMoveSpeed), (int)(playerPos.Y)].TextureType < 1)
                    playerPos.X += planeDir.X * playerMoveSpeed;

                if (Map.FloorsAndWalls[(int)(playerPos.X), (int)(playerPos.Y + planeDir.Y * playerMoveSpeed)].TextureType < 1)
                    playerPos.Y += planeDir.Y * playerMoveSpeed;
            }
        }
        static void Raycast()
        {
            FrameStartTime = animationStopwatch.Elapsed.TotalSeconds;

            Task firstHalf = Task.Run(() => WallCast(0, rayCount / 2));
            Task secondHalf = Task.Run(() => WallCast(rayCount / 2, rayCount));
            Task.WaitAll(firstHalf, secondHalf);
            DrawSprites();
        }
        static void Render()
        {
            renderer.RenderBuffer();
            renderer.DrawDebugLogs();
            DrawScreenSprites();
            renderer.DrawUI();

            renderer.RenderSelf();
        }
        static void WallCast(int firstRayNum, int lastRayNum)
        {
            int mapX, mapY;

            float movementOffset = MathF.Sin((float)time.Elapsed.TotalMilliseconds * 0.01f) * 2.1f;
            //movementOffset = 0;

            for (int r = firstRayNum; r < lastRayNum; r++)
            {
                int x = r * xScalingFactor;
                float cameraX = (2 * x / (float)gameWindowWidth - 1);

                if (moving)
                    cameraX += movementOffset / 700;

                float rayDirX = playerDir.X + planeDir.X * cameraX;
                float rayDirY = playerDir.Y + planeDir.Y * cameraX;

                mapX = (int)playerPos.X;
                mapY = (int)playerPos.Y;

                float sideDistX;
                float sideDistY;

                float deltaDistX = (rayDirX == 0) ? float.PositiveInfinity : MathF.Abs(1 / rayDirX);
                float deltaDistY = (rayDirY == 0) ? float.PositiveInfinity : MathF.Abs(1 / rayDirY);
                float perpWallDist = 0;

                int stepX;
                int stepY;

                bool hit = false;
                int side = 0;

                List<RayHit> wallsHit = new List<RayHit>();

                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (playerPos.X - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0f - playerPos.X) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (playerPos.Y - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0f - playerPos.Y) * deltaDistY;
                }

                while (!hit)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (Map.FloorsAndWalls[mapX, mapY].TextureType > 0)
                    {
                        if (Map.FloorsAndWalls[mapX, mapY].TextureType == 7)
                        {
                            switch (side)
                            {
                                case 0:
                                    if ((rayDirX > 0 && WallTilePresent(mapX - 1, mapY)) || (rayDirX < 0 && WallTilePresent(mapX + 1, mapY)))
                                        continue;
                                    break;

                                case 1:
                                    if ((rayDirY > 0 && WallTilePresent(mapX, mapY - 1)) || (rayDirY < 0 && WallTilePresent(mapX, mapY + 1)))
                                        continue;
                                    break;

                                default:
                                    break;
                            }
                        }

                        if (side == 0)
                            perpWallDist = sideDistX - deltaDistX;
                        else
                            perpWallDist = sideDistY - deltaDistY;


                        //thin walls test
                        /*if (map.FloorsAndWalls[mapX, mapY].TextureType == 7 && side == 0)
                            continue;*/

                        wallsHit.Add(new RayHit(perpWallDist, new Vector2(rayDirX, rayDirY), new Point(mapX, mapY), side));

                        if (Map.FloorsAndWalls[mapX, mapY].TextureType != 7 /*true*/)
                        {
                            hit = true;
                        }
                    }
                }

                distanceToWalls[x] = perpWallDist;

                for (int i = wallsHit.Count - 1; i >= 0; i--)
                {
                    float pWallDist = wallsHit[i].PerpDist;
                    float rDirX = wallsHit[i].Direction.X;
                    float rDirY = wallsHit[i].Direction.Y;
                    Point hitPoint = wallsHit[i].HitPoint;
                    int? textureType = Map.FloorsAndWalls[hitPoint.X, hitPoint.Y].TextureType;

                    int lineHeight = (int)(gameWindowHeight / pWallDist * wallHeightFactor);
                    int lineStart = -lineHeight / 2 + gameWindowHeight / 2;
                    int lineEnd = lineHeight / 2 + gameWindowHeight / 2;
                    int k = lineEnd - lineStart;
                    if (lineStart < 0)
                        lineStart = 0;

                    if (lineEnd >= gameWindowHeight)
                        lineEnd = gameWindowHeight - 1;

                    float wallX;

                    if (wallsHit[i].Side == 0)
                        wallX = playerPos.Y + pWallDist * rDirY;
                    else
                        wallX = playerPos.X + pWallDist * rDirX;

                    wallX -= MathF.Floor(wallX);

                    Decal dec = DecalManager.Decals.FirstOrDefault(d => d.MapX == hitPoint.X && d.MapY == hitPoint.Y && d.WallXStart <= wallX && d.WallXEnd > wallX);

                    bool wallHasDecal = dec is not null;

                    int ty = 0;
                    int textureX = (int)(wallX * 64);

                    if ((wallsHit[i].Side == 0 && rDirX > 0) || (wallsHit[i].Side == 1 && rDirY < 0))
                        textureX = 64 - textureX - 1;

                    float dy = (float)lineHeight / scale;

                    float shade = 0;

                    switch (wallsHit[i].Side)
                    {
                        case 0:
                            shade = darkerWallShade;
                            break;
                        case 1:
                            shade = lighterWallShade;
                            break;

                        default:
                            break;
                    }

                    shade /= pWallDist / 4;
                    shade *= 0.5f;

                    if (shade > 1)
                        shade = 1;

                    Texture currentTexture = TextureManager.AllTextures[(int)textureType];
                    int lineOffset = gameWindowHeight / 2 - lineHeight / 2;

                    int alpha = 255;

                    if (textureType == 7)
                        alpha = 64;


                    /*if (moving)
                        lineOffset += movementOffset;*/

                    DrawWallSlice(lineStart, lineEnd, lineHeight, shade, x, textureX, currentTexture, wallHasDecal, dec, alpha);

#if true
                    DrawFloorPixelColumn(wallsHit[i], new Vector2(rDirX, rDirY), lineHeight, lineOffset, pWallDist, wallX, shade, x);
#endif
                }
            }
        }
        static void DrawFloorPixelColumn(RayHit hit, Vector2 rayDir, int lineHeight, int lineOffset, float perpWallDist, float wallX, float shade, int x)
        {
            float floorTexelX, floorTexelY;

            if (hit.Side == 0 && rayDir.X > 0)
            {
                floorTexelX = hit.HitPoint.X;
                floorTexelY = hit.HitPoint.Y + wallX;
            }
            else if (hit.Side == 0 && rayDir.X < 0)
            {
                floorTexelX = hit.HitPoint.X + 1f;
                floorTexelY = hit.HitPoint.Y + wallX;
            }
            else if (hit.Side == 1 && rayDir.Y > 0)
            {
                floorTexelX = hit.HitPoint.X + wallX;
                floorTexelY = hit.HitPoint.Y;
            }
            else
            {
                floorTexelX = hit.HitPoint.X + wallX;
                floorTexelY = hit.HitPoint.Y + 1f;
            }

            for (int y = lineHeight + lineOffset; y < gameWindowHeight && lineHeight > 0; y++)
            {
                float currentDist = gameWindowHeight / (2f * y - gameWindowHeight);

                float weight = currentDist / perpWallDist;

                float floorX = weight * floorTexelX + (1 - weight) * playerPos.X;
                float floorY = weight * floorTexelY + (1 - weight) * playerPos.Y;

                int textureX = (int)Math.Abs(floorX * 64) % 64;
                int textureY = (int)Math.Abs(floorY * 64) % 64;

                int? floorType = Map.FloorsAndWalls[(int)floorX & Map.Width - 1, (int)floorY & Map.Height - 1].TextureType;

                if (floorType >= 0)
                    continue;

                floorType = -floorType;

                int ceilingType = Math.Abs((int)Map.Ceilings[(int)floorX & Map.Width - 1, (int)floorY & Map.Height - 1].TextureType);

                float distShade = 1 / ((MathHelper.Distance(playerPos.X, playerPos.Y, floorX, floorY) / 2) + 0.1f) * 0.5f;
                //distShade = 1;
                shade = 1;

                if (shade > 1)
                    shade = 1;

                if (distShade > 1)
                    distShade = 1;

                Color floorColor = TextureManager.AllTextures[(int)floorType][textureX, textureY];
                floorColor = Color.FromArgb((int)(floorColor.R * shade * distShade), (int)(floorColor.G * shade * distShade), (int)(floorColor.B * shade * distShade));

                if (ceilingType != 0)
                {
                    Color ceilingColor = TextureManager.AllTextures[ceilingType][textureX, textureY];
                    ceilingColor = Color.FromArgb((int)(ceilingColor.R * distShade), (int)(ceilingColor.G * distShade), (int)(ceilingColor.B * distShade));
                    renderer.SetBufferPixel(x, gameWindowHeight - y, ceilingColor);
                }

                renderer.SetBufferPixel(x, y, floorColor);
            }
        }
        static void DrawSprites()
        {
            SpriteManager.DepthSort(playerPos);

            foreach (Sprite sprite in SpriteManager.WorldSprites)
            {
                sprite.Tick();

                if (sprite is Frenchman)
                {
                    Frenchman currentFrenchman = sprite as Frenchman;
                    Vector2 relativePos = playerPos - sprite.Position;
                    float angle = MathF.Atan2(relativePos.X, relativePos.Y);
                    angle *= 180 / MathF.PI;
                    currentFrenchman.RelativeAngle = angle;
                    Debug.Log(angle, "relative pos");
                }

                Vector2 projected = SpriteManager.Project(sprite.Position, playerPos, playerDir, planeDir);
                int spriteYOffset = (int)(spritesOffset / projected.Y);
                int screenX = (int)((gameWindowWidth / 2) * (1 + projected.X / projected.Y));
                int spriteSize = Math.Abs((int)(gameWindowHeight / projected.Y));
                int drawStartY = -spriteSize / 2 + gameWindowHeight / 2 + spriteYOffset;

                if (drawStartY < 0)
                    drawStartY = 0;

                int drawEndY = spriteSize / 2 + gameWindowHeight / 2 + spriteYOffset;

                if (drawEndY > gameWindowHeight)
                    drawEndY = gameWindowHeight - 1;

                int drawStartX = -spriteSize / 2 + screenX;

                if (drawStartX < 0)
                    drawStartX = 0;

                int drawEndX = spriteSize / 2 + screenX;

                if (drawEndX > gameWindowWidth)
                    drawEndX = gameWindowWidth - 1;

                float shade = projected.Y / 4;

                if (shade < 1)
                    shade = 1;

                Color[][] spriteColorMap = sprite.GetColorMap();

                for (int x = drawStartX; x < drawEndX; x++)
                {
                    if (projected.Y > 0 && x > 0 && x < gameWindowWidth && projected.Y < distanceToWalls[x])
                    {
                        int textureX = (256 * (x - (-spriteSize / 2 + screenX)) * 64 / spriteSize) / 256;

                        for (int y = drawStartY; y < drawEndY; y++)
                        {
                            int dy = (y - spriteYOffset) * 256 - gameWindowHeight * 128 + spriteSize * 128;
                            int textureY = ((dy * 64) / spriteSize) / 256;

                            if (textureX < 0 || textureY < 0)
                                continue;

                            Color c = spriteColorMap[textureX][textureY];
                            c = Color.FromArgb(c.A, (int)(c.R / shade), (int)(c.G / shade), (int)(c.B / shade));

                            if (c.A < 128)
                                continue;

                            renderer.SetBufferPixel(x, y, c);
                        }
                    }
                }
            }
        }
        static void DrawScreenSprites()
        {

            foreach (Sprite sprite in SpriteManager.ScreenSprites)
            {
                if (!sprite.Visible || (sprite is Weapon && PlayerResourceManager.GetWeapon() != sprite as Weapon))
                    continue;

                float movementOffset = 0;

                sprite.Tick();

                if (moving && sprite is Weapon)
                    movementOffset = MathF.Sin((float)time.Elapsed.TotalMilliseconds * 0.01f) * 15f;

                float spriteWidth = sprite.Size.Width;
                float spriteHeight = sprite.Size.Height;

                if (sprite.ScalingMode == SpriteScalingMode.ScaleToScreen)
                {
                    float scalingFactor = (float)sprite.TargetScaleWidth * graphicsScale / sprite.Size.Width;
                    spriteWidth = sprite.TargetScaleWidth * graphicsScale;
                    spriteHeight = sprite.Size.Height * scalingFactor;
                }

                renderer.DrawScreenSprite(sprite.Img, sprite.Position.X + gameWindowXOffset, sprite.Position.Y + gameWindowYOffset + movementOffset, spriteWidth, spriteHeight);
            }

            if (flash > 0)
            {
                renderer.FillScreen(Color.FromArgb((int)(flashColor.A * flash) & 254, flashColor));
            }

            flash -= 1f / targetFPS;
        }
        static Vector2 RotateVector(Vector2 v, float radians)
        {
            float ca = MathF.Cos(radians);
            float sa = MathF.Sin(radians);
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }
        static void DrawWallSlice(int lineStart, int lineEnd, int lineHeight, float shade, int x, int textureX, Texture texture, bool wallHasDecal, Decal decal, int alpha)
        {
            for (int y = lineStart; y < lineEnd && y < gameWindowHeight; y++)
            {
                int d = y * 2 - gameWindowHeight + lineHeight;
                int textureY = d * 64 / lineHeight / 2;
                textureY &= 63;

                Color wallColor = texture[textureX, textureY];
                wallColor = Color.FromArgb(alpha, wallColor);

                if (wallHasDecal && decal.Texture[textureX][textureY].A > 128)
                {
                    wallColor = decal.Texture[textureX][textureY];
                }

                wallColor = Color.FromArgb(wallColor.A, (int)(wallColor.R * shade), (int)(wallColor.G * shade), (int)(wallColor.B * shade));

                if (alpha < 255)
                {
                    renderer.ModifyBufferPixel(x, y, wallColor);
                    continue;
                }

                renderer.SetBufferPixel(x, y, wallColor);
            }
        }
        static bool WallTilePresent(int x, int y)
        {
            if (x < 0 || x > Map.Width - 1 || y < 0 || y > Map.Height - 1)
                return false;

            return Map.FloorsAndWalls[x, y].TextureType > 0;
        }
    }
}
