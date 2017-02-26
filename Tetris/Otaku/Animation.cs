using System;
using Microsoft.Xna.Framework;

namespace Otaku
{
    public class Animation
    {
        private readonly TimeSpan _duration;
        private TimeSpan _elapsedTime;
        private bool _firstFrame;
        private bool _isRunning;

        public float Step { get; private set; }
        public bool IsFinished { get; private set; }

        public Animation(TimeSpan duration)
        {
            _duration = duration;
            _elapsedTime = TimeSpan.Zero;
            _firstFrame = true;
            _isRunning = false;

            Step = 0;
            IsFinished = false;
        }

        public void Start()
        {
            _isRunning = true;
        }

        public void Pause()
        {
            _isRunning = false;
        }

        public void Reset()
        {
            _isRunning = false;
            _elapsedTime = TimeSpan.Zero;
            _firstFrame = true;

            Step = 0;
            IsFinished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isRunning)
            {
                return;
            }

            if (!_firstFrame)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
            }

            if (_elapsedTime >= _duration)
            {
                IsFinished = true;
                _isRunning = false;
            }

            _firstFrame = false;
            Step = Convert.ToSingle(_elapsedTime.TotalMilliseconds / _duration.TotalMilliseconds);
        }
    }
}