/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant l'état d'usure d'un exemplaire d'une revue
    /// </summary>
    public class Etat
    {
        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant de l'état d'usure</param>
        /// <param name="libelle">Libellé de l'état d'usure</param>
        public Etat(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        /// <summary>
        /// Identifiant de l'état d'usure. Getter et Setter
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Libellé de l'état d'usure. Getter et Setter
        /// </summary>
        public string Libelle { get; set; }
    }
}
