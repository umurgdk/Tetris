using System;

namespace Tetris
{
    public class Animation
    {
        private readonly TimeSpan _duration;
        private TimeSpan _currentTime;

        public Animation(TimeSpan duration)
        {
            _duration = duration;
            _currentTime = TimeSpan.Zero;
        }
    }
}