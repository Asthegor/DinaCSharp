using DinaCSharp.Services.Scenes;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="Scene"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SceneEventArgs"/> pour la scène spécifiée.
    /// </remarks>
    /// <param name="scene">La scène associée à l'événement.</param>
    public class SceneEventArgs(Scene scene) : EventArgs
    {
        /// <summary>
        /// La scène qui a déclenché l'événement.
        /// </summary>
        public Scene Scene { get; } = scene;
    }
}
