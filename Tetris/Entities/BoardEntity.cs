using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Otaku;
using Otaku.Rendering;
using Tetris.Components;

namespace Tetris.Entities
{
    public class BoardEntity : IEntity
    {
        public const string Id = "Board";

        public bool Alive { get; set; }
        public string Name { get; set; }
        public IScene Scene { get; set; }
        public Transform Transform { get; private set; }
        public bool IsFalling { get; private set; }
        public readonly BoardMovement Movement;

        private readonly Board _board;
        private readonly BoardRenderer _boardRenderer;

        private FallingBoardRenderer _fallingBoardRenderer;
        private Animation _fallingAnimation;

        public BoardEntity(BoardRenderer boardRenderer)
        {
            Alive = true;
            Name = Id;
            IsFalling = false;
            Transform = new Transform();

            _board = new Board(new Size(10, 20));
            _boardRenderer = boardRenderer;

            Movement = new BoardMovement(_board);
        }

        public void Start()
        {

        }

        public void Update(GameTime gameTime)
        {
            _fallingAnimation?.Update(gameTime);

            if (_fallingAnimation?.IsFinished == true)
            {
                IsFalling = false;
                _fallingAnimation = null;
                _fallingBoardRenderer = null;
                _board.MoveFallingRows(_board.GetFallingRows());
            }
        }

        public void EmbedPiece(PieceEntity pieceEntity)
        {
            _board.EmbedPiece(pieceEntity.Piece, pieceEntity.Position);
        }

        public void EmbedPieceAtGround(PieceEntity pieceEntity)
        {
            var groundPosition = Movement.GetGroundPosition(pieceEntity);
            _board.EmbedPiece(pieceEntity.Piece, groundPosition);
        }

        public List<BoardRow> ExtractFullRows()
        {
            return _board.ExtractFullRows();
        }

        public void DropRowsDown()
        {
            var fallingRows = _board.GetFallingRows();
            if (fallingRows.Count > 0)
            {
                IsFalling = true;
                _fallingAnimation = new Animation(TimeSpan.FromMilliseconds(300));
                _fallingAnimation.Start();

                _fallingBoardRenderer = new FallingBoardRenderer(_boardRenderer, fallingRows);
            }
        }

        public void Render(RenderingContext renderingContext)
        {
            if (_fallingAnimation != null && _fallingBoardRenderer != null)
            {
                _fallingBoardRenderer.Render(renderingContext, _board, Transform, _fallingAnimation.Step);
            }

            else
            {
                _boardRenderer.Render(renderingContext, _board, Transform);
            }

        }
    }
}