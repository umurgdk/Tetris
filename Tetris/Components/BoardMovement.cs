using System;
using Microsoft.Xna.Framework;
using Tetris.Entities;

namespace Tetris.Components
{
    public enum BoardMovementDirection
    {
        Down,
        Right,
        Left
    }

    public class BoardMovement
    {
        private readonly Board _board;

        public BoardMovement(Board board)
        {
            _board = board;
        }

        public static Point DirectionToOffset(BoardMovementDirection direction)
        {
            switch (direction)
            {
                case BoardMovementDirection.Down:
                    return new Point(0, 1);

                case BoardMovementDirection.Left:
                    return new Point(-1 ,0);

                case BoardMovementDirection.Right:
                    return new Point(1, 0);

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public bool CanPieceMove(PieceEntity piece, BoardMovementDirection direction)
        {
            var targetPosition = piece.Position + DirectionToOffset(direction);
            
            return _board.IsPieceInBoard(piece.Piece, targetPosition) &&
                   !_board.IsPieceColliding(piece.Piece, targetPosition);
        }

        public Point GetGroundPosition(PieceEntity piece)
        {
            return _board.GetGroundPosition(piece.Piece, piece.Position);
        }
    }
}