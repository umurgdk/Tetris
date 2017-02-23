using Microsoft.Xna.Framework;

namespace Tetris
{
    public struct BlockRow
    {
        public readonly Block[] Blocks;
        public readonly Vector2 WorldPosition;

        public BlockRow(Block[] blocks, Vector2 worldPosition)
        {
            Blocks = blocks;
            WorldPosition = worldPosition;
        }
    }
}