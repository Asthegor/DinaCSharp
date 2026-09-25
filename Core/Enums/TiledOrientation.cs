namespace DinaCSharp.Core.Enums
{
    /// <summary>
    /// Définit l’orientation de la carte Tiled.
    /// </summary>
    public enum TiledOrientation
    {
        /// <summary>
        /// Carte orthogonale (grille carrée classique).
        /// </summary>
        Orthogonal,

        /// <summary>
        /// Carte isométrique (vue en diagonale type 45°).
        /// </summary>
        Isometric,

        /// <summary>
        /// Carte en losange décalé (staggered).
        /// </summary>
        Stagged,

        /// <summary>
        /// Carte hexagonale.
        /// </summary>
        Hexagonal
    }
}
