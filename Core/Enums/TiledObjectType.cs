namespace DinaCSharp.Core.Enums
{

    /// <summary>
    /// Types d’objets géométriques ou textuels présents dans un calque d’objets.
    /// </summary>
    public enum TiledObjectType
    {
        /// <summary>
        /// Objet par défaut (rectangle).
        /// </summary>
        Default,

        /// <summary>
        /// Objet de type ellipse.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Objet de type point (x,y uniquement).
        /// </summary>
        Point,

        /// <summary>
        /// Objet défini par un polygone.
        /// </summary>
        Polygon,

        /// <summary>
        /// Objet textuel (zone de texte).
        /// </summary>
        Text
    }
}
