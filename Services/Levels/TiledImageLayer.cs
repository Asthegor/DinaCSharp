#pragma warning disable CA2227 // Les propriétés de collection doivent être en lecture seule
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
#pragma warning disable CA1002 // Ne pas exposer de listes génériques

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaCSharp.Services.Levels
{
    public class TiledImageLayer : ILayer
    {
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public float Opacity { get; set; } = 1.0f;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public Texture2D Texture { get; set; }
        public Color Transparency { get; set; }
        public List<IProperty> Properties { get; set; } = [];
        public bool Visible { get; set; }
        public string Parent { get; internal set; } = string.Empty;
    }
}

#pragma warning restore CA1002 // Ne pas exposer de listes génériques
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning restore CA2227 // Les propriétés de collection doivent être en lecture seule