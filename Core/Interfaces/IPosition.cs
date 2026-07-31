using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents an object that has a position in two-dimensional space.
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Gets or sets the position of the object in two-dimensional space.
        /// </summary>
        public abstract Vector2 Position { get; set; }
    }
}
