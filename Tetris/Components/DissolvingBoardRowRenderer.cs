using Microsoft.Xna.Framework;
using Otaku;
using Otaku.Rendering;

namespace Tetris.Components
{
    public class DissolvingBoardRowRenderer
    {
        private readonly BoardRenderer _boardRenderer;
        private readonly BlockRenderer _blockRenderer;

        public DissolvingBoardRowRenderer(BoardRenderer boardRenderer, BlockRenderer blockRenderer)
        {
            _boardRenderer = boardRenderer;
            _blockRenderer = blockRenderer;
        }

        public void Render(RenderingContext context, Transform boardTransform, BoardRow boardRow, float animationStep)
        {
            var row = boardRow.Row;
            for (var col = 0; col < boardRow.Blocks.Length; col++)
            {
                var blockBoardPosition = new Point(col, row);
                var blockPosition = _boardRenderer.ToWorldPosition(boardTransform, blockBoardPosition, Vector2.Zero);
                var shrinkedSize = _blockRenderer.QuadSize * (1.0f - animationStep);
                var diff = _blockRenderer.QuadSize - shrinkedSize;
                var centeredPosition = blockPosition + diff / 2.0f;

                _blockRenderer.Render(context, boardRow.Blocks[col], centeredPosition, shrinkedSize);
            }
        }
    }
}