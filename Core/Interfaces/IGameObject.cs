namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a game object that supports update and draw operations.
    /// </summary>
    /// <remarks>Implement this interface to represent entities within a game that require regular updates and
    /// rendering. Inheriting from both IUpdate and IDraw ensures that the object can participate in the game loop for
    /// logic updates and visual output.</remarks>
    public interface IGameObject : IUpdate, IDraw
    { }
}
