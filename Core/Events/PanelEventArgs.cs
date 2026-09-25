using DinaCSharp.Graphics.UI;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Panel"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="PanelEventArgs"/> pour le panneau spécifié.
    /// </remarks>
    /// <param name="panel">Le panneau associé à l'événement.</param>
    public class PanelEventArgs(Panel panel) : EventArgs
    {
        /// <summary>
        /// Le panneau qui a déclenché l'événement.
        /// </summary>
        public Panel Panel { get; } = panel;
    }
}
