using DinaCSharp.Graphics.UI;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Slider"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SliderEventArgs"/> pour le slider spécifié.
    /// </remarks>
    /// <param name="slider">Le slider associé à l'événement.</param>
    public class SliderEventArgs(Slider slider) : EventArgs
    {
        /// <summary>
        /// Le slider qui a déclenché l'événement.
        /// </summary>
        public Slider Slider { get; } = slider;
    }
}
