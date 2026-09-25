using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaCSharp.Extensions
{
    public static class Texture2DExtensions
    {

        /// <summary>
        /// Récupère les dimensions d'une texture sous forme d'un vecteur 2D.
        /// </summary>
        /// <param name="texture">La texture à partir de laquelle extraire les dimensions.</param>
        /// <returns>Un Vector2 contenant la largeur et la hauteur de la texture.</returns>
        /// <exception cref="ArgumentNullException">Si la texture fournie est <c>null</c>.</exception>
        public static Vector2 ToVector2(this Texture2D texture)
        {
            ArgumentNullException.ThrowIfNull(texture, nameof(texture));
            return new Vector2(texture.Width, texture.Height);
        }
    }
}