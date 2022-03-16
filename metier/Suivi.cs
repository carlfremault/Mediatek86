/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un état de suivi d'une commande
    /// </summary>
    public class Suivi
    {
        /// <summary>
        /// Identifiant de l'état de suivi
        /// </summary>
        private readonly int id;

        /// <summary>
        /// Libellé de l'état de suivi
        /// </summary>
        private readonly string libelle;

        /// <summary>
        /// Constructueur, valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant de l'état de suivi</param>
        /// <param name="libelle">Libellé de l'état de suivi</param>
        public Suivi(int id, string libelle)
        {
            this.id = id;
            this.libelle = libelle;
        }

        /// <summary>
        /// Getter pour l'identifiant de l'état de suivi
        /// </summary>
        public int Id { get => id; }

        /// <summary>
        /// Getter pour le libellé de l'état de suivi
        /// </summary>
        public string Libelle { get => libelle; }
    }
}
