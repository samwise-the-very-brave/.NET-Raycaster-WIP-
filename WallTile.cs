namespace DDA
{
    class WallTile : MapTile
    {
        public int South { get; init; }
        public int North { get; init; }
        public int East { get; init; }
        public int West { get; init; }

        public WallTile(int s, int n, int e, int w) : base(null)
        {
            South = s;
            North = n;
            East = e;
            West = w;
        }
    }
}
