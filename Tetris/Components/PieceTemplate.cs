using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tetris.Helpers;

namespace Tetris.Components
{
    public static class PieceTemplate
    {
        private static readonly Random RandomIndexer = new Random((int)DateTime.Now.Ticks);
        private static readonly List<string[]> PieceTemplates = new List<string[]>
        {
            new[]
            {
                "-##",
                "##-"
            },

            new[]
            {
                "##-",
                "-##"
            },

            new[]
            {
                "##",
                "##"
            },

            new[]
            {
                "#",
                "#",
                "#",
                "#"
            },

            new[]
            {
                "#-",
                "#-",
                "##"
            },

            new[]
            {
                "-#",
                "-#",
                "##"
            },

            new[]
            {
                "-#-",
                "###"
            }
        };

        private static readonly ColorPalette ColorPalette = new ColorPalette(new []{
            new Color(96, 180, 198),
            new Color(96, 180, 198),
            new Color(100, 139, 202),
            new Color(102, 119, 203),
            new Color(108, 104, 205),
            new Color(131, 106, 207),
            new Color(155, 108, 208)
        });

        public static Piece GetRandomPiece()
        {
            var randomTemplateIndex = RandomIndexer.Next(PieceTemplates.Count);
            return Piece.FromTemplate(PieceTemplates[randomTemplateIndex], ColorPalette[randomTemplateIndex]);
        }
    }
}