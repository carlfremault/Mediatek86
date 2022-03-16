using System;

/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier qui représente une commande d'un document de la classe LivresDvd
    /// </summary>
    public class CommandeDocument : Commande
    {
        /// <summary>
        /// Nombre d'exemplaires commandés
        /// </summary>
        private readonly int nbExemplaires;

        /// <summary>
        /// Identifiant de l'état de suivi de la commande
        /// </summary>
        private readonly int idSuivi;

        /// <summary>
        /// Libellé de l'état de suivi de la commande
        /// </summary>
        private readonly string libelleSuivi;

        /// <summary>
        /// Identifiant du LivreDvd commandé
        /// </summary>
        private readonly string idLivreDvd;

        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant de la commande</param>
        /// <param name="dateCommande">Date de la commande</param>
        /// <param name="montant">Montant de la commande</param>
        /// <param name="nbExemplaires">Nombre d'exemplaires de la commande</param>
        /// <param name="idLivreDvd">Identifiant du LivreDvd commandé</param>
        /// <param name="idSuivi">Identifiant de l'état de suivi de la commande</param>
        /// <param name="libelleSuivi">Libellé de l'état de suivi de la commande</param>
        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaires, string idLivreDvd, int idSuivi, string libelleSuivi) : base(id, dateCommande, montant)
        {
            this.nbExemplaires = nbExemplaires;
            this.idSuivi = idSuivi;
            this.libelleSuivi = libelleSuivi;
            this.idLivreDvd = idLivreDvd;
        }

        /// <summary>
        /// Getter pour le nombre d'exemplaires commandés
        /// </summary>
        public int NbExemplaires { get => nbExemplaires; }

        /// <summary>
        /// Getter pour l'identifiant de l'état de suivi de la commande
        /// </summary>
        public int IdSuivi { get => idSuivi; }

        /// <summary>
        /// Getter pour le libellé de l'état de suivi de la commande
        /// </summary>
        public string LibelleSuivi { get => libelleSuivi; }

        /// <summary>
        /// Getter pour l'identifiant du LivreDvd commandé
        /// </summary>
        public string IdLivreDvd { get => idLivreDvd; }
    }
}
