namespace Tetris.Components
{
    public struct BoardRow
    {
        public readonly int Row;
        public readonly Block[] Blocks;

        public BoardRow(int row, Block[] blocks)
        {
            Row = row;
            Blocks = blocks;
        }
    }
}