using System.IO;
using System.Numerics;

namespace DDA
{
    class Map
    {
        public string Name { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int CellSize { get; init; }
        public Vector2 PlayerStart { get; init; }
        public Vector2[] Exits { get; init; }
        public MapTile[,] FloorsAndWalls { get; init; }
        public MapTile[,] Ceilings { get; init; }

        public Map(string mapFilePath)
        {
            string[] mapData = File.ReadAllLines(mapFilePath);

            Name = mapData[0].Trim();
            Width = int.Parse(mapData[1]);
            Height = int.Parse(mapData[2]);
            CellSize = int.Parse(mapData[3]);

            FloorsAndWalls = new MapTile[Width, Height];
            Ceilings = new MapTile[Width, Height];

            string[] fileNames = mapData[4].Split(",");

            for (int i = 1; i < fileNames.Length + 1; i++)
            {
                TextureManager.IncludeTexture(@$"..\..\..\textures\{fileNames[i - 1]}", i);
            }

            string[] map = mapData[6].Trim().Split(",");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int type = int.Parse(map[y * Width + x]);

                    if (type == 11)
                    {
                        FloorsAndWalls[x, y] = new Door(type, x, y);
                    }
                    else
                    {
                        FloorsAndWalls[x, y] = new MapTile(type);
                    }
                }
            }

            map = mapData[7].Trim().Split(",");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Ceilings[x, y] = new MapTile(int.Parse(map[y * Width + x]));
                }
            }

            float playerX = float.Parse(mapData[8]);
            float playerY = float.Parse(mapData[9]);

            PlayerStart = new Vector2(playerX, playerY);
        }

        public Door GetFirstDoorInRadius(Vector2 origin, float radius)
        {
            for (int y = (int)(origin.Y - radius); y < (int)(origin.Y + radius) && y < FloorsAndWalls.GetUpperBound(1); y++)
            {
                for (int x = (int)(origin.X - radius); x < (int)(origin.X + radius) && x < FloorsAndWalls.GetUpperBound(0); x++)
                {
                    if (FloorsAndWalls[x, y] is Door && MathHelper.Distance(origin.X, origin.Y, x, y) <= radius)
                    {
                        return FloorsAndWalls[x, y] as Door;
                    }
                }
            }

            return null;
        }

        public void ReplaceWallTile(int x, int y, int newTileType)
        {
            FloorsAndWalls[x, y] = new MapTile(newTileType);
        }
    }
}
