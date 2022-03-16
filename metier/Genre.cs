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
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
