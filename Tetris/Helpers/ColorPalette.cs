using System;
using Microsoft.Xna.Framework;

namespace Tetris.Helpers
{
    public class ColorPalette
    {
        private readonly Color[] _colors;
        public Color this[int i] => _colors[i];

        private Random _random;

        public ColorPalette(Color[] colors)
        {
            _colors = colors;
        }

        public Color PickRandom()
        {
            if (_random == null)
            {
                _random = new Random();
            }

            return _colors[_random.Next(_colors.Length)];
        }
    }
}