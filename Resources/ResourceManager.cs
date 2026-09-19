using DinaCSharp.Services;
using DinaCSharp.Services.Keys;
using DinaCSharp.SpriteSheets;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

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
        private sealed class ParsedSheet(Texture2D texture)
        {
            public string Name = string.Empty;
            public Texture2D FullTexture = texture ?? throw new ArgumentNullException(nameof(texture));
            public Dictionary<string, Rectangle> Regions = [];
        }
        
        private static readonly Dictionary<string, Texture2D> textureCache = [];
        private static readonly Dictionary<string, Texture2D> subTextureCache = [];

        private readonly ContentManager _contentManager = new ContentManager(services, contentPath);

        /// <summary>
        /// Charge une ressource depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé contenant le chemin pour accéder à la ressource.</param>
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
        /// <summary>
        /// Décharge une ressource depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé de la ressource.</param>
        public void Unload<T>(Key<ResourceTag> key)
        {
            try
            {
                _contentManager.UnloadAsset(key.Value);
            }
            catch (ObjectDisposedException)
            {
                // Ne rien faire car la ressource a déjà été déchargée
            }
        }
        /// <summary>
        /// Charge une SpriteSheet en combinant la texture et son fichier XML associé portant le même nom.
        /// </summary>
        public SpriteSheet LoadSpriteSheet(Key<ResourceTag> key)
        {
            // 1. Charge la texture via le mécanisme existant
            if(!textureCache.TryGetValue(key.Value, out var texture))
            {
                texture = Load<Texture2D>(key);
                textureCache[key.Value] = texture!;
            }

            // 2. Initialise la SpriteSheet
            var spritesheetname = Path.GetFileName(key.Value);
            var parsedSheet = new ParsedSheet(texture!) { Name = spritesheetname };


            // 3. Charge et parse le fichier XML associé via le ContentManager/TitleContainer
            string xmlPath = Path.Combine(_contentManager.RootDirectory, key.Value + ".xml");

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"Fichier '{xmlPath}' not found");

            using (var stream = TitleContainer.OpenStream(xmlPath))
            {
                var doc = XDocument.Load(stream);
                foreach (var elem in doc.Descendants("SubTexture"))
                {
                    string? name = elem.Attribute("name")?.Value;
                    if (name == null)
                        continue;

                    int x = int.Parse(elem.Attribute("x")!.Value, CultureInfo.InvariantCulture);
                    int y = int.Parse(elem.Attribute("y")!.Value, CultureInfo.InvariantCulture);
                    int w = int.Parse(elem.Attribute("width")!.Value, CultureInfo.InvariantCulture);
                    int h = int.Parse(elem.Attribute("height")!.Value, CultureInfo.InvariantCulture);

                    parsedSheet.Regions[name] = new Rectangle(x, y, w, h);
                }
            }
            if (parsedSheet.FullTexture == null)
                throw new InvalidOperationException("La texture de la spritesheet n'a pas pu être chargée.");
            var spriteSheet = new SpriteSheet(parsedSheet.Name)
            {
                Texture = parsedSheet.FullTexture!,
            };
            foreach (var kvp in parsedSheet.Regions)
                spriteSheet.Regions[kvp.Key] = kvp.Value;
            return spriteSheet;
        }
        /// <summary>
        /// Chargement d'une sous=texture d'un SpriteSheet
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="textureName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static Texture2D LoadSubTexture(SpriteSheet spriteSheet, string textureName)
        {
            ArgumentNullException.ThrowIfNull(spriteSheet);

            // Clé unique combinant le nom de la spritesheet et le nom du sprite
            string cacheKey = $"{spriteSheet.Name}/{textureName}";

            if (subTextureCache.TryGetValue(cacheKey, out var cachedTexture))
                return cachedTexture;

            if (!spriteSheet.Regions.TryGetValue(textureName, out Rectangle rect))
                throw new KeyNotFoundException($"Le sprite '{textureName}' n'existe pas dans la spritesheet '{spriteSheet.Name}'.");

            // Extraire les pixels
            Color[] pixels = new Color[rect.Width * rect.Height];
            spriteSheet.Texture?.GetData(0, rect, pixels, 0, pixels.Length);

            // Créer la texture
            Texture2D texture1px = ServiceLocator.Get<Texture2D>(DinaServiceKeys.Texture1px)!;
            Texture2D texture = new Texture2D(texture1px.GraphicsDevice, rect.Width, rect.Height);
            texture.SetData(pixels);

            // Stocker dans le cache
            subTextureCache[cacheKey] = texture;

            return texture;
        }


        /// <summary>
        /// Permet d'enregistrer la ressource dans le ServiceLocator.
        /// </summary>
        /// <param name="key">Clé de la ressource à enregistrer dans le ServiceLocator.</param>
        public void Register(Key<ResourceTag> key)
        {
            ServiceLocator.Register(key, this);
        }
        /// <summary>
        /// Décharge toutes les ressources.
        /// </summary>
        public void Unload()
        {
            _contentManager.Unload();
        }
        /// <summary>
        /// Permet de libérer les ressources utilisées par le ResourceManager.
        /// </summary>
        public void Dispose()
        {
            Unload();
            _contentManager.Dispose();
        }

    }
}

