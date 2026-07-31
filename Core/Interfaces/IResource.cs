namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines methods for managing named resources within a collection.
    /// </summary>
    /// <remarks>Implementations of this interface allow adding, retrieving, and removing resources by name.
    /// Resource names must be unique within the collection. This interface is typically used to provide flexible
    /// storage and retrieval of resources of varying types.</remarks>
    public interface IResource
    {
        /// <summary>
        /// Adds a resource with the specified name and value to the collection.
        /// </summary>
        /// <typeparam name="T">The type of the resource to add.</typeparam>
        /// <param name="resourceName">The name that uniquely identifies the resource. Cannot be null or empty.</param>
        /// <param name="resource">The resource value to add.</param>
        /// <returns>true if the resource was added successfully; otherwise, false.</returns>
        public abstract bool AddResource<T>(string resourceName, T resource);
        /// <summary>
        /// Retrieves a resource of the specified type by its name.
        /// </summary>
        /// <typeparam name="T">The type of the resource to retrieve.</typeparam>
        /// <param name="resourceName">The name of the resource to retrieve. Cannot be null or empty.</param>
        /// <returns>The resource of type T if found; otherwise, null.</returns>
        public abstract T? GetResource<T>(string resourceName);
        /// <summary>
        /// Removes the resource with the specified name from the collection.
        /// </summary>
        /// <param name="resourceName">The name of the resource to remove. Cannot be null or empty.</param>
        public abstract void RemoveResource(string resourceName);
    }
}
