using DinaCSharp.Graphics.Texts;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Graphics.Texts.Text"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="TextEventArgs"/> pour le Text spécifié.
    /// </remarks>
    /// <param name="text">Le Text associé à l'événement.</param>
    public class TextEventArgs(Text text) : EventArgs
    {
        /// <summary>
        /// Le Text qui a déclenché l'événement.
        /// </summary>
        public Text Text { get; } = text;
    }
}
