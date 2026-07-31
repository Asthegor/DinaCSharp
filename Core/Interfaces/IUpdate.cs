using Microsoft.Xna.Framework;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for updating the state of a game component based on the elapsed game time.
    /// </summary>
    /// <remarks>Implement this interface to provide custom update logic that executes during each game update
    /// cycle. The update method is typically called once per frame by the game loop.</remarks>
    public interface IUpdate
    {
        /// <summary>
        /// Updates the state of the game component based on the elapsed game time.
        /// </summary>
        /// <remarks>Override this method to implement logic that should be executed on each game update
        /// cycle. This method is typically called once per frame.</remarks>
        /// <param name="gametime">An object that provides a snapshot of timing values, including the elapsed game time since the last update.</param>
        public abstract void Update(GameTime gametime);
    }
}
