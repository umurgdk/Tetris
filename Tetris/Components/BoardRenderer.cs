using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Otaku;
using Otaku.Rendering;

namespace Tetris.Components
{
    public class BoardRenderer
    {
        private readonly BlockRenderer _blockRenderer;
        public Vector2 BlockQuadSize => _blockRenderer.QuadSize;

        public BoardRenderer(BlockRenderer blockRenderer)
        {
            _blockRenderer = blockRenderer;
        }

        Vector2 BoardToLocalPosition(Point positionInBoard)
        {
            return positionInBoard.ToVector2() * _blockRenderer.QuadSize;
        }

        public Vector2 ToWorldPosition(Transform transform, Point positionInBoard, Vector2 offset)
        {
            return Vector2.Transform(BoardToLocalPosition(positionInBoard) + offset, transform.Matrix);
        }

        public void RenderRow(RenderingContext context, Board board, Transform transform, int row, Vector2 offset)
        {
            for (var col = 0; col < board.Columns; col++)
            {
                if (!board[row, col].IsEmpty)
                {
                    var blockPosition = ToWorldPosition(transform, new Point(col, row), offset);
                    _blockRenderer.Render(context, board[row, col], blockPosition);
                }
            }
        }

        public void Render(RenderingContext context, Board board, Transform transform)
        {
            var backgroundSize = new Vector2(board.Columns, board.Rows) * BlockQuadSize;
            context.QuadBatchRenderer.AddQuad(new Vector2(transform.Position.X, transform.Position.Y), backgroundSize, new Color(Color.Black, 0.5f));

            for (var row = 0; row < board.Rows; row++)
            {
                RenderRow(context, board, transform, row, Vector2.Zero);
            }
        }
    }
}