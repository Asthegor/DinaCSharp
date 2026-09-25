using DinaCSharp.Graphics.UI;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un clic sur un élément d'une <see cref="ListBox"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="ListBoxClickEventArgs"/> pour l'index spécifié.
    /// </remarks>
    /// <param name="index">L'index de l'élément cliqué.</param>
    public class ListBoxClickEventArgs(int index) : EventArgs
    {
        /// <summary>
        /// L'index de l'élément cliqué dans la liste.
        /// </summary>
        public int Index { get; } = index;
    }
}
