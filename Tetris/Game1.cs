using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Tetris.Helpers;

namespace Tetris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private QuadBatchRenderer _quadBatch;
        private SpriteFont _debugFont;

        private readonly List<string[]> _pieceTemplates = new List<string[]>
        {
            new[]
            {
                "-##",
                "##-"
            },

            new[]
            {
                "##-",
                "-##"
            },

            new[]
            {
                "##",
                "##"
            },

            new[]
            {
                "#",
                "#",
                "#",
                "#"
            },

            new[]
            {
                "#-",
                "#-",
                "##"
            },

            new[]
            {
                "-#",
                "-#",
                "##"
            },

            new[]
            {
                "-#-",
                "###"
            }
        };

        private readonly ColorPalette _mainPalette = new ColorPalette(new []{
            new Color(254, 74, 73),
            new Color(42, 183, 202),
            new Color(254, 215, 102),
            new Color(125, 206, 105),
            new Color(46, 64, 82)
        });

        private readonly GraphicsDeviceManager _deviceManager;

        private Board _board;
        private Matrix _projection;
        private Camera2D _camera;
        private ViewportAdapter _viewportAdapter;
        private FramesPerSecondCounter _fpsCounter;
        private readonly InputManager _inputManager;
        private Piece _fallingPiece;
        private readonly Interval _fallInterval;
        private readonly Timer _createFallingPieceTimer;
        private bool _gameOver = false;
        private List<BlockCleanAnimation> _blockCleanAnimations;

        public static readonly Vector2 BlockSize = new Vector2(30);
        public static readonly Point BoardSize = new Point(10, 20);

        public Game1()
        {
            _deviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 800
            };

            _inputManager = new InputManager();

            _fallInterval = new Interval(TimeSpan.FromSeconds(0.3), MoveFallingPieceDown);
            _fallInterval.Start();

            _createFallingPieceTimer = new Timer(TimeSpan.FromSeconds(0.5), CreateFallingPiece);
            _createFallingPieceTimer.Stop();

            _blockCleanAnimations = new List<BlockCleanAnimation>();

            Content.RootDirectory = "Content";
        }

        private void CreateFallingPiece()
        {
            _fallingPiece = CreateRandomPiece();
            _fallInterval.Start();
            _createFallingPieceTimer.Stop();
        }

        private void MoveFallingPieceDown()
        {
            if (_board.CanPieceContinueFalling(_fallingPiece))
            {
                _fallingPiece.Position.Y += 1;
            }

            else if (_fallingPiece.Position.Y <= 0)
            {
                _gameOver = true;
                _fallInterval.Stop();
                _fallingPiece = null;
            }

            else
            {
                PlaceFallingPieceIntoBoard();
            }
        }

        void PlaceFallingPieceIntoBoard()
        {
            _board.AddPieceIntoPlace(_fallingPiece);
            _fallingPiece = null;
            _fallInterval.Stop();

            _createFallingPieceTimer.Restart();

            var cleanAnimations = _board.ExtractFullRows().Select(blockRow => new BlockCleanAnimation(blockRow));
            _blockCleanAnimations.AddRange(cleanAnimations);
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _fpsCounter = new FramesPerSecondCounter();

            _projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 0, 0, 100);

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            _camera = new Camera2D(_viewportAdapter);

            var center = _viewportAdapter.Center.ToVector2();
            var boardSizeInPixels = BoardSize.ToVector2() * BlockSize;

            _board = new Board(BoardSize, BlockSize);
            _board.Transform.Position = new Vector3(center - boardSizeInPixels / 2.0f, 0);

            _fallingPiece = CreateRandomPiece();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _quadBatch = new QuadBatchRenderer(GraphicsDevice);

            _debugFont = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            _fallInterval.Update(gameTime.ElapsedGameTime);
            _createFallingPieceTimer.Update(gameTime.ElapsedGameTime);

            if (_fallingPiece != null)
            {
                if (_inputManager.IsKeyReleased(Keys.Right) &&
                    _board.CanPieceMoveTo(_fallingPiece, BoardMoveDirection.Right))
                {
                    _fallingPiece.Position.X += 1;
                }

                else if (_inputManager.IsKeyReleased(Keys.Left) &&
                         _board.CanPieceMoveTo(_fallingPiece, BoardMoveDirection.Left))
                {
                    _fallingPiece.Position.X -= 1;
                }

                else if (_inputManager.IsKeyReleased(Keys.Space))
                {
                    _fallingPiece.Rotate();
                }

                else if (_inputManager.IsKeyReleased(Keys.Down))
                {
                    _fallingPiece.Position = _board.GetGroundPositionForPiece(_fallingPiece);
                    PlaceFallingPieceIntoBoard();
                }
            }

            _blockCleanAnimations.ForEach(animation => animation.Update(gameTime));

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _quadBatch.Clear();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _board.Render(_quadBatch);

            _fallingPiece?.Render(_quadBatch, _board.BoardToWorldPosition(_fallingPiece.Position.Y, _fallingPiece.Position.X), BlockSize);
            _blockCleanAnimations.ForEach(animation => animation.Render(_quadBatch, BlockSize));

            _quadBatch.Render(GraphicsDevice, _camera.GetViewMatrix(), _projection);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_debugFont, _fpsCounter.AverageFramesPerSecond.ToString(), Vector2.Zero, Color.White);
            if (_gameOver)
            {
                _spriteBatch.DrawString(_debugFont, "GAME OVER!", _viewportAdapter.Center.ToVector2(), Color.Red);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            var cleanAnimationsLength = _blockCleanAnimations.Count;
            _blockCleanAnimations = _blockCleanAnimations.Where(animation => !animation.IsFinished).ToList();

            if (cleanAnimationsLength > _blockCleanAnimations.Count)
            {
                _board.FallRows();
            }

            base.EndDraw();
        }

        private Piece CreateRandomPiece()
        {
            var randomIndex = new Random().Next(_pieceTemplates.Count);
            return Piece.FromTemplate(_pieceTemplates[randomIndex], _mainPalette.PickRandom());
        }

        protected override void Dispose(bool disposing)
        {
            _quadBatch?.Dispose();

            base.Dispose(disposing);
        }
    }
}