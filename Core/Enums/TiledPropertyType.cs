namespace DinaCSharp.Core.Enums
{
    /// <summary>
    /// Types de propriétés personnalisées définies dans Tiled.
    /// </summary>
    public enum TiledPropertyType
    {
        /// <summary>
        /// Valeur booléenne (true/false).
        /// </summary>
        Bool,

        /// <summary>
        /// Couleur au format hexa (#RRGGBB ou #AARRGGBB).
        /// </summary>
        Color,

        /// <summary>
        /// Référence à un fichier externe.
        /// </summary>
        File,

        /// <summary>
        /// Nombre flottant (simple précision).
        /// </summary>
        Float,

        /// <summary>
        /// Nombre entier.
        /// </summary>
        Int,

        /// <summary>
        /// Référence à un objet Tiled.
        /// </summary>
        Object,

        /// <summary>
        /// Chaîne de caractères.
        /// </summary>
        String
    }
}
