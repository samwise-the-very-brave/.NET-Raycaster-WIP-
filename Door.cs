using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDA
{
    class Door : MapTile, IInteractable
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Door(int? texType, int x, int y) : base(texType)
        {
            X = x;
            Y = y;
        }

        public void Interact()
        {
            Main.Map.ReplaceWallTile(X, Y, 0);
        }
    }
}
