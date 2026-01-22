#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable CA1002 // Ne pas exposer de listes génériques
#pragma warning disable CA2227 // Les propriétés de collection doivent être en lecture seule


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaCSharp.Services.Levels
{
    public class TiledTileset
    {
        public int FirstGid { get; internal set; } = 1;
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int TileWidth { get; internal set; }
        public int TileHeight { get; internal set; }
        public int Spacing { get; internal set; }
        public int Margin { get; internal set; }
        public int TileCount { get; internal set; }
        public int Columns { get; internal set; }
        public bool Visible { get; internal set; }
        public Vector2 TileOffset { get; internal set; }
        public bool HorizontalFlip { get; internal set; }
        public bool VerticalFlip { get; internal set; }
        public bool Rotate { get; internal set; }
        public bool PreferUntransformed { get; internal set; }
        public Color Transparency { get; internal set; } = Color.Transparent;
        public Texture2D Image { get; internal set; } = default!;
        public Dictionary<int, Rectangle> Quads { get; internal set; } = [];
        public List<IProperty> Properties { get; set; } = [];

        public Rectangle GetTile(int tileid)
        {
            return Quads[tileid];
        }
    }
}
#pragma warning restore CA2227 // Les propriétés de collection doivent être en lecture seule
#pragma warning restore CA1002 // Ne pas exposer de listes génériques
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
