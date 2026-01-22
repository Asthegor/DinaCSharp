using DinaCSharp.Interfaces;
using DinaCSharp.Services;

using Microsoft.Xna.Framework.Content;

using System;

namespace DinaCSharp.Resources
{
    /// <summary>
    /// Tag pour identifier les SpriteFont par une clé.
    /// </summary>
    public sealed class ResourceTag { }

    /// <summary>
    /// Gère le chargement et l'accès aux ressources du ContentManager donné.
    /// </summary>
    public sealed class ResourceManager(IServiceProvider services, string contentPath) : IDisposable
    {
        private readonly ContentManager _contentManager = new ContentManager(services, contentPath);

        /// <summary>
        /// Charge une ressource depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé associée à la ressource.</param>
        public T? Load<T>(Key<ResourceTag> key)
        {
            try
            {
                return _contentManager.Load<T>(key.Value);
            }
            catch (ContentLoadException ex)
            {
                throw new InvalidOperationException($"Impossible de charger la ressource. Chemin : {key.Value}", ex);
            }
        }

        public void Register(Key<ResourceTag> key)
        {
            ServiceLocator.Register(key, this);
        }
        /// <summary>
        /// Décharge toutes les fonts.
        /// </summary>
        public void Unload()
        {
            _contentManager.Unload();
        }
        /// <summary>
        /// Permet de libérer les ressources utilisées par le FontManager.
        /// </summary>
        public void Dispose()
        {
            Unload();
            _contentManager.Dispose();
        }

    }
}

