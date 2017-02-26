using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Otaku.Rendering
{
    public class RenderingContext
    {
        private GraphicsDevice _device;

        public QuadBatchRenderer QuadBatchRenderer { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public RenderingContext(GraphicsDevice device)
        {
            _device = device;

            QuadBatchRenderer = new QuadBatchRenderer(device);
            SpriteBatch = new SpriteBatch(device);
        }
    }
}