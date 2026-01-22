using DinaCSharp.Interfaces;
using DinaCSharp.Services.Screen;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaCSharp.Services.Fonts
{
    /// <summary>
    /// Tag pour identifier les SpriteFont par une clé.
    /// </summary>
    public sealed class FontTag { }

    /// <summary>
    /// Gère le chargement et l'accès aux SpriteFont en fonction du FontProfile actif.
    /// </summary>
    public sealed class FontManager : IDisposable, IRegister
    {
        private readonly ContentManager _contentManager;

        private static FontManager? _instance;

        private FontManager(IServiceProvider services, string contentPath)
        {
            _contentManager = new ContentManager(services, contentPath);
        }
        /// <summary>
        /// Permet d'initialiser le singleton FontManager.
        /// </summary>
        public static FontManager Initialize(IServiceProvider services, string contentPath, IEnumerable<ResolutionFontInfo> infos, Key<ResolutionTag> defaultResolution)
        {
            if (_instance != null)
                throw new InvalidOperationException("FontManager est déjà initialisé.");

            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(infos);

            if (string.IsNullOrWhiteSpace(contentPath))
                throw new ArgumentException("Le chemin du contenu ne peut pas être nul ou vide.", nameof(contentPath));

            if (!FontProfile.IsInitialized)
                FontProfile.Initialize(infos, defaultResolution);

            _instance = new FontManager(services, contentPath);

            return _instance;
        }

        public void Register(Key<ServiceTag> key)
        {
            ServiceLocator.Register(key, this);
        }

        /// <summary>
        /// Charge une SpriteFont depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé associée à la font.</param>
        public SpriteFont Load(Key<FontTag> key)
        {
            var profile = ServiceLocator.Get<FontProfile>(ServiceKeys.FontProfile)
                ?? throw new InvalidOperationException("FontProfile doit être initialisé avant de charger des fonts.");
            ScreenManager? screenManager = ServiceLocator.Get<ScreenManager>(ServiceKeys.ScreenManager)
                ?? throw new InvalidOperationException("ScreenManager doit être enregistré dans le ServiceLocator avant de charger des fonts.");
            var resolution = screenManager.CurrentResolution;

            var info = profile.GetInfo(resolution);
            string assetPath = $"{info.ContentPath}/{key.Value}";
            try
            {
                var font = _contentManager.Load<SpriteFont>(assetPath);
                return font;
            }
            catch (ContentLoadException ex)
            {
                throw new InvalidOperationException($"Impossible de charger la font '{key.Value}' pour la résolution {resolution}. Chemin : {assetPath}", ex);
            }
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
