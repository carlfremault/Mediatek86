/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un service dont peut dépendre un utilisateur
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Identifiant du service
        /// </summary>
        private readonly int id;

        /// <summary>
        /// Libellé du service
        /// </summary>
        private readonly string libelle;

        /// <summary>
        /// Constructueur, valorise les propriétés de la class
        /// </summary>
        /// <param name="id">Identifiant du service</param>
        /// <param name="libelle">Libellé du service</param>
        public Service(int id, string libelle)
        {
            this.id = id;
            this.libelle = libelle;
        }

        /// <summary>
        /// Getter pour l'identifiant du service
        /// </summary>
        public int Id => id;

        /// <summary>
        /// Getter pour le libellé du service
        /// </summary>
        public string Libelle => libelle;
    }
}
