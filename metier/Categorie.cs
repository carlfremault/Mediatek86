/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant une Categorie. Classe mère de Genre, Public et Rayon
    /// </summary>
    public abstract class Categorie
    {
        /// <summary>
        /// Identifiant de la catégorie
        /// </summary>
        private readonly string id;

        /// <summary>
        /// Libellé de la catégorie
        /// </summary>
        private readonly string libelle;

        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant de la catégorie</param>
        /// <param name="libelle">Libellé de la catégorie</param>
        protected Categorie(string id, string libelle)
        {
            this.id = id;
            this.libelle = libelle;
        }

        /// <summary>
        /// Getter pour l'identifiant de la catégorie
        /// </summary>
        public string Id { get => id; }

        /// <summary>
        /// Getter pour le libellé de la catégorie
        /// </summary>
        public string Libelle { get => libelle; }

        /// <summary>
        /// Récupération du libellé pour l'affichage dans les combos
        /// </summary>
        /// <returns>Le libellé de la catégorie</returns>
        public override string ToString()
        {
            return this.libelle;
        }
    }
}
