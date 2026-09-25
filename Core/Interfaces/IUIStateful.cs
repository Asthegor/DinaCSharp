using DinaCSharp.Core.Enums;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Represents a UI component that exposes its current state for reading and updating.
    /// </summary>
    /// <remarks>Implement this interface to provide access to the UI state in components that support
    /// stateful interactions. The UI state can be used to track and manage the current visual or logical state of the
    /// component.</remarks>
    public interface IUIStateful
    {
        /// <summary>
        /// Gets or sets the current user interface state.
        /// </summary>
        public abstract UIState UIState { get; set; }
    }
}
