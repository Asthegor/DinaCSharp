using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DinaCSharp.Services.Keys
{
    /// <summary>
    /// Classe de base générique permettant d'enregistrer et de résoudre dynamiquement 
    /// des identifiants typés <see cref="Key{TTag}"/> par leur nom via la réflexion.
    /// </summary>
    /// <typeparam name="TDerived">Le type de la classe dérivée (pattern CRTP).</typeparam>
    /// <typeparam name="TTag">Le type de marqueur (tag) associé aux clés manipulées.</typeparam>
    public abstract class KeyResolver<TDerived, TTag>
        where TDerived : class
        where TTag : class
    {
        private static readonly Dictionary<string, Key<TTag>> _keysByName = [];

        /// <summary>
        /// Initialise le dictionnaire statique lors du premier accès à la classe dérivée 
        /// en scannant tous ses champs publics statiques du type <see cref="Key{TTag}"/>.
        /// </summary>
        static KeyResolver()
        {
            Type targetType = typeof(Key<TTag>);
            FieldInfo[] fields = typeof(TDerived).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == targetType)
                {
                    var value = (Key<TTag>)field.GetValue(null)!;
                    _keysByName[field.Name] = value;
                }
            }
        }
        /// <summary>
        /// Recherche et retourne l'instance de <see cref="Key{TTag}"/> correspondant au nom donné.
        /// </summary>
        /// <param name="key">Le nom exact du champ statique définissant la clé.</param>
        /// <returns>La clé typée <see cref="Key{TTag}"/> associée au nom fourni.</returns>
        /// <exception cref="ArgumentException">
        /// Levée si aucune clé correspondant à <paramref name="key"/> n'a été trouvée dans le registre.
        /// </exception>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", 
        Justification = "Utilisé via le pattern CRTP. Les classes dérivées fournissent le contexte de type sans syntaxe lourde.")]
        public static Key<TTag> GetKeyFromString(string key)
        {
            if (_keysByName.TryGetValue(key, out var resourceKey))
            {
                return resourceKey;
            }

            throw new ArgumentException($"La clé '{key}' n'existe pas dans {typeof(TDerived).Name}.", nameof(key));
        }
    }
}
