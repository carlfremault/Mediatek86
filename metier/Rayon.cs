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
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Rayon(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
