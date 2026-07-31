namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Gets or sets the drawing order of the element relative to other elements.
    /// </summary>
    /// <remarks>Elements with higher Z-order values are rendered above those with lower values. This property
    /// determines the visual stacking order when multiple elements overlap.</remarks>
    public interface IZOrder
    {
        /// <summary>
        /// Gets or sets the drawing order of the element relative to other elements.
        /// </summary>
        /// <remarks>Elements with higher Z-order values are rendered above those with lower values. This
        /// property determines the visual stacking order when multiple elements overlap.</remarks>
        public abstract int ZOrder { get; set; }
    }
}
