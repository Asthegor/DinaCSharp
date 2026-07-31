namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents a drawable element within a drawing or graphics context.
    /// </summary>
    /// <remarks>Implementations of this interface support both general element behavior and drawing
    /// operations by combining the contracts of IElement and IDraw. This interface is typically used as a base for
    /// graphical components that can be rendered on a surface.</remarks>
    public interface IDrawingElement : IElement, IDraw
    { }
}
