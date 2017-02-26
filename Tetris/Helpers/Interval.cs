using System;
using Microsoft.Xna.Framework;

namespace Tetris.Helpers
{
    public class Interval
    {
        private bool _paused;

        private int _currentRepeat = 0;
        private TimeSpan _delay;
        private TimeSpan _remaining;
        private readonly Action _action;
        private readonly int _maxRepeat = 0;
        private bool _firstFrame;

        public Interval(TimeSpan delay, Action action, int maxRepeat = 0)
        {
            _delay = TimeSpan.FromTicks(delay.Ticks);
            _action = action;
            _maxRepeat = maxRepeat;
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
            _remaining = new TimeSpan(_delay.Ticks);
            _currentRepeat = 0;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                return;
            }

            if (_paused || (_maxRepeat != 0 && (_currentRepeat >= _maxRepeat)))
            {
                return;
            }

            _remaining -= gameTime.ElapsedGameTime;;

            if (_remaining.Ticks <= 0)
            {
                _action();
                _currentRepeat += 1;
                _remaining = TimeSpan.FromTicks(_delay.Ticks);
            }
        }
    }
}