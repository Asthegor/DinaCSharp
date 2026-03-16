#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

namespace DinaCSharp.Services.Levels
{
    public interface ILayer
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        string Parent { get; }
    }
    public interface IProperty
    {
        public string Name { get; }
        public TiledPropertyType Type { get; }
    }
}

#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement