using Microsoft.Xna.Framework;

namespace DinaCSharp.Extensions
{
    public static class ColorExtensions
    {

        /// <summary>
        /// Pre-multiply the color by its own alpha to match the BlendState.AlphaBlend.
        /// </summary>
        /// <param name="color">Color to pre-multiply.</param>
        /// <returns></returns>
        public static Color PreMultiply(this Color color) => Color.Multiply(color, (float)color.A / byte.MaxValue);
    }
}