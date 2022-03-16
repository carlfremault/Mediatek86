/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un Public. Classe fille de la classe Categorie
    /// </summary>
    public class Public : Categorie
    {
        /// <summary>
        /// Constructueur,  appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du Public</param>
        /// <param name="libelle">Libellé du Public</param>
        public Public(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
