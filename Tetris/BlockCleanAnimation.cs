using System;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class BlockCleanAnimation
    {
        private readonly BlockRow _blockRow;
        private readonly TimeSpan _duration;

        private TimeSpan _currentTime;
        private float _step;

        public bool IsFinished { get; private set; } = false;

        public BlockCleanAnimation(BlockRow blockRow, TimeSpan duration)
        {
            _blockRow = blockRow;
            _duration = duration;

            _currentTime = TimeSpan.Zero;
            _step = 1.0f;
        }

        public BlockCleanAnimation(BlockRow blockRow) : this(blockRow, TimeSpan.FromSeconds(0.5))
        {
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime;
            _currentTime += deltaTime;

            var timing = (float)_currentTime.Milliseconds / _duration.Milliseconds;
            _step = (float) Math.Cos((Math.PI / 2) * timing);

            if (_currentTime > _duration)
            {
                IsFinished = true;
            }
        }

        public void Render(QuadBatchRenderer renderer, Vector2 blockSize)
        {
            for (var col = 0; col < _blockRow.Blocks.Length; col++)
            {
                var blockPosition = _blockRow.WorldPosition;
                blockPosition.X += col * blockSize.X;

                var newSize = blockSize * _step;
                var sizeDiff = blockSize - newSize;
                var centeredPosition = blockPosition + sizeDiff / 2.0f;

                renderer.AddQuad(centeredPosition, newSize, new Color(_blockRow.Blocks[col].Color, _step));
            }
        }
    }
}