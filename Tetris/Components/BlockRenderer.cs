using Microsoft.Xna.Framework;
using Otaku.Rendering;

namespace Tetris.Components
{
    public class BlockRenderer
    {
        public Vector2 QuadSize { get; set; }

        public BlockRenderer(Vector2 quadSize)
        {
            QuadSize = quadSize;
        }

        public void Render(RenderingContext context, Block block, Vector2 position, Vector2 quadSize)
        {
            context.QuadBatchRenderer.AddQuad(position, quadSize, block.Color);
        }

        public void Render(RenderingContext context, Block block, Vector2 position)
        {
            Render(context, block, position, QuadSize);
        }
    }
}