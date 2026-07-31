using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines members for objects that support collision detection and provide position and size information.
    /// </summary>
    /// <remarks>Implement this interface to enable collision detection between objects in a two-dimensional
    /// space. Inherits position and dimension properties from IPosition and IDimensions.</remarks>
    public interface ICollide : IPosition, IDimensions
    {
        /// <summary>
        /// Determines whether this object collides with the specified item.
        /// </summary>
        /// <param name="item">The object to check for a collision with this instance. Cannot be null.</param>
        /// <returns>true if a collision is detected; otherwise, false.</returns>
        public abstract bool Collide(ICollide item);
        /// <summary>
        /// Gets the bounding rectangle that defines the position and size of the object.
        /// </summary>
        public Rectangle Rectangle { get; }
    }
}
