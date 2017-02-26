using Microsoft.Xna.Framework;

namespace Tetris.Components
{
    public class PlayerController
    {
        private bool CanMove(Board board, Piece piece, Point piecePosition, Point offset)
        {
            return !board.IsPieceColliding(piece, piecePosition + offset);
        }

        public bool CanMoveRight(Board board, Piece piece, Point piecePosition)
        {
            return CanMove(board, piece, piecePosition, new Point(1, 0));
        }

        public bool CanMoveLeft(Board board, Piece piece, Point piecePosition)
        {
            return CanMove(board, piece, piecePosition, new Point(-1, 0));
        }

        public bool CanMoveDown(Board board, Piece piece, Point piecePosition)
        {
            return CanMove(board, piece, piecePosition, new Point(0, 1));
        }
    }
}