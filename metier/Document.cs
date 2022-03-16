/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un document. Classe mère de Revue et LivreDvd
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Identifiant du document
        /// </summary>
        private readonly string id;

        /// <summary>
        /// Titre du document
        /// </summary>
        private readonly string titre;

        /// <summary>
        /// Image du document
        /// </summary>
        private readonly string image;

        /// <summary>
        /// Identifiant du Genre du document
        /// </summary>
        private readonly string idGenre;

        /// <summary>
        /// Libellé du Genre du document
        /// </summary>
        private readonly string genre;

        /// <summary>
        /// Identifiant du Public du document
        /// </summary>
        private readonly string idPublic;

        /// <summary>
        /// Libellé du Public du document
        /// </summary>
        private readonly string lePublic;

        /// <summary>
        /// Identifiant du Rayon du document
        /// </summary>
        private readonly string idRayon;

        /// <summary>
        /// Libellé du Rayon du document
        /// </summary>
        private readonly string rayon;

#pragma warning disable S107 // Methods should not have too many parameters
        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <param name="titre">Titre du document</param>
        /// <param name="image">Image du document</param>
        /// <param name="idGenre">Identifiant du Genre du document</param>
        /// <param name="genre">Libellé du Genre du document</param>
        /// <param name="idPublic">Identifiant du Public du document</param>
        /// <param name="lePublic">Libellé du Public du document</param>
        /// <param name="idRayon">Identifiant du Rayon du document</param>
        /// <param name="rayon">Libellé du Rayon du document</param>
        public Document(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.id = id;
            this.titre = titre;
            this.image = image;
            this.idGenre = idGenre;
            this.genre = genre;
            this.idPublic = idPublic;
            this.lePublic = lePublic;
            this.idRayon = idRayon;
            this.rayon = rayon;
        }

        /// <summary>
        /// Getter pour l'identifiant du document
        /// </summary>
        public string Id { get => id; }

        /// <summary>
        /// Getter pour le titre du document
        /// </summary>
        public string Titre { get => titre; }

        /// <summary>
        /// Getter pour l'image du document
        /// </summary>
        public string Image { get => image; }

        /// <summary>
        /// Getter pour l'identifiant du Genre du document
        /// </summary>
        public string IdGenre { get => idGenre; }

        /// <summary>
        /// Getter pour le libellé du Genre du document
        /// </summary>
        public string Genre { get => genre; }

        /// <summary>
        /// Getter pour l'identifiant du Public du document
        /// </summary>
        public string IdPublic { get => idPublic; }

        /// <summary>
        /// Getter pour le libellé du Public du document
        /// </summary>
        public string Public { get => lePublic; }

        /// <summary>
        /// Getter pour l'identifiant du Rayon du document
        /// </summary>
        public string IdRayon { get => idRayon; }

        /// <summary>
        /// Getter pour le libellé du Rayon du document
        /// </summary>
        public string Rayon { get => rayon; }
    }
}
