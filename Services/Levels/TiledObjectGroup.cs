#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

using System.Collections.Generic;

namespace DinaCSharp.Services.Levels
{
    public class TiledObjectGroup : ILayer
    {
        private readonly List<TiledObject> _objects = [];

        public string Name { get; set; } = string.Empty;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                foreach (var obj in _objects)
                    obj.Visible = value;
            }
        }
        private bool _visible;
        public float Opacity { get; set; }
        public IReadOnlyList<TiledObject> Objects => _objects;

        public string Parent { get; internal set; } = string.Empty;

        public void AddObject(TiledObject obj)
        {
            _objects.Add(obj);
        }
    }
}

#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement