namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a complete game object that supports loading, resetting, updating, and drawing
    /// operations.
    /// </summary>
    /// <remarks>Implement this interface to represent a game object that participates fully in the game loop
    /// lifecycle. This includes resource loading, state resetting, per-frame updates, and rendering. The interface
    /// combines the behaviors of ILoad, IReset, IUpdate, and IDraw for convenience and consistency.</remarks>
    public interface IFullGameObject : ILoad, IReset, IUpdate, IDraw
    { }
}
