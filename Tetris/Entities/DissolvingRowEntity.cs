using System;
using Microsoft.Xna.Framework;
using Otaku;
using Otaku.Rendering;
using Tetris.Components;

namespace Tetris.Entities
{
    public class DissolvingRowEntity : IEntity
    {
        public bool Alive { get; set; } = true;
        public string Name { get; set; } = "DissolvingRow";
        public IScene Scene { get; set; }
        public Transform Transform { get; }

        private readonly DissolvingBoardRowRenderer _dissolvingBoardRowRenderer;
        private readonly BoardRow _boardRow;
        private readonly Animation _disolveAnimation;

        private BoardEntity _board;

        public DissolvingRowEntity(DissolvingBoardRowRenderer dissolvingBoardRowRenderer, BoardRow boardRow)
        {
            Alive = true;
            Transform = new Transform();
            _dissolvingBoardRowRenderer = dissolvingBoardRowRenderer;
            _boardRow = boardRow;
            _disolveAnimation = new Animation(TimeSpan.FromMilliseconds(300));
            _disolveAnimation.Start();
        }

        public void Start()
        {
            _board = Scene.FindEntity<BoardEntity>(BoardEntity.Id);

            if (_board == null)
                throw new Exception("Board entity needed by DissolingRowEntities, not found!");
        }

        public void Update(GameTime gameTime)
        {
            _disolveAnimation.Update(gameTime);

            if (_disolveAnimation.IsFinished)
            {
                Alive = false;
            }
        }

        public void Render(RenderingContext renderingContext)
        {
            _dissolvingBoardRowRenderer.Render(renderingContext, _board.Transform, _boardRow, _disolveAnimation.Step);
        }
    }
}