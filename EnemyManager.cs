using System.Collections.Generic;

namespace DDA
{
    static class EnemyManager
    {
        public static IReadOnlyList<Enemy> Enemies { get => enemies.AsReadOnly(); }

        static List<Enemy> enemies = new List<Enemy>();

        public static void Add(Enemy enemy)
        {
            enemies.Add(enemy);
        }
    }
}
