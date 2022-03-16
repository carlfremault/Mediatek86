/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant une Revue, classe fille de la classe mère Document
    /// </summary>
    public class Revue : Document
    {
        /// <summary>
        /// Constructueur, valorise les propriétés de la classe et appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant de la revue</param>
        /// <param name="titre">Titre de la revue</param>
        /// <param name="image">Image de la revue</param>
        /// <param name="idGenre">Identifiant du Genre de la revue</param>
        /// <param name="genre">Libellé du Genre de la revue</param>
        /// <param name="idPublic">Identifiant du Public de la revue</param>
        /// <param name="lePublic">Libellé du Public de la revue</param>
        /// <param name="idRayon">Identifiant du Rayon de la revue</param>
        /// <param name="rayon">Libellé du Rayon de la revue</param>
        /// <param name="empruntable">Booléen qui indique si la revue est empruntable (true) ou non (false)</param>
        /// <param name="periodicite">Périodicité de la revue</param>
        /// <param name="delaiMiseADispo">Délai de mise à disposition de la revue</param>
        public Revue(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon,
            bool empruntable, string periodicite, int delaiMiseADispo)
             : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            Periodicite = periodicite;
            Empruntable = empruntable;
            DelaiMiseADispo = delaiMiseADispo;
        }

        /// <summary>
        /// Périodicité de la revue. Getter et Setter
        /// </summary>
        public string Periodicite { get; set; }

        /// <summary>
        /// Booléen qui indique si la revue est empruntable (true) ou non (false). Getter et Setter
        /// </summary>
        public bool Empruntable { get; set; }

        /// <summary>
        /// Délai de mise à disposition de la revue. Getter et Setter
        /// </summary>
        public int DelaiMiseADispo { get; set; }
    }
}
