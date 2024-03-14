using System.Drawing;
using System.Numerics;

namespace DDA
{
    struct RayHit
    {
        public float PerpDist { get; }
        public Vector2 Direction { get; }
        public Point HitPoint { get; }
        public int Side { get; }

        public RayHit(float perpDist, Vector2 dir, Point hitPoint, int side)
        {
            PerpDist = perpDist;
            Direction = dir;
            HitPoint = hitPoint;
            Side = side;
        }
    }
}
