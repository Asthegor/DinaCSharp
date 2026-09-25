using DinaCSharp.Graphics;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="ListBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ListBoxEventArgs"/> pour la ListBox spécifiée.
    /// </remarks>
    /// <param name="listBox">La ListBox associée à l'événement.</param>
    public class ListBoxEventArgs(ListBox listBox) : EventArgs
    {
        /// <summary>
        /// La liste qui a déclenché l'événement.
        /// </summary>
        public ListBox ListBox { get; } = listBox;
    }
}
