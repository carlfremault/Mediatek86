/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un Livre. Classe fille de la classe LivreDvd
    /// </summary>
    public class Livre : LivreDvd
    {
        /// <summary>
        /// Code ISBN du livre
        /// </summary>
        private readonly string isbn;

        /// <summary>
        /// Auteur du livre
        /// </summary>
        private readonly string auteur;

        /// <summary>
        /// Collection dans laquelle le livre est paru
        /// </summary>
        private readonly string collection;

        /// <summary>
        /// Constructueur, valorise les propriétés de la classe et appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant du livre</param>
        /// <param name="titre">Titre du livre</param>
        /// <param name="image">Image du livre</param>
        /// <param name="isbn">Code ISBN du livre</param>
        /// <param name="auteur">Auteur du livre</param>
        /// <param name="collection">Collection dans laquelle le livre est paru</param>
        /// <param name="idGenre">Identifiant du Genre du livre</param>
        /// <param name="genre">Libellé du Genre du livre</param>
        /// <param name="idPublic">Identifiant du Public du livre</param>
        /// <param name="lePublic">Libellé du Public du livre</param>
        /// <param name="idRayon">Identifiant du Rayon du livre</param>
        /// <param name="rayon">Libellé du Rayon du livre</param>
        public Livre(string id, string titre, string image, string isbn, string auteur, string collection,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.isbn = isbn;
            this.auteur = auteur;
            this.collection = collection;
        }

        /// <summary>
        /// Getter pour le code ISBN du livre
        /// </summary>
        public string Isbn { get => isbn; }

        /// <summary>
        /// Getter pour l'auteur du livre
        /// </summary>
        public string Auteur { get => auteur; }

        /// <summary>
        /// Getter pour la collection dans laquelle le livre est paru
        /// </summary>
        public string Collection { get => collection; }
    }
}
