using DinaCSharp.Graphics.UI;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Button"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ButtonEventArgs"/> pour le bouton spécifié.
    /// </remarks>
    /// <param name="button">Le bouton associé à l'événement.</param>
    public class ButtonEventArgs(Button button) : EventArgs
    {
        /// <summary>
        /// Le bouton qui a déclenché l'événement.
        /// </summary>
        public Button Button { get; } = button;
    }
}
