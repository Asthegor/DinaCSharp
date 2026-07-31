using System;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines methods for querying and performing click actions on a control or UI element.
    /// </summary>
    /// <remarks>Implement this interface to provide standardized click interaction capabilities for custom
    /// controls or UI elements. Methods allow checking the current click state and simulating left or right mouse
    /// button clicks. Thread safety and input handling behavior depend on the implementing class.</remarks>
    public interface IClickable
    {
        /// <summary>
        /// Determines whether the associated control or element has been clicked.
        /// </summary>
        /// <returns>true if the control or element is currently in a clicked state; otherwise, false.</returns>
        public abstract bool IsClicked();
        /// <summary>
        /// Determines whether the left mouse button is currently pressed.
        /// </summary>
        /// <returns>true if the left mouse button is pressed; otherwise, false.</returns>
        public abstract bool IsLeftClicked();
        /// <summary>
        /// Determines whether the right mouse button is currently pressed.
        /// </summary>
        /// <returns>true if the right mouse button is pressed; otherwise, false.</returns>
        public abstract bool IsRightClicked();
        /// <summary>
        /// Performs a left mouse button click at the current location.
        /// </summary>
        public abstract void LeftClick();
        /// <summary>
        /// Performs a right-click action at the current location or on the associated UI element.
        /// </summary>
        public abstract void RightClick();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface IClickable<TEventArgs> where TEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<TEventArgs>? OnClicked;
    }
}
