#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.

namespace DinaCSharp.Services.Levels
{
    public class TiledProperty<T> : IProperty
    {
        public int ID { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public TiledPropertyType Type { get; internal set; } = TiledPropertyType.String;
        public T Value { get; internal set; }
    }
}

#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement