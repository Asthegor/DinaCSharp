using Microsoft.Xna.Framework;

using System;
using System.Globalization;

namespace DinaCSharp.Helpers
{
    internal static class ColorHelper
    {

        /// <summary>
        /// Convertit une couleur hexadécimale en Color MonoGame.
        /// Format accepté : RRGGBB ou AARRGGBB, avec ou sans '#'.
        /// </summary>
        public static Color FromHex(string hex)
        {
            ArgumentException.ThrowIfNullOrEmpty(hex);

            hex = hex.Trim();
            if (string.IsNullOrWhiteSpace(hex))
                return Color.Transparent;

            hex = hex.TrimStart('#');

            if (hex.Length == 6)
            {
                byte r = byte.Parse(hex.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte g = byte.Parse(hex.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte b = byte.Parse(hex.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return new Color(r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = byte.Parse(hex.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte r = byte.Parse(hex.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte g = byte.Parse(hex.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte b = byte.Parse(hex.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return new Color(r, g, b, a);
            }
            else
            {
                throw new ArgumentException($"Format hex invalide : {hex}");
            }
        }
    }
}