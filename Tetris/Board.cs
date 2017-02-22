using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class Board
    {
        public static readonly Vector2 BlockSize = new Vector2(30.0f);

        public Transform Transform { get; } = new Transform();

        public Interval FallingPieceInterval;

        private Block[,] _blocks;
        private Piece _fallingPiece;

        public int Columns => _blocks.GetLength(1);
        public int Rows => _blocks.GetLength(0);

        private bool _isGameOver = false;
        private readonly Action _onGameOver;

        public bool IsWaitingForFallingPiece => _fallingPiece == null;

        public Board(Vector2 position, Point boardSize, Action onGameOver)
        {
            Transform.Position = new Vector3(position, 0);

            _blocks = new Block[boardSize.Y, boardSize.X];

            _onGameOver = onGameOver;

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    _blocks[row, col].IsEmpty = true;
                }
            }

            FallingPieceInterval = new Interval(TimeSpan.FromSeconds(0.5), MoveFallingPieceDown);
            FallingPieceInterval.Start();
        }

        public void Clear()
        {
            _blocks = new Block[Rows, Columns];
        }

        public void MoveFallingPieceDown()
        {
            _fallingPiece.Position.Y += 1;
        }

        public void MoveFallingPieceRight()
        {
            if (_fallingPiece.Right < Columns - 1)
            {
                var col = _fallingPiece.Cols - 1;
                for (var row = 0; row < _fallingPiece.Rows; row++)
                {
                    var blockPosition = _fallingPiece.Position + new Point(col, row);

                    if (blockPosition.X < 0 || blockPosition.Y < 0)
                    {
                        continue;
                    }

                    if (!_fallingPiece.Blocks[row, col].IsEmpty && !_blocks[blockPosition.Y, blockPosition.X + 1].IsEmpty)
                    {
                        return;
                    }
                }

                _fallingPiece.Position.X += 1;
            }
        }

        public void MoveFallingPieceLeft()
        {
            if (_fallingPiece.Position.X <= 0)
            {
                return;
            }

            for (var row = 0; row < _fallingPiece.Rows; row++)
            {
                var blockPosition = _fallingPiece.Position + new Point(0, row);

                if (blockPosition.X < 0 || blockPosition.Y < 0)
                {
                    continue;
                }

                if (!_fallingPiece.Blocks[row, 0].IsEmpty && !_blocks[blockPosition.Y, blockPosition.X - 1].IsEmpty)
                {
                    return;
                }
            }

            _fallingPiece.Position.X -= 1;
        }

        public void RotateFallingPiece()
        {
            _fallingPiece.Rotate();
        }

        public void Update(GameTime gameTime)
        {
            if (_fallingPiece == null || _isGameOver)
            {
                return;
            }

            FallingPieceInterval.Update(gameTime.ElapsedGameTime);

            CheckCollision();
        }

        private void CheckCollision()
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var shouldEmbed = false;

            // Check for ground collision
            if (_fallingPiece.Position.Y + _fallingPiece.Rows == Rows)
            {
                shouldEmbed = true;
            }

            // Check falling piece collision with other pieces
            // Starting from bottom of the piece
            for (int row = _fallingPiece.Rows - 1; (row >= 0) && !shouldEmbed; row--)
            {
                for (int col = 0; col < _fallingPiece.Cols; col++)
                {
                    var block = _fallingPiece[row, col];
                    var blockGridPosition = _fallingPiece.Position + new Point(col, row);

                    if (!block.IsEmpty && blockGridPosition.Y >= 0 && !_blocks[blockGridPosition.Y + 1, blockGridPosition.X].IsEmpty)
                    {
                        shouldEmbed = true;
                        break;
                    }
                }
            }

            // If falling piece collides and doesn't fit in the board, game over!
            if (shouldEmbed && _fallingPiece.Position.Y <= 0 && !_isGameOver)
            {
                _isGameOver = true;
                _onGameOver();
            }

            // If there is any collision embed piece in board blocks
            if (!_isGameOver && shouldEmbed)
            {
                EmbedFallingPiece();
                _fallingPiece = null;
                FallingPieceInterval.Stop();
            }
        }

        public void EmbedFallingPiece()
        {
            for (var row = 0; row < _fallingPiece.Rows; row++)
            {
                for (var col = 0; col < _fallingPiece.Cols; col++)
                {
                    var block = _fallingPiece[row, col];

                    if (!block.IsEmpty)
                    {
                        var blockPosition = _fallingPiece.Position + new Point(col, row);
                        _blocks[blockPosition.Y, blockPosition.X] = block;
                    }
                }
            }
        }

        public void SetFallingPiece(Piece piece)
        {
            _fallingPiece = piece;
            FallingPieceInterval.Start();

            piece.Position.Y = -piece.Rows;
        }

        public Vector2 BoardToLocalPosition(int row, int col)
        {
            return new Vector2(col, row) * BlockSize;
        }

        public Vector3 BoardToWorldPosition(int row, int col)
        {
            return Vector3.Transform(new Vector3(BoardToLocalPosition(row, col), 0), Transform.Matrix);
        }

        public void Render(GraphicsDevice device, QuadBatchRenderer batch, Matrix view, Matrix projection)
        {
            // Draw background
            batch.AddQuad(Transform, Vector2.Zero, new Vector2(Columns, Rows) * BlockSize, new Color(0f, 0f, 0f, 0.4f));

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    if (!_blocks[row,col].IsEmpty)
                    {
                        batch.AddQuad(Transform, BoardToLocalPosition(row, col), BlockSize, _blocks[row, col].Color);
                    }
                }
            }

            if (_fallingPiece == null)
            {
                return;
            }
            
            for (var row = 0; row < _fallingPiece.Rows; row++)
            {
                for (var col = 0; col < _fallingPiece.Cols; col++)
                {
                    var block = _fallingPiece[row, col];
                    var piecePosition = BoardToLocalPosition(_fallingPiece.Position.Y, _fallingPiece.Position.X);
                    var blockPosition = piecePosition + BoardToLocalPosition(row, col);

                    if (!block.IsEmpty && blockPosition.X >= 0 && blockPosition.Y >= 0)
                    {
                        batch.AddQuad(Transform, blockPosition, BlockSize, block.Color);
                    }
                }
            }
        }
    }
}