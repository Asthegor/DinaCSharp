using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents an object that exposes its width and height as a two-dimensional vector.
    /// </summary>
    /// <remarks>Implementations typically use the X and Y components of the vector to represent width and
    /// height, respectively. The interface does not specify units; consumers should refer to the implementing type's
    /// documentation for details.</remarks>
    public interface IDimensions
    {
        /// <summary>
        /// Gets or sets the width and height of the object as a two-dimensional vector.
        /// </summary>
        public abstract Vector2 Dimensions { get; set; }
    }
}
