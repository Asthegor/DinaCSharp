
using Microsoft.Xna.Framework;

using System;

namespace DinaCSharp.Core.Utils
{
    /// <summary>
    /// Permet de scaler les éléments UI en fonction de la résolution de l'écran.
    /// </summary>
    public static class UIScaler
    {
        private const int REF_WIDTH = 1920;
        private const int REF_HEIGHT = 1080;

        private static float _scale;

        /// <summary>
        /// Met à jour le facteur de scale en fonction des dimensions de l'écran.
        /// </summary>
        /// <param name="screenDimensions">Dimensions de l'écran.</param>
        public static void Update(Vector2 screenDimensions)
        {
            float scaleX = screenDimensions.X / REF_WIDTH;
            float scaleY = screenDimensions.Y / REF_HEIGHT;
            _scale = MathF.Min(scaleX, scaleY); // conserve proportions
        }
        /// <summary>
        /// Met à l'échelle une valeur entière en fonction du facteur de scale.
        /// </summary>
        /// <param name="value">Valeur entière.</param>
        /// <returns></returns>
        public static int Scale(int value) => (int)(value * _scale);
        /// <summary>
        /// Met à l'échelle une valeur flottante en fonction du facteur de scale.
        /// </summary>
        /// <param name="value">Valeur flottante.</param>
        /// <returns></returns>
        public static float Scale(float value) => value * _scale;
        /// <summary>
        /// Met à l'échelle un vecteur 2D en fonction du facteur de scale.
        /// </summary>
        /// <param name="v">Vecteur 2D.</param>
        /// <returns></returns>
        public static Vector2 Scale(Vector2 v) => v * _scale;
    }

}
