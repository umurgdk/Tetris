using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Tetris.Helpers
{
    public class Timer
    {
        private bool _paused;
        private bool _finished;

        private TimeSpan _remaining;

        private TimeSpan _delay;
        private readonly Action _action;

        private bool _firstFrame;

        public Timer(TimeSpan delay, Action action)
        {
            _delay = delay;
            _action = action;
            _remaining = TimeSpan.FromTicks(delay.Ticks);
            _paused = true;
            _firstFrame = true;
        }

        public void Start()
        {
            _paused = false;
        }

        public void Stop()
        {
            _paused = true;
            _remaining = TimeSpan.FromTicks(_delay.Ticks);
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Restart()
        {
            _finished = false;
            _paused = false;
            _remaining = TimeSpan.FromTicks(_delay.Ticks);
            _firstFrame = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                return;
            }

            if (_finished || _paused)
            {
                return;
            }

            _remaining -= gameTime.ElapsedGameTime;

            if (_remaining.TotalSeconds <= 0)
            {
                _action();
                _finished = true;
            }
        }
    }
}