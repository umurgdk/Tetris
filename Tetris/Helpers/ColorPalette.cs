using System;
using Microsoft.Xna.Framework;

namespace Tetris.Helpers
{
    public class ColorPalette
    {
        private static readonly Random RandomIndexer = new Random((int)DateTime.Now.Ticks);
        private readonly Color[] _colors;
        public Color this[int i] => _colors[i];

        public ColorPalette(Color[] colors)
        {
            _colors = colors;
        }

        public Color PickRandom()
        {
            return _colors[RandomIndexer.Next(_colors.Length)];
        }
    }
}