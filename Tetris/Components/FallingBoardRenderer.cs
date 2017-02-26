using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Otaku;
using Otaku.Rendering;

namespace Tetris.Components
{
    public class FallingBoardRenderer
    {
        private readonly BoardRenderer _boardRenderer;
        private readonly Dictionary<int, FallingBoardRow> _fallingBoardRows;

        public FallingBoardRenderer(BoardRenderer boardRenderer, List<FallingBoardRow> fallingBoardRows)
        {
            _boardRenderer = boardRenderer;

            _fallingBoardRows = new Dictionary<int, FallingBoardRow>();
            fallingBoardRows.ForEach(row => _fallingBoardRows[row.Index] = row);
        }

        public void Render(RenderingContext context, Board board, Transform transform, float offsetStep)
        {
            var backgroundSize = new Vector2(board.Columns, board.Rows) * _boardRenderer.BlockQuadSize;
            context.QuadBatchRenderer.AddQuad(new Vector2(transform.Position.X, transform.Position.Y), backgroundSize, new Color(Color.Black, 0.5f));


            for (var row = 0; row < board.Rows; row++)
            {
                var rowOffset = Vector2.Zero;

                if (_fallingBoardRows.ContainsKey(row))
                {
                    var fallingRow = _fallingBoardRows[row];
                    var offsetY = (_boardRenderer.BlockQuadSize.Y * fallingRow.FallBy) * offsetStep;
                    rowOffset = new Vector2(0, offsetY);
                }

                _boardRenderer.RenderRow(context, board, transform, row, rowOffset);
            }
        }
    }
}