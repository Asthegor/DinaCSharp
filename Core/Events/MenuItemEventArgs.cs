using DinaCSharp.Services.Menus;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="MenuItem"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="MenuItemEventArgs"/> pour le MenuItem spécifié.
    /// </remarks>
    /// <param name="menuitem">Le MenuItem associé à l'événement.</param>
    public class MenuItemEventArgs(MenuItem menuitem) : EventArgs
    {
        /// <summary>
        /// Le MenuItem qui a déclenché l'événement.
        /// </summary>
        public MenuItem MenuItem { get; } = menuitem;
    }
}
