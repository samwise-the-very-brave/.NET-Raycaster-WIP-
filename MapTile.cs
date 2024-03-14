namespace DDA
{
    class MapTile
    {
        public int? TextureType { get; private set; }

        public MapTile(int? texType)
        {
            TextureType = texType;
        }
    }
}
