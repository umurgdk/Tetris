using Microsoft.Xna.Framework;
using Otaku.Rendering;

namespace Tetris.Components
{
    public class PieceRenderer
    {
        private readonly BlockRenderer _blockRenderer;

        public PieceRenderer(BlockRenderer blockRenderer)
        {
            _blockRenderer = blockRenderer;
        }

        public void Render(RenderingContext context, Vector2 worldPosition, Piece piece, Vector2 quadSize)
        {
            for (var row = 0; row < piece.Height; row++)
            {
                for (var col = 0; col < piece.Width; col++)
                {
                    if (piece[row, col].IsEmpty) continue;

                    var blockPosition = worldPosition + quadSize * new Vector2(col, row);
                    _blockRenderer.Render(context, piece[row, col], blockPosition, quadSize);
                }
            }
        }

        public void Render(RenderingContext context, Vector2 worldPosition, Piece piece)
        {
            Render(context, worldPosition, piece, _blockRenderer.QuadSize);
        }
    }
}