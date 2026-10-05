using DinaCSharp.Graphics.Primitives;

using System;

namespace DinaCSharp.Core.Events
{
    /// <summary>
    /// Fournit des données pour les événements liés au Polygon.
    /// </summary>
    public class PolygonEventArgs(Polygon polygon) : EventArgs
    {
        /// <summary>
        /// Obtient le Polygon associé à l'événement.
        /// </summary>
        public Polygon Polygon { get; } = polygon;
    }
}
