using System;

namespace Tetris
{
    public class Timer
    {
        private bool _paused;
        private bool _finished;

        private TimeSpan _remaining;

        public TimeSpan Delay;
        public Action Action;

        public Timer(TimeSpan delay, Action action)
        {
            Delay = delay;
            Action = action;
            _remaining = TimeSpan.FromTicks(delay.Ticks);
        }

        public void Start()
        {
            _paused = false;
        }

        public void Stop()
        {
            _paused = true;
            _remaining = Delay;
        }

        public void Pause()
        {
            _paused = true;
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
                Action();
                _finished = true;
            }
        }
    }
}