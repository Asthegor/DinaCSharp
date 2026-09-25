using DinaCSharp.Services.Screen;

using System;

namespace DinaCSharp.Core.Events
{

    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="ScreenManager"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ScreenManagerEventArgs"/> pour le ScreenManager spécifié.
    /// </remarks>
    /// <param name="screenManager">Le ScreenManager associé à l'événement.</param>
    public class ScreenManagerEventArgs(ScreenManager screenManager) : EventArgs
    {
        /// <summary>
        /// Le ScreenManager qui a déclenché l'événement.
        /// </summary>
        public ScreenManager ScreenManager { get; } = screenManager;
    }
}
