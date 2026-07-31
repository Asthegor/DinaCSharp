namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents a visual element that provides position, dimensions, and z-order information.
    /// </summary>
    /// <remarks>Implementations of this interface combine positional, sizing, and stacking order
    /// capabilities. This interface is typically used as a base for UI or graphical elements that require layout and
    /// rendering information.</remarks>
    public interface IElement : IPosition, IDimensions, IZOrder
    { }
}
