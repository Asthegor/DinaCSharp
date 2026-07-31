using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Gets or sets the horizontal and vertical flip factors applied to the object.
    /// </summary>
    /// <remarks>A value of (1, 1) indicates no flip. Negative values invert the corresponding axis. This
    /// property is typically used to mirror or reverse the object's orientation along the X or Y axis.</remarks>
    public interface IFlip
    {
        /// <summary>
        /// Gets or sets the horizontal and vertical flip factors applied to the object.
        /// </summary>
        /// <remarks>A value of (1, 1) indicates no flip. Negative values invert the corresponding axis.
        /// This property is typically used to mirror or reverse the object's orientation along the X or Y
        /// axis.</remarks>
        public abstract Vector2 Flip { get; set; }
    }
}
