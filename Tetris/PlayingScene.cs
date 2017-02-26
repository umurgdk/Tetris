using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Otaku;
using Otaku.Rendering;
using Tetris.Components;
using Tetris.Entities;
using Tetris.Helpers;

namespace Tetris
{
    public class PlayingScene : IScene
    {
        public SceneState State { get; set; }
        public Color BackgroundColor { get; } = Color.Blue;
        public List<IEntity> Entities { get; private set; }

        private readonly BoardRenderer _boardRenderer;
        private readonly PieceRenderer _pieceRenderer;
        private readonly DissolvingBoardRowRenderer _dissolvingBoardRowRenderer;

        private readonly Interval _fallInterval;
        private readonly Timer _createFallingPieceTimer;

        private BoardEntity _board;
        private PieceEntity _piece;
        private GUIEntity _gui;

        private int _score;
        private Piece _nextPiece;

        public PlayingScene()
        {
            Entities = new List<IEntity>();

            var blockRenderer = new BlockRenderer(new Vector2(30));
            _boardRenderer = new BoardRenderer(blockRenderer);
            _pieceRenderer = new PieceRenderer(blockRenderer);
            _dissolvingBoardRowRenderer = new DissolvingBoardRowRenderer(_boardRenderer, blockRenderer);

            _fallInterval = new Interval(TimeSpan.FromMilliseconds(300), FallPiece);
            _createFallingPieceTimer = new Timer(TimeSpan.FromMilliseconds(300), CreateFallingPiece);
        }

        public void LoadContent(ContentManager contentManager)
        {
            _gui = new GUIEntity(contentManager.Load<SpriteFont>("Content/font"), _pieceRenderer)
            {
                NextPiece = _nextPiece,
                Score = _score
            };
            AddEntity(_gui);
        }

        public void Enter()
        {
            _board = new BoardEntity(_boardRenderer);
            AddEntity(_board);

            _piece = new PieceEntity(_pieceRenderer, _boardRenderer, PieceTemplate.GetRandomPiece());
            AddEntity(_piece);

            _nextPiece = PieceTemplate.GetRandomPiece();

            _fallInterval.Start();

            InputManager.Instance.BindAction(Keys.Left,  MovePieceLeft);
            InputManager.Instance.BindAction(Keys.Right, MovePieceRight);
            InputManager.Instance.BindAction(Keys.Down,  MovePieceGround);
            InputManager.Instance.BindAction(Keys.Space, RotatePiece);
        }

        public void Exit()
        {

        }

        public void AddEntity(IEntity entity)
        {
            entity.Scene = this;
            Entities.Add(entity);

            if (State == SceneState.Play)
            {
                entity.Start();
            }
        }

        public void CreateEntity(string name)
        {

        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public TEntity FindEntity<TEntity>(string name) where TEntity : IEntity
        {
            return (TEntity)Entities.FirstOrDefault(entity => entity.Name == name);
        }

        private void FallPiece()
        {
            if (_board.Movement.CanPieceMove(_piece, BoardMovementDirection.Down))
            {
                _piece.Move(BoardMovementDirection.Down);
            }

            else
            {
                PlacePieceInBoard(false);
            }
        }

        private void PlacePieceInBoard(bool toGround)
        {
            if (toGround)
            {
                _board.EmbedPieceAtGround(_piece);
            }

            else
            {
                _board.EmbedPiece(_piece);
            }

            RemoveEntity(_piece);
            _piece = null;

            _createFallingPieceTimer.Restart();
            _fallInterval.Stop();

            _board.ExtractFullRows().ForEach(row =>
            {
                var dissolvingRow = new DissolvingRowEntity(_dissolvingBoardRowRenderer, row);
                AddEntity(dissolvingRow);
                _score += 5;
            });

            _board.DropRowsDown();
            _score += 1;
        }

        private void MovePieceLeft()
        {
            if (_piece != null && _board.Movement.CanPieceMove(_piece, BoardMovementDirection.Left))
            {
                _piece.Move(BoardMovementDirection.Left);
            }
        }

        private void MovePieceRight()
        {
            if (_piece != null && _board.Movement.CanPieceMove(_piece, BoardMovementDirection.Right))
            {
                _piece.Move(BoardMovementDirection.Right);
            }
        }

        private void MovePieceGround()
        {
            if (_piece == null) return;
            PlacePieceInBoard(true);
        }

        private void RotatePiece()
        {
            _piece?.Rotate();
        }

        private void CreateFallingPiece()
        {
            _piece = new PieceEntity(_pieceRenderer, _boardRenderer, _nextPiece);
            AddEntity(_piece);

            _nextPiece = PieceTemplate.GetRandomPiece();

            _fallInterval.Start();
        }

        public void Update(GameTime gameTime)
        {
            _fallInterval.Update(gameTime);
            _createFallingPieceTimer.Update(gameTime);

            _gui.NextPiece = _nextPiece;
            _gui.Score = _score;

            Entities.ForEach(e => e.Update(gameTime));
        }

        public void Render(RenderingContext renderingContext)
        {
            Entities.ForEach(e => e.Render(renderingContext));
        }
    }
}