namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a method that resets the state of an object to its initial configuration.
    /// </summary>
    /// <remarks>Implement this interface to provide a standardized way to reinitialize objects for reuse. The
    /// specific behavior of the reset operation depends on the implementing type.</remarks>
    public interface IReset
    {
        /// <summary>
        /// Resets the state of the object to its initial configuration.
        /// </summary>
        /// <remarks>Call this method to reinitialize the object so that it can be reused as if newly
        /// created. The specific effects of this operation depend on the implementation.</remarks>
        public abstract void Reset();
    }
}
