using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Otaku.Rendering;

namespace Otaku
{
    public class BaseGame : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private RenderingContext _renderingContext;

        private Matrix _projection;
        private Camera2D _camera;
        private ViewportAdapter _viewportAdapter;
        private FramesPerSecondCounter _fpsCounter;

        private SpriteFont _debugFont;

        public IScene Scene { get; protected set; }

        private static BaseGame _instance;
        public static IScene CurrentScene => _instance.Scene;

        public BaseGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 800
            };

            _instance = this;
        }

        protected override void Initialize()
        {
            _renderingContext = new RenderingContext(GraphicsDevice);

            _viewportAdapter = new WindowViewportAdapter(Window, GraphicsDevice);
            _camera = new Camera2D(_viewportAdapter);

            var viewingVolume = new Rectangle(0, 0, _viewportAdapter.ViewportWidth, _viewportAdapter.ViewportHeight);
            _projection = Matrix.CreateOrthographicOffCenter(viewingVolume, 0, 100);

            _fpsCounter = new FramesPerSecondCounter();

            Scene.State = SceneState.Enter;
            Scene.Enter();
            base.Initialize();
        }

        protected override void BeginRun()
        {
            Scene.Entities.ForEach(entity => entity.Start());
            Scene.State = SceneState.Play;
            base.BeginRun();
        }

        protected override void LoadContent()
        {
            Scene.State = SceneState.LoadContent;
            _debugFont = Content.Load<SpriteFont>("Content/font");
            Scene.LoadContent(Content);
            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Scene.BackgroundColor);
            _renderingContext.QuadBatchRenderer.Clear();
            _renderingContext.SpriteBatch.Begin();

            Scene.Render(_renderingContext);

            var fps = _fpsCounter.AverageFramesPerSecond.ToString();
            _renderingContext.SpriteBatch.DrawString(_debugFont, fps, Vector2.Zero, Color.White);

            _renderingContext.QuadBatchRenderer.Render(GraphicsDevice, _camera.GetViewMatrix(), _projection);
            _renderingContext.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update(gameTime);
            _fpsCounter.Update(gameTime);
            Scene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void EndDraw()
        {
            Scene.Entities.RemoveAll(e => !e.Alive);
            base.EndDraw();
        }
    }
}