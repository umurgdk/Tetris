using System;
using Microsoft.Xna.Framework;
using Otaku;
using Otaku.Rendering;
using Tetris.Components;

namespace Tetris.Entities
{
    public class PieceEntity : IEntity
    {
        public bool Alive { get; set; }
        public string Name { get; set; }
        public IScene Scene { get; set; }
        public Transform Transform { get; }

        public Piece Piece { get; set; }
        public Point Position { get; private set; }

        private readonly PieceRenderer _pieceRenderer;
        private readonly BoardRenderer _boardRenderer;

        private BoardEntity _boardEntity;

        public PieceEntity(PieceRenderer pieceRenderer, BoardRenderer boardRenderer, Piece piece)
        {
            Alive = true;
            Name = "Piece";
            Transform = new Transform();
            Position = Point.Zero;
            Piece = piece;

            _pieceRenderer = pieceRenderer;
            _boardRenderer = boardRenderer;
        }

        public void Move(BoardMovementDirection direction)
        {
            Position += BoardMovement.DirectionToOffset(direction);
        }

        public void Start()
        {
            _boardEntity = Scene.FindEntity<BoardEntity>(BoardEntity.Id);

            if (_boardEntity == null)
            {
                throw new Exception("Board entity needed by piece entity, which is not found!");
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Render(RenderingContext renderingContext)
        {
            var worldPosition = _boardRenderer.ToWorldPosition(_boardEntity.Transform, Position, Vector2.Zero);
            _pieceRenderer.Render(renderingContext, worldPosition, Piece);
        }

        public void Rotate()
        {
            Piece.Rotate();
        }
    }
}