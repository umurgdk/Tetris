using System;

namespace Tetris.Helpers
{
    public class Interval
    {
        private bool _paused;

        private int _currentRepeat = 0;
        private TimeSpan _remaining;

        public TimeSpan Delay;
        public Action Action;
        public int MaxRepeat = 0;

        public Interval(TimeSpan delay, Action action, int maxRepeat = 0)
        {
            Delay = TimeSpan.FromTicks(delay.Ticks);
            Action = action;
            MaxRepeat = maxRepeat;
            _remaining = TimeSpan.FromTicks(delay.Ticks);
            _paused = true;
        }

        public void Start()
        {
            _paused = false;
        }

        public void Stop()
        {
            _paused = true;
            _remaining = new TimeSpan(Delay.Ticks);
            _currentRepeat = 0;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Update(TimeSpan deltaTime)
        {
            if (_paused || (MaxRepeat != 0 && (_currentRepeat >= MaxRepeat)))
            {
                return;
            }

            _remaining -= deltaTime;

            if (_remaining.Ticks <= 0)
            {
                Action();
                _currentRepeat += 1;
                _remaining = new TimeSpan(Delay.Ticks);
            }
        }
    }
}