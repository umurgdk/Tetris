using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Tetris.Components
{
    public class Board
    {
        private Block[,] _blocks;
        public int Rows => _blocks.GetLength(0);
        public int Columns => _blocks.GetLength(1);

        public Block this[int row, int col] => _blocks[row, col];

        public Board(Size boardSize)
        {
            _blocks = new Block[boardSize.Height, boardSize.Width];

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    _blocks[row, col] = Block.Empty;
                }
            }
        }

        public void EmbedPiece(Piece piece, Point position)
        {
            if (position.X + piece.Width - 1 >= Columns ||
                position.Y + piece.Height - 1 >= Rows ||
                position.X < 0 ||
                position.Y < 0)
            {
                return;
            }

            for (var row = 0; row < piece.Height; row++)
            {
                for (var col = 0; col < piece.Width; col++)
                {
                    if (!piece[row, col].IsEmpty)
                    {
                        _blocks[position.Y + row, position.X + col] = piece[row, col];
                    }
                }
            }
        }

        public Point GetGroundPosition(Piece piece, Point piecePosition)
        {
            var bottom = piecePosition.Y + piece.Height - 1;
            var targetPosition = piecePosition;

            for (var targetBottom = bottom + 1; targetBottom < Rows; targetBottom++)
            {
                if (IsPieceColliding(piece, targetPosition + new Point(0, 1)))
                {
                    break;
                }

                targetPosition.Y += 1;
            }

            return targetPosition;
        }

        public bool IsPieceColliding(Piece piece, Point piecePosition)
        {
            var x = piecePosition.X;
            var y = piecePosition.Y;

            for (var row = 0; row < piece.Height; row++)
            {
                for (var col = 0; col < piece.Width; col++)
                {
                    if (piece[row, col].IsEmpty)
                    {
                        continue;
                    }

                    if (y + row >= Rows)
                    {
                        return true;
                    }

                    if (y + row < Rows && x + col < Columns && y >= 0 && x >= 0)
                    {
                        if (!_blocks[y + row, x + col].IsEmpty)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsPieceInBoard(Piece piece, Point piecePosition)
        {
            return piecePosition.X >= 0 && piecePosition.Y < Rows && piecePosition.X + piece.Width - 1 < Columns;
        }

        public bool IsRowEmpty(int row)
        {
            for (var col = 0; col < Columns; col++)
            {
                if (!_blocks[row, col].IsEmpty) return false;
            }

            return true;
        }

        public bool IsRowFull(int row)
        {
            for (var col = 0; col < Columns; col++)
            {
                if (_blocks[row, col].IsEmpty) return false;
            }

            return true;
        }

        public List<BoardRow> ExtractFullRows()
        {
            var fullRows = new List<int>();

            for (var row = Rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    fullRows.Add(row);
                }
            }

            if (fullRows.Count == 0)
            {
                return new List<BoardRow>();
            }

            return fullRows.Select(row =>
            {
                var fullBlocks = new Block[Columns];

                for (var col = 0; col < Columns; col++)
                {
                    fullBlocks[col] = _blocks[row, col];
                    _blocks[row, col] = Block.Empty;
                }

                return new BoardRow(row, fullBlocks);
            }).ToList();
        }

        public bool FindEmptyRow(out int index)
        {
            for (var row = Rows - 1; row >= 0; row--)
            {
                if (IsRowEmpty(row))
                {
                    index = row;
                    return true;
                }
            }

            index = 0;
            return false;
        }

        public List<FallingBoardRow> GetFallingRows()
        {
            // There will be no empty rows more than 4 in tetris
            var emptyRowIndex = 0;
            var fallingRows = new List<FallingBoardRow>();

            if (!FindEmptyRow(out emptyRowIndex))
            {
                return fallingRows;
            }

            var fallBy = 0;
            for (var row = emptyRowIndex; row > 0; row--)
            {
                if (IsRowEmpty(row))
                {
                    fallBy += 1;
                    continue;
                }

                fallingRows.Add(new FallingBoardRow(row, fallBy));
            }

            return fallingRows;
        }

        public void MoveFallingRows(List<FallingBoardRow> rows)
        {
            rows.ForEach(row =>
            {
                for (var col = 0; col < Columns; col++)
                {
                    _blocks[row.Index + row.FallBy, col] = _blocks[row.Index, col];
                    _blocks[row.Index, col].IsEmpty = true;
                }
            });
        }
    }
}