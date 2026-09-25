using System;

namespace DinaCSharp.Helpers
{
    /// <summary>
    /// Fonctions utilitaires pour la manipulation de chaînes.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Extrait la partie de <paramref name="value"/> située avant <paramref name="sep"/>
        /// et retire cette partie (séparateur inclus) de <paramref name="value"/>.
        /// Si le séparateur est absent, retourne toute la chaîne et vide <paramref name="value"/>.
        /// </summary>
        public static string ExtractValue(ref string value, string sep)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(sep))
                return value;

            int posSep = value.IndexOf(sep, StringComparison.Ordinal);
            if (posSep < 0)
            {
                string res = value;
                value = string.Empty;
                return res;
            }

            string extracted = value[..posSep];
            value = value[(posSep + sep.Length)..];
            return extracted;
        }

        /// <summary>
        /// Indique si au moins un caractère de <paramref name="str"/> se trouve hors de toutes les plages fournies.
        /// </summary>
        public static bool AsciiCharOutOfRange(string str, params (int min, int max)[] ranges)
        {
            if (string.IsNullOrEmpty(str) || ranges == null || ranges.Length == 0)
                return false;

            foreach (char c in str)
            {
                bool inRange = false;
                foreach (var (min, max) in ranges)
                {
                    if (c >= min && c <= max)
                    {
                        inRange = true;
                        break;
                    }
                }
                if (!inRange)
                    return true;
            }
            return false;
        }
    }
}