using DinaCSharp.Services.Keys;
using DinaCSharp.Services.Logs;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;

namespace DinaCSharp.Services.Localization
{
    /// <summary>
    /// Gère la traduction des chaînes de texte dans l'application, permettant de récupérer des traductions pour différentes langues.
    /// </summary>
    public static class LocalizationManager
    {
        private static readonly Dictionary<string, string> _cache = [];
        private static readonly List<ResourceManager> _resourceManagers = [];
        private static readonly List<Assembly> _assemblies = [];
        private static string[] _availableLanguages = [];
        private static bool _loaded;
        private static bool _updated;
        private static readonly HashSet<string> _reportedSatellites = [];
        private static readonly HashSet<string> _reportedMissingNeutralLanguage = [];
        private static string? _localizationFolderName;
        private static bool _resolverRegistered;

        //private static readonly List<Type> _listStrings = [];

        private static CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// Vérifie si les valeurs de traduction ont été chargées.
        /// </summary>
        public static bool IsLoaded => _loaded;

        /// <summary>
        /// Vérifie si les traductions ont été mises à jour.
        /// </summary>
        public static bool IsUpdated => _updated;

        /// <summary>
        /// Obtient ou définit la langue actuelle.
        /// </summary>
        public static string CurrentLanguage
        {
            get => _currentCulture.TwoLetterISOLanguageName;
            set
            {
                _currentCulture = new CultureInfo(value);
                _updated = true;
                _cache.Clear();
            }
        }

        /// <summary>
        /// Nom par défaut du dossier contenant les assemblys satellites.
        /// </summary>
        public const string DefaultLocalizationFolderName = "Localization";

        /// <summary>
        /// Clé de l'attribut <see cref="AssemblyMetadataAttribute"/> lu dans l'assembly d'entrée (l'exécutable du jeu)
        /// pour connaître le nom du dossier des assemblys satellites.
        /// Cet attribut est généré par le fichier PostBuildLocalization.targets à partir de la propriété MSBuild <c>LocalizationFolderName</c>.
        /// </summary>
        public const string LocalizationFolderMetadataKey = "DinaCSharp.LocalizationFolderName";

        /// <summary>
        /// Obtient le nom du dossier contenant les assemblys satellites (un sous-dossier par culture),
        /// relatif au dossier de l'exécutable.
        /// La valeur provient de l'attribut <see cref="LocalizationFolderMetadataKey"/> de l'assembly d'entrée ;
        /// à défaut, <see cref="DefaultLocalizationFolderName"/> est utilisé.
        /// </summary>
        public static string LocalizationFolderName => _localizationFolderName ??= ReadLocalizationFolderName();

        private static string ReadLocalizationFolderName()
        {
            string? value = Assembly.GetEntryAssembly()?
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(a => a.Key == LocalizationFolderMetadataKey)?
                .Value;
            return string.IsNullOrWhiteSpace(value) ? DefaultLocalizationFolderName : value;
        }

        private static void RegisterSatelliteResolver()
        {
            if (_resolverRegistered)
                return;
            AssemblyLoadContext.Default.Resolving += ResolveSatelliteAssembly;
            _resolverRegistered = true;
        }

        private static Assembly? ResolveSatelliteAssembly(AssemblyLoadContext context, AssemblyName name)
        {
            if (string.IsNullOrEmpty(name.CultureName) || name.Name is null
                || !name.Name.EndsWith(".resources", StringComparison.Ordinal))
                return null;

            string path = Path.Combine(AppContext.BaseDirectory, LocalizationFolderName, name.CultureName, name.Name + ".dll");
            return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
        }

        /// <summary>
        /// Ajoute une classe contenant des traductions.
        /// </summary>
        /// <param name="resourceClass">La classe contenant les traductions à ajouter.</param>
        public static void Register(Type resourceClass)
        {
            ArgumentNullException.ThrowIfNull(resourceClass);

            RegisterSatelliteResolver();

            var prop = resourceClass.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ?? throw new InvalidOperationException($"La classe {resourceClass.Name} ne contient pas de ResourceManager.");
            var rm = prop.GetValue(null) as ResourceManager
                ?? throw new InvalidOperationException($"La classe {resourceClass.Name} ne fournit pas de ResourceManager valide.");
            _resourceManagers.Add(rm);

            _assemblies.Add(resourceClass.Assembly);
            _loaded = true;
            _availableLanguages = SearchAvailableLanguages();
        }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans la langue actuelle.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="values">Liste des </param>
        /// <returns>La traduction correspondant à la clé, ou la clé elle-même si non trouvée.</returns>
        public static string GetTranslation(string key, params string[] values)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (values?.Length > 0)
                    return string.Format(CultureInfo.InvariantCulture, cached, values);
                return cached;
            }

            foreach (var rm in _resourceManagers)
            {
                try
                {
                    var translation = rm.GetString(key, _currentCulture);
                    if (!string.IsNullOrEmpty(translation))
                    {
                        _cache[key] = translation;
                        if (values?.Length > 0)
                            return string.Format(CultureInfo.InvariantCulture, translation, values);
                        return translation;
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // Ignore cette exception si la ressource n'est pas trouvée
                }
            }
            return key;
        }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans une culture spécifique.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="culture">Le code de la culture.</param>
        /// <returns>La traduction pour la culture spécifiée.</returns>
        public static string GetTranslationForCulture(string key, string culture)
        {
            var previous = _currentCulture;
            try
            {
                _currentCulture = new CultureInfo(culture);
                // The cache is keyed by key only: it must be emptied, otherwise the text of the
                // previously requested language is returned for every culture.
                _cache.Clear();
                return GetTranslation(key);
            }
            finally
            {
                _currentCulture = previous;
                _cache.Clear();
            }
        }

        /// <summary>
        /// Récupère la longueur maximale d'une traduction pour une clé donnée en utilisant une police.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="font">La police utilisée pour mesurer la longueur.</param>
        /// <returns>La longueur maximale sous forme de Vector2.</returns>
        public static Vector2 GetTranslationMaxLength(string key, SpriteFont font)
        {
            ArgumentNullException.ThrowIfNull(font);

            var maxLength = Vector2.Zero;
            foreach (var language in GetAvailableLanguages())
            {
                var translation = GetTranslationForCulture(key, language);
                var size = font.MeasureString(translation);

                maxLength = new Vector2(
                    Math.Max(maxLength.X, size.X),
                    Math.Max(maxLength.Y, size.Y)
                );
            }

            return maxLength;
        }

        /// <summary>
        /// Récupère la liste des cultures disponibles via les assemblies satellites.
        /// </summary>
        /// <returns>Un tableau des langues disponibles.</returns>
        public static string[] GetAvailableLanguages() => _availableLanguages ?? [];

        private static string[] SearchAvailableLanguages()
        {
            var cultures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < _assemblies.Count; i++)
            {
                var assembly = _assemblies[i];
                AddNeutralLanguage(assembly, _resourceManagers[i], cultures);

                var dir = Path.GetDirectoryName(assembly.Location);
                if (dir != null)
                    AddCulturesFromSubdirectories(dir, dir, cultures, assembly);
            }
            return [.. cultures];
        }

        /// <summary>
        /// Ajoute la langue par défaut de l'assembly : ses ressources neutres sont intégrées à l'assembly
        /// elle-même et sont donc toujours chargées. Le nom de la langue provient de l'attribut
        /// <see cref="NeutralResourcesLanguageAttribute"/> (propriété MSBuild <c>NeutralLanguage</c> du projet).
        /// </summary>
        private static void AddNeutralLanguage(Assembly assembly, ResourceManager resourceManager, HashSet<string> cultures)
        {
            if (resourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: false) == null)
                return;

            string? neutralLanguage = assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>()?.CultureName;
            if (string.IsNullOrWhiteSpace(neutralLanguage))
            {
                if (_reportedMissingNeutralLanguage.Add(assembly.GetName().Name ?? assembly.FullName ?? string.Empty))
                    ServiceLocator.Get<LogManager>(DinaServiceKeys.LogManager)?.Warning(
                        $"La langue par défaut de l'assembly '{assembly.GetName().Name}' est inconnue : " +
                        "ajoutez <NeutralLanguage> dans son fichier projet pour qu'elle apparaisse dans les langues disponibles.");
                return;
            }
            cultures.Add(neutralLanguage);
        }

        private static void AddCulturesFromSubdirectories(string assemblyDirectory, string directoryPath, HashSet<string> cultures, Assembly assembly)
        {
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                var cultureName = Path.GetFileName(directory);
                if (CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Any(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase)))
                {
                    var satellitePath = Path.Combine(directory, $"{assembly.GetName().Name}.resources.dll");
                    if (File.Exists(satellitePath) && IsSatelliteReachable(assemblyDirectory, directory, satellitePath))
                        cultures.Add(cultureName);
                }
                AddCulturesFromSubdirectories(assemblyDirectory, directory, cultures, assembly);
            }
        }

        /// <summary>
        /// Indique si l'assembly satellite sera chargée : elle doit se trouver dans le dossier de l'assembly
        /// (&lt;dossier&gt;\&lt;culture&gt;) ou dans le dossier <see cref="LocalizationFolderName"/>.
        /// Sinon, un avertissement est envoyé (une seule fois par fichier) au <see cref="LogManager"/>
        /// enregistré sous la clé <see cref="DinaServiceKeys.LogManager"/>, s'il existe.
        /// </summary>
        private static bool IsSatelliteReachable(string assemblyDirectory, string cultureDirectory, string satellitePath)
        {
            string? parent = Path.GetDirectoryName(cultureDirectory);
            if (parent == null)
                return false;

            string localizationDirectory = Path.Combine(AppContext.BaseDirectory, LocalizationFolderName);
            if (IsSameDirectory(parent, assemblyDirectory) || IsSameDirectory(parent, localizationDirectory))
                return true;

            if (!_reportedSatellites.Add(Path.GetFullPath(satellitePath)))
                return false;

            ServiceLocator.Get<LogManager>(DinaServiceKeys.LogManager)?.Warning(
                $"L'assembly satellite '{satellitePath}' ne sera pas chargée : elle doit se trouver dans " +
                $"'{Path.Combine(assemblyDirectory, "<culture>")}' ou dans '{Path.Combine(localizationDirectory, "<culture>")}'.");
            return false;
        }

        private static bool IsSameDirectory(string path1, string path2)
        {
            StringComparison comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return string.Equals(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(path1)),
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(path2)),
                comparison);
        }

    }
}
