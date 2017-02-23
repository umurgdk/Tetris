using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using Microsoft.Xna.Framework;
using Tetris;

namespace Tetris
{
    public enum BoardMoveDirection
    {
        Down,
        Left,
        Right
    }

    public class Board
    {
        public Transform Transform { get; } = new Transform();

        private Block[,] _blocks;
        private Vector2 _blockSize;

        public int Columns => _blocks.GetLength(1);
        public int Rows => _blocks.GetLength(0);

        public Board(Point boardSize, Vector2 blockSize)
        {
            _blockSize = blockSize;
            _blocks = new Block[boardSize.Y, boardSize.X];

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    _blocks[row, col].IsEmpty = true;
                }
            }
        }

        public bool CanPieceContinueFalling(Piece piece)
        {
            if (piece.Bottom == Rows - 1)
            {
                return false;
            }

            for (var row = piece.Rows - 1; row >= 0; row--)
            {
                for (var col = 0; col < piece.Cols; col++)
                {
                    var block = piece[row, col];
                    var blockGridPosition = piece.Position + new Point(col, row);

                    if (!block.IsEmpty && blockGridPosition.Y >= 0 && !_blocks[blockGridPosition.Y + 1, blockGridPosition.X].IsEmpty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanPieceMoveTo(Piece piece, BoardMoveDirection direction)
        {
            if (IsPieceNearBorder(piece, direction))
            {
                return false;
            }

            if (IsPieceNeighborOccupied(piece, direction))
            {
                return false;
            }

            return true;
        }

        private bool IsPieceNeighborOccupied(Piece piece, int xOffset, int yOffset)
        {
            for (var row = 0; row < piece.Rows; row++)
            {
                for (var col = 0; col < piece.Cols; col++)
                {
                    var block = piece.Blocks[row, col];

                    if (block.IsEmpty) continue;

                    var blockPosition = piece.Position + new Point(col, row);
                    var targetPosition = new Point(blockPosition.X + xOffset, blockPosition.Y + yOffset);

                    if (targetPosition.X < 0 || targetPosition.X >= Columns || targetPosition.Y >= Rows) continue;

                    if (!_blocks[targetPosition.Y, targetPosition.X].IsEmpty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPieceNeighborOccupied(Piece piece, BoardMoveDirection direction)
        {
            var xOffset = direction == BoardMoveDirection.Left
                ? -1
                : (direction == BoardMoveDirection.Right ? +1 : 0);

            var yOffset = direction == BoardMoveDirection.Down ? 1 : 0;

            return IsPieceNeighborOccupied(piece, xOffset, yOffset);
        }

        private bool IsPieceNearBorder(Piece piece, BoardMoveDirection direction)
        {
            switch (direction)
            {
                case BoardMoveDirection.Down:
                    return piece.Bottom == Rows - 1;

                case BoardMoveDirection.Left:
                    return piece.Position.X == 0;

                case BoardMoveDirection.Right:
                    return piece.Right == Columns - 1;

                default:
                    return false;
            }
        }

        private bool FindNextEmptyRow(int indexFromBottom, out int nextEmptyRow)
        {
            var nextIndex = indexFromBottom;

            for (var row = indexFromBottom; row >= 0; row--)
            {
                var allEmpty = true;

                for (var col = 0; col < Columns - 1; col++)
                {
                    if (!_blocks[row, col].IsEmpty)
                    {
                        allEmpty = false;
                        break;
                    }
                }

                if (allEmpty)
                {
                    nextEmptyRow = nextIndex;
                    return true;
                }

                nextIndex -= 1;
            }

            nextEmptyRow = -1;

            return false;
        }

        public void FallRows()
        {
            // There will be no empty rows more than 4 in tetris
            var rowsMoved = 0;
            var nextEmptyRowIndex = 0;

            while (rowsMoved < 4 && FindNextEmptyRow(Rows - 1, out nextEmptyRowIndex))
            {
                for (var row = nextEmptyRowIndex; row > 0; row--)
                {
                    for (var col = 0; col < Columns; col++)
                    {
                        _blocks[row, col] = _blocks[row - 1, col];
                        _blocks[row - 1, col] = new Block {IsEmpty = true};
                    }
                }

                rowsMoved += 1;
            }
        }

        public Point GetGroundPositionForPiece(Piece piece)
        {
            var targetPosition = piece.Position;
            var yOffset = 0;

            for (var targetBottom = piece.Bottom + 1; targetBottom < Rows; targetBottom++)
            {
                if (IsPieceNeighborOccupied(piece, 0, yOffset + 1))
                {
                    break;
                }

                yOffset += 1;
            }

            targetPosition.Y += yOffset;

            return targetPosition;
        }

        public void AddPieceIntoPlace(Piece piece)
        {
            for (var row = 0; row < piece.Rows; row++)
            {
                for (var col = 0; col < piece.Cols; col++)
                {
                    var block = piece[row, col];

                    if (!block.IsEmpty)
                    {
                        var blockPosition = piece.Position + new Point(col, row);
                        _blocks[blockPosition.Y, blockPosition.X] = block;
                    }
                }
            }
        }

        Vector2 BoardToLocalPosition(int row, int col)
        {
            return new Vector2(col, row) * _blockSize;
        }

        public Vector2 BoardToWorldPosition(int row, int col)
        {
            return Vector2.Transform(BoardToLocalPosition(row, col), Transform.Matrix);
        }

        public void Render(QuadBatchRenderer batch)
        {
            // Draw background
            batch.AddQuad(BoardToWorldPosition(0, 0), new Vector2(Columns, Rows) * _blockSize, new Color(0f, 0f, 0f, 0.4f));

            // Draw blocks
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    if (!_blocks[row,col].IsEmpty)
                    {
                        batch.AddQuad(BoardToWorldPosition(row, col), _blockSize, _blocks[row, col].Color);
                    }
                }
            }
        }

        public List<BlockRow> ExtractFullRows()
        {
            var fullRows = new List<int>();

            for (var row = Rows - 1; row >= 0; row--)
            {
                var isRowFull = true;
                for (var col = 0; col < Columns; col++)
                {
                    if (_blocks[row, col].IsEmpty)
                    {
                        isRowFull = false;
                        break;
                    }
                }

                if (isRowFull)
                {
                    fullRows.Add(row);
                }
            }

            if (fullRows.Count == 0)
            {
                return new List<BlockRow>();
            }

            var emptyBlock = new Block
            {
                IsEmpty = true
            };


            return fullRows.Select(row =>
            {
                var fullBlocks = new Block[Columns];

                for (int col = 0; col < Columns; col++)
                {
                    fullBlocks[col] = _blocks[row, col];
                    _blocks[row, col] = emptyBlock;
                }

                return new BlockRow(fullBlocks, BoardToWorldPosition(row, 0));
            }).ToList();
        }
    }
}