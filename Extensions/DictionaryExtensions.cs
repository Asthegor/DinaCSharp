using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DinaCSharp.Extensions
{
    /// <summary>
    /// Fournit des méthodes d'extension pour convertir entre différents types de données.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Compares two dictionaries and returns a list of keys whose values differ between them.
        /// </summary>
        /// <remarks>This method performs a key-by-key comparison between the two dictionaries. A key is
        /// considered "modified" if it exists in both dictionaries  but the values associated with the key are not
        /// equal. Equality is determined using the <see cref="object.Equals(object, object)"/> method.</remarks>
        /// <param name="sourceDictionary">The source dictionary to compare.</param>
        /// <param name="otherDictionary">The dictionary to compare against the source dictionary.</param>
        /// <returns>A list of keys from the source dictionary whose values differ from the corresponding values in the other
        /// dictionary.  If no values differ, the list will be empty.</returns>
        public static Collection<string> GetModifiedKeys(this Dictionary<string, object> sourceDictionary, Dictionary<string, object> otherDictionary)
        {
            ArgumentNullException.ThrowIfNull(sourceDictionary);
            ArgumentNullException.ThrowIfNull(otherDictionary);
            if (otherDictionary.Count == 0)
                return [];

            List<string> modifiedKeys = [];

            foreach (KeyValuePair<string, object> kvp in sourceDictionary)
            {
                if (otherDictionary.ContainsKey(kvp.Key)) // Vérifie si la clé existe dans l'autre dictionnaire
                {
                    if (!Equals(kvp.Value, otherDictionary[kvp.Key]))
                    {
                        modifiedKeys.Add(kvp.Key);
                    }
                }
            }
            return [.. modifiedKeys];
        }
    }
}
