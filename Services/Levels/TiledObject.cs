#pragma warning disable CA1002 // Ne pas exposer de listes génériques
#pragma warning disable CA2227 // Les propriétés de collection doivent être en lecture seule
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;


namespace DinaCSharp.Services.Levels
{
    public class TiledObject
    {
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public uint GID { get; set; }
        public Rectangle Bounds { get; set; }
        public float Rotation { get; set; }
        public bool Visible { get; set; }
        public TiledObjectType Shape { get; set; } = TiledObjectType.Default;
        public List<IProperty> Properties { get; set; } = [];
        public string Parent { get; internal set; } = string.Empty;
        public int ID { get; internal set; }

        /// <summary>
        /// Permet de récupérer une propriété par son nom
        /// </summary>
        public IProperty? GetProperty(string name)
        {
            foreach (var property in Properties)
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return property;
            }
            return null;
        }
        /// <summary>
        /// Permet de récupérer une propriété par son nom et son type.
        /// </summary>
        public TiledProperty<T>? GetPropertyAs<T>(string name)
        {
            foreach (var property in Properties)
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && property is TiledProperty<T> typedProperty)
                    return typedProperty;
            }
            return null;
        }
        /// <summary>
        /// Permet de recalculer la vraie position de l'objet à l'écran.
        /// L'erreur de calcul provient du fait que Tiled place l'origine des objets utilisant une image (GID != 0) en bas à gauche.
        /// </summary>
        public bool Contains(Point point)
        {
            if (GID == 0)
                return Bounds.Contains(point);

            Point size = new Point(Bounds.Width, Bounds.Height);
            Point location = Bounds.Location - new Point(0, Bounds.Height);
            Rectangle rect = new Rectangle(location, size);
            return rect.Contains(point);
        }
    }
}

#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning restore CA2227 // Les propriétés de collection doivent être en lecture seule
#pragma warning restore CA1002 // Ne pas exposer de listes génériques