using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Otaku
{
    public class InputManager
    {
        private KeyboardState _oldKeyboardState;
        private MouseState _oldMouseState;

        private Dictionary<Keys, HashSet<Action>> _actionBindings;

        private static InputManager _instance;
        public static InputManager Instance => _instance ?? (_instance = new InputManager());

        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }

        public Point MousePosition => MouseState.Position;

        private InputManager()
        {
            _oldKeyboardState = Keyboard.GetState();
            _oldMouseState = Mouse.GetState();

            KeyboardState = _oldKeyboardState;
            MouseState = _oldMouseState;

            _actionBindings = new Dictionary<Keys, HashSet<Action>>();
        }

        public void BindAction(Keys key, Action action)
        {
            if (!_actionBindings.ContainsKey(key))
            {
                _actionBindings[key] = new HashSet<Action>();
            }

            if (!_actionBindings[key].Contains(action))
            {
                _actionBindings[key].Add(action);
            }

            #if DEBUG
            else
            {
                Logger.Instance.Debug("Trying to bind action handler more than once");
            }
            #endif
        }

        public void Update(GameTime gameTime)
        {
            _oldKeyboardState = KeyboardState;
            _oldMouseState = MouseState;

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            foreach (var key in _actionBindings.Keys)
            {
                if (IsKeyReleased(key))
                {
                    foreach (var handler in _actionBindings[key])
                    {
                        handler();
                    }
                }
            }
        }

        public bool IsKeyReleased(Keys key)
        {
            return _oldKeyboardState.IsKeyUp(key) && KeyboardState.IsKeyDown(key);
        }
    }
}