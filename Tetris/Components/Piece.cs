using Microsoft.Xna.Framework;

namespace Tetris.Components
{
    public class Piece
    {
        private Block[,] _blocks;

        public int Width => _blocks.GetLength(1);
        public int Height => _blocks.GetLength(0);

        public Block this[int row, int col] => _blocks[row, col];

        private Piece(Block[,] blocks)
        {
            _blocks = blocks;
        }

        public static Piece FromTemplate(string[] templateRows, Color color)
        {
            var rows = templateRows.Length;
            var columns = templateRows[0].ToCharArray().Length;

            var blocks = new Block[rows, columns];

            for (var row = 0; row < rows; row++)
            {
                var rowCharArray = templateRows[row].ToCharArray();

                for (var col = 0; col < columns; col++)
                {
                    var isEmpty = rowCharArray[col] == '-';

                    blocks[row, col] = new Block
                    {
                        Color = color,
                        IsEmpty = isEmpty
                    };
                }
            }

            return new Piece(blocks);
        }

        public void Rotate()
        {
            var newRows = Width;
            var newCols = Height;

            var newBlocks = new Block[newRows, newCols];

            for (var row = Height - 1; row >= 0; row--)
            {
                for (var col = 0; col < Width; col++)
                {
                    newBlocks[col, Height - 1 - row] = _blocks[row, col];
                }
            }

            _blocks = newBlocks;
        }
    }
}