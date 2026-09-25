using DinaCSharp.Services.Scenes;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="SceneManager"/>.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de <see cref="SceneManagerEventArgs"/> pour le SceneManager spécifié.
    /// </remarks>
    /// <param name="sceneManager">Le SceneManager associé à l'événement.</param>
    public class SceneManagerEventArgs(SceneManager sceneManager) : EventArgs
    {
        /// <summary>
        /// Le SceneManager qui a déclenché l'événement.
        /// </summary>
        public SceneManager SceneManager { get; } = sceneManager;
    }
}
