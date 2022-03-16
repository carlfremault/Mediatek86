using System;

/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un exemplaire d'une revue
    /// </summary>
    public class Exemplaire
    {
        /// <summary>
        /// Constructeur, valorise les propriétés de la classe
        /// </summary>
        /// <param name="numero">Numéro de l'exemplaire</param>
        /// <param name="dateAchat">Date d'achat de l'exemplaire</param>
        /// <param name="photo">Photo de l'exemplaire</param>
        /// <param name="idEtat">Identifiant de l'état d'usure de l'exemplaire</param>
        /// <param name="idDocument">Identifiant de la revue dont l'exemplaire est un exemplaire</param>
        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.IdDocument = idDocument;
        }

        /// <summary>
        /// Numéro de l'exemplaire. Getter et Setter
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Photo de l'exemplaire. Getter et Setter
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Date d'achat de l'exemplaire. Getter et Setter
        /// </summary>
        public DateTime DateAchat { get; set; }

        /// <summary>
        /// Identifiant de l'état d'usure de l'exemplaire. Getter et Setter
        /// </summary>
        public string IdEtat { get; set; }

        /// <summary>
        /// Identifiant de la revue dont l'exemplaire est un exemplaire. Getter et Setter
        /// </summary>
        public string IdDocument { get; set; }
    }
}
