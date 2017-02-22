using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public class InputManager
    {
        private KeyboardState _oldKeyboardState;
        private MouseState _oldMouseState;

        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }

        public Point MousePosition => MouseState.Position;

        public InputManager()
        {
            _oldKeyboardState = Keyboard.GetState();
            _oldMouseState = Mouse.GetState();

            KeyboardState = _oldKeyboardState;
            MouseState = _oldMouseState;
        }

        public void Update(GameTime gameTime)
        {
            _oldKeyboardState = KeyboardState;
            _oldMouseState = MouseState;

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public bool IsKeyReleased(Keys key)
        {
            return _oldKeyboardState.IsKeyUp(key) && KeyboardState.IsKeyDown(key);
        }
    }
}