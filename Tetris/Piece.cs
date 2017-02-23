using System.Collections.Specialized;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class Piece
    {
        public Point Position;
        public Block[,] Blocks;
        public int Rows => Blocks.GetLength(0);
        public int Cols => Blocks.GetLength(1);
        public int Bottom => Position.Y + Rows - 1;
        public int Right => Position.X + Cols - 1;

        public Block this[int row, int col]
        {
            get { return Blocks[row, col]; }
            set { Blocks[row, col] = value; }
        }

        private Piece(int rows, int cols)
        {
            Blocks = new Block[rows, cols];
        }

        public static Piece FromTemplate(string[] template, Color color)
        {
            var piece = new Piece(template.Length, template[0].Length);

            for (var row = 0; row < template.Length; row++)
            {
                var charArray = template[row].ToCharArray();

                for (var col = 0; col < charArray.Length; col++)
                {
                    var character = charArray[col];
                    var isEmpty = character == '-';

                    piece[row, col] = new Block
                    {
                        IsEmpty = isEmpty,
                        Color = color
                    };
                }
            }

            return piece;
        }

        public void Rotate()
        {
            var newRows = Cols;
            var newCols = Rows;

            var newBlocks = new Block[newRows, newCols];
            
            for (var oldRow = Rows -1; oldRow >= 0; oldRow--)
            {
                for (var oldCol = 0; oldCol < Cols; oldCol++)
                {
                    newBlocks[oldCol, (Rows - 1) - oldRow] = Blocks[oldRow, oldCol];
                }
            }

            Blocks = newBlocks;
        }

        public void Render(QuadBatchRenderer renderer, Vector2 position, Vector2 blockSize)
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    if (Blocks[row, col].IsEmpty)
                    {
                        continue;
                    }

                    var blockPosition = new Vector2(col, row) * blockSize + position;
                    renderer.AddQuad(blockPosition, blockSize, Blocks[row, col].Color);
                }
            }
        }
    }
}