using System;

namespace Tetris.Helpers
{
    public class Timer
    {
        private bool _paused;
        private bool _finished;

        private TimeSpan _remaining;

        private TimeSpan _delay;
        private readonly Action _action;

        public Timer(TimeSpan delay, Action action)
        {
            _delay = delay;
            _action = action;
            _remaining = TimeSpan.FromTicks(delay.Ticks);
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
        }

        public void Update(TimeSpan deltaTime)
        {
            if (_finished || _paused)
            {
                return;
            }

            _remaining -= deltaTime;

            if (_remaining.TotalSeconds <= 0)
            {
                _action();
                _finished = true;
            }
        }
    }
}