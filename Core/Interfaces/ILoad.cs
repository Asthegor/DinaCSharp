namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for loading resources or data associated with the implementing class.
    /// </summary>
    /// <remarks>Implementing types must provide the logic for loading the relevant resource or data. The
    /// specific loading behavior is determined by the implementation.</remarks>
    public interface ILoad
    {
        /// <summary>
        /// Loads the resource or data associated with the implementing class.
        /// </summary>
        /// <remarks>This method must be implemented by derived classes to define how the resource or data
        /// is loaded. The specific behavior depends on the implementation.</remarks>
        public abstract void Load();
    }
}
