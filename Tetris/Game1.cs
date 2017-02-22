using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

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
                "-##",
                "-##"
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

        private Board _board;
        private Matrix _projection;
        private Camera2D _camera;
        private ViewportAdapter _viewportAdapter;
        private FramesPerSecondCounter _fpsCounter;
        private bool _gameOver;
        private readonly InputManager _inputManager;

        public Game1()
        {
            var manager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 1600,
                SynchronizeWithVerticalRetrace = false
            };

            _inputManager = new InputManager();

            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _debugFont = Content.Load<SpriteFont>("font");

            _quadBatch = new QuadBatchRenderer(GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _fpsCounter = new FramesPerSecondCounter();

            _projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 100);

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            _camera = new Camera2D(_viewportAdapter);

            var boardSize = new Point(10, 20);
            var boardX = _viewportAdapter.VirtualWidth / 2.0f - (Board.BlockSize.X * boardSize.X / 2.0f);
            var boardY = _viewportAdapter.VirtualHeight / 2.0f - (Board.BlockSize.Y * boardSize.Y / 2.0f);
            _board = new Board(new Vector2(boardX, boardY), boardSize, GameOver);

            base.Initialize();
        }

        private void GameOver()
        {
            _gameOver = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouse = Mouse.GetState();

            var mousePos = mouse.Position.ToVector2();
            var pos = _camera.ScreenToWorld(mousePos);

            // TODO: Add your update logic here
            _board.Update(gameTime);
            _fpsCounter.Update(gameTime);

            if (_board.IsWaitingForFallingPiece)
            {
                var randomPieceTemplate = _pieceTemplates[0];
                var piece = Piece.FromTemplate(randomPieceTemplate, Color.White);

                _board.SetFallingPiece(piece);
            }

            if (_inputManager.IsKeyReleased(Keys.A))
            {
                _board.FallingPieceInterval.Delay += TimeSpan.FromMilliseconds(50);
            }

            if (_inputManager.IsKeyReleased(Keys.S))
            {
                _board.FallingPieceInterval.Delay -= TimeSpan.FromMilliseconds(50);
            }

            if (_inputManager.IsKeyReleased(Keys.Space))
            {
                _board.RotateFallingPiece();
            }

            if (_inputManager.IsKeyReleased(Keys.Left))
            {
                _board.MoveFallingPieceLeft();
            }

            if (_inputManager.IsKeyReleased(Keys.Right))
            {
                _board.MoveFallingPieceRight();
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            _quadBatch.Clear();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _board.Render(GraphicsDevice, _quadBatch, _camera.GetViewMatrix(), _projection);
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

        protected override void Dispose(bool disposing)
        {
            _quadBatch?.Dispose();

            base.Dispose(disposing);
        }
    }
}
