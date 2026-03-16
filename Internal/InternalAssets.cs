using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Reflection;

namespace DinaCSharp.Internal
{
    internal static class InternalAssets
    {
        private static Texture2D? _circle;
        private static Texture2D? _logo;
        private static Texture2D? _dropDownArrow;
        private static Texture2D? _pixel;
        public static Texture2D Pixel(GraphicsDevice device)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(device, 1, 1);
                _pixel.SetData([Color.White]);
            }
            return _pixel;
        }
        public static Texture2D Circle(GraphicsDevice device)
        {
            _circle ??= GetInternalResource(device, "CircleMask.png");
            return _circle;
        }
        public static Texture2D Logo(GraphicsDevice device)
        {
            _logo ??= GetInternalResource(device, "Logo.png");
            return _logo;
        }
        public static Texture2D DropDownArrow(GraphicsDevice device)
        {
            _dropDownArrow ??= GetInternalResource(device, "DropDownArrow.png");
            return _dropDownArrow;
        }
        private static Texture2D GetInternalResource(GraphicsDevice device, string filename)
        {
            Assembly assembly = typeof(InternalAssets).Assembly;
            string resourcePrefix = assembly.GetName().Name ?? string.Empty;
            string filepath = $"{(!string.IsNullOrEmpty(resourcePrefix) ? string.Concat(resourcePrefix, '.') : string.Empty)}Resources.{filename}";
            using Stream? stream = assembly.GetManifestResourceStream(filepath);
            return stream == null
                ? throw new FileNotFoundException($"{filename} introuvable dans les ressources embarquées. Chemin: {filepath}")
                : Texture2D.FromStream(device, stream);
        }
    }
}
