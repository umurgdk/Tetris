using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tetris
{

    public class BoardRowFallAnimation
    {
        private readonly TimeSpan _duration;

        private TimeSpan _currentTime;
        private float _step;

        public bool IsFinished { get; private set; }
        public readonly Dictionary<int, int> RowIndexMoves;

        public BoardRowFallAnimation(TimeSpan duration, Dictionary<int, int> rowIndexMoves)
        {
            _duration = duration;
            RowIndexMoves = rowIndexMoves;

            _currentTime = TimeSpan.Zero;
            _step = 0.0f;
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime;
            _currentTime += deltaTime;

            var timing = (float)_currentTime.Milliseconds / _duration.Milliseconds;
            _step = (float) Math.Sin((Math.PI / 2) * timing);

            if (_currentTime > _duration)
            {
                IsFinished = true;
            }
        }

        public Vector2 GetRowOffset(int rowIndex, Vector2 blockSize)
        {
            if (!RowIndexMoves.ContainsKey(rowIndex))
            {
                return Vector2.Zero;
            }

            var moveScale = RowIndexMoves[rowIndex];
            var targetOffset = new Vector2(0, moveScale * blockSize.Y);

            return targetOffset * _step;
        }
    }
}