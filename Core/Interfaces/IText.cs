using DinaCSharp.Core.Enums;

using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines an interface for text elements that provide position, dimensions, and alignment capabilities, as well as
    /// access to the rendered text size.
    /// </summary>
    /// <remarks>Implementations of this interface allow querying the rendered size of text and adjusting its
    /// alignment within a layout. This interface extends both IPosition and IDimensions, ensuring that position and
    /// size information is available alongside text-specific features.</remarks>
    public interface IText : IPosition, IDimensions, IColor, IDraw
    {
        /// <summary>
        /// Gets the width and height, in device-independent units, of the rendered text content.
        /// </summary>
        public abstract Vector2 TextDimensions { get; }
        /// <summary>
        /// Sets the horizontal and vertical alignment for the content.
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment to apply. Must be a valid value of the HorizontalAlignment enumeration.</param>
        /// <param name="verticalAlignment">The vertical alignment to apply. Must be a valid value of the VerticalAlignment enumeration.</param>
        public abstract void SetAlignments(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment);
    }
}
