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
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Public(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
