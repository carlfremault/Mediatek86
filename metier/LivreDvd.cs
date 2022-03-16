/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un LivreDvd. Classe fille de la classe Document
    /// </summary>
    public abstract class LivreDvd : Document
    {
        /// <summary>
        /// Constructueur, appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du Document</param>
        /// <param name="titre">Titre du Document</param>
        /// <param name="image">Image du Document</param>
        /// <param name="idGenre">Identifiant du Genre du Document</param>
        /// <param name="genre">Libellé du Genre du Document</param>
        /// <param name="idPublic">Identifiant du Public du Document</param>
        /// <param name="lePublic">Libellé du Public du Document</param>
        /// <param name="idRayon">Identifiant du Rayon du Document</param>
        /// <param name="rayon">Libellé du Rayon du Document</param>
        protected LivreDvd(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
        }
    }
}

