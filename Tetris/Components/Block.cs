using Microsoft.Xna.Framework;

namespace Tetris.Components
{
    public struct Block
    {
        public bool IsEmpty;
        public Color Color;

        public static Block Empty = new Block { IsEmpty = true };
    }
}