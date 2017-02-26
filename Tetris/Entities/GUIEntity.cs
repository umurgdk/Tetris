using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Otaku;
using Otaku.Rendering;
using Tetris.Components;

namespace Tetris.Entities
{
    public class GUIEntity : IEntity
    {
        public bool Alive { get; set; }
        public string Name { get; set; }
        public IScene Scene { get; set; }
        public Transform Transform { get; }

        public int Score { get; set; }
        public Piece NextPiece { get; set; }

        private readonly SpriteFont _font;
        private readonly PieceRenderer _pieceRenderer;

        public GUIEntity(SpriteFont font, PieceRenderer pieceRenderer)
        {
            Alive = true;
            Name = "GUI";
            Transform = new Transform();
            Score = 0;

            _font = font;
            _pieceRenderer = pieceRenderer;
        }

        public void Start()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Render(RenderingContext renderingContext)
        {
            var spriteBatch = renderingContext.SpriteBatch;

            spriteBatch.DrawString(_font, $"Score: {Score}", new Vector2(400, 100), Color.White);
            spriteBatch.DrawString(_font, $"Next Piece:", new Vector2(400, 130), Color.White);
            _pieceRenderer.Render(renderingContext, new Vector2(400, 200), NextPiece, new Vector2(20));
        }
    }
}