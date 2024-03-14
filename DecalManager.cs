using System.Collections.Generic;

namespace DDA
{
    static class DecalManager
    {
        public static IReadOnlyCollection<Decal> Decals { get => decals; }

        static List<Decal> decals = new List<Decal>();

        public static void Add(Decal decal)
        {
            decals.Add(decal);
        }
    }
}
