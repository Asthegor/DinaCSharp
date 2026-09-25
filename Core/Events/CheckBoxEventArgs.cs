using DinaCSharp.Graphics.UI;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="CheckBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="CheckBoxEventArgs"/> pour la CheckBox spécifiée.
    /// </remarks>
    /// <param name="checkBox">La CheckBox associée à l'événement.</param>
    public class CheckBoxEventArgs(CheckBox checkBox) : EventArgs
    {
        /// <summary>
        /// La case à cocher qui a déclenché l'événement.
        /// </summary>
        public CheckBox CheckBox { get; } = checkBox;
    }
}
