using System;
using System.Collections.Generic;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for determining whether an object is currently in a hovered state.
    /// </summary>
    public interface IHovered
    {
        /// <summary>
        /// Determines whether the current element is being hovered over by the user.
        /// </summary>
        /// <returns>true if the element is currently hovered; otherwise, false.</returns>
        public abstract bool IsHovered();
    }
}
