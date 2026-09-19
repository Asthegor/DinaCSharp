using DinaCSharp.Services.Keys;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Interface pour l'enregistrement des services.
    /// </summary>
    public interface IRegister
    {
        /// <summary>
        /// Permet d'enregistrer les services.
        /// </summary>
        /// <param name="key">Clé du service à enregistrer.</param>
        public void Register(Key<ServiceTag> key);
    }
}
