/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un DVD. Classe fille de la classe LivreDvd
    /// </summary>
    public class Dvd : LivreDvd
    {
        /// <summary>
        /// Durée du DVD
        /// </summary>
        private readonly int duree;

        /// <summary>
        /// Réalisateur du DVD
        /// </summary>
        private readonly string realisateur;

        /// <summary>
        /// Synopsis du DVD
        /// </summary>
        private readonly string synopsis;

        /// <summary>
        /// Constructueur, valorise les propriétés de la classe et appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du DVD</param>
        /// <param name="titre">Titre du DVD</param>
        /// <param name="image">Image du DVD</param>
        /// <param name="duree">Durée du DVD</param>
        /// <param name="realisateur">Réalisateur du DVD</param>
        /// <param name="synopsis">Synopsis du DVD</param>
        /// <param name="idGenre">Identifiant du Genre du DVD</param>
        /// <param name="genre">Libellé du Genre du DVD</param>
        /// <param name="idPublic">Identifiant du Public du DVD</param>
        /// <param name="lePublic">Libellé du Public du DVD</param>
        /// <param name="idRayon">Identifiant du Rayon du DVD</param>
        /// <param name="rayon">Libellé du Rayon du DVD</param>
        public Dvd(string id, string titre, string image, int duree, string realisateur, string synopsis,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.duree = duree;
            this.realisateur = realisateur;
            this.synopsis = synopsis;
        }

        /// <summary>
        /// Getter pour la durée du DVD
        /// </summary>
        public int Duree { get => duree; }

        /// <summary>
        /// Getter pour le réalisateur du DVD
        /// </summary>
        public string Realisateur { get => realisateur; }

        /// <summary>
        /// Getter pour le synopsis du DVD
        /// </summary>
        public string Synopsis { get => synopsis; }
    }
}
