/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un Rayon. Classe fille de la classe Categorie
    /// </summary>
    public class Rayon : Categorie
    {

        /// <summary>
        /// Constructueur,  appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du Rayon</param>
        /// <param name="libelle">Libellé du Rayon</param>
        public Rayon(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
