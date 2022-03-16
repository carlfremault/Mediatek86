/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un Genre. Classe fille de la classe Categorie
    /// </summary>
    public class Genre : Categorie
    {
        /// <summary>
        /// Constructueur,  appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du Genre</param>
        /// <param name="libelle">Libellé du Genre</param>
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
