namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a method that creates a copy of the current object.
    /// </summary>
    /// <remarks>Implement this interface to provide a strongly typed copy operation for objects. The returned
    /// copy should be independent of the original object, unless otherwise specified by the implementation.</remarks>
    /// <typeparam name="T">The type of object that is returned by the copy operation.</typeparam>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Creates a copy of the current instance.
        /// </summary>
        /// <remarks>The returned copy is typically a deep copy, but the exact semantics depend on the
        /// implementation in the derived class. Callers should refer to the specific implementation for details on what
        /// is copied.</remarks>
        /// <returns>A new instance of type T that is a copy of the current object.</returns>
        public abstract T Copy();
    }
}
