#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable CA1819 // Les propriétés ne doivent pas retourner de tableaux
#pragma warning disable CA1002 // Ne pas exposer de listes génériques
#pragma warning disable CA2227 // Les propriétés de collection doivent être en lecture seule

using System.Collections.Generic;

namespace DinaCSharp.Services.Levels
{
    public class TiledLayer : ILayer
    {
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public float Opacity { get; set; }
        public uint[] Data { get; set; } = [];
        public List<IProperty> Properties { get; set; } = [];
        public bool Visible { get; set; }
        public string Parent { get; internal set; } = string.Empty;
    }
}

#pragma warning restore CA2227 // Les propriétés de collection doivent être en lecture seule
#pragma warning restore CA1002 // Ne pas exposer de listes génériques
#pragma warning restore CA1819 // Les propriétés ne doivent pas retourner de tableaux
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement