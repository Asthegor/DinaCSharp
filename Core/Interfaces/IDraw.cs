using Microsoft.Xna.Framework.Graphics;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for drawable objects that can render themselves using a specified sprite batch.
    /// </summary>
    public interface IDraw
    {
        /// <summary>
        /// Draws the object using the specified sprite batch.
        /// </summary>
        /// <param name="spritebatch">The sprite batch used to render the object. Cannot be null.</param>
        public abstract void Draw(SpriteBatch spritebatch);
    }
}
