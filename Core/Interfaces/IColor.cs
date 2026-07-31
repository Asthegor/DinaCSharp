using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents an object that has an associated color.
    /// </summary>
    public interface IColor
    {
        /// <summary>
        /// Gets or sets the color associated with this instance.
        /// </summary>
        public abstract Color Color { get; set; }
    }
}
