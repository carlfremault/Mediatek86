using System;

/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier qui représente une commande. Classe mère de Abonnement et CommandeDocument
    /// </summary>
    public class Commande
    {
        /// <summary>
        /// Identifiant de la commande
        /// </summary>
        private readonly string id;

        /// <summary>
        /// Date de la commande
        /// </summary>
        private readonly DateTime dateCommande;

        /// <summary>
        /// Montant de la commande
        /// </summary>
        private readonly Double montant;

        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="id">Identifiant de la commande</param>
        /// <param name="dateCommande">Date de la commande</param>
        /// <param name="montant">Montant de la commande</param>
        public Commande(string id, DateTime dateCommande, double montant)
        {
            this.id = id;
            this.dateCommande = dateCommande;
            this.montant = montant;
        }

        /// <summary>
        /// Getter pour l'identifiant de la commande
        /// </summary>
        public string Id { get => id; }

        /// <summary>
        /// Getter pour la date de la commande
        /// </summary>
        public DateTime DateCommande { get => dateCommande; }

        /// <summary>
        /// Getter pour le montant de la commande
        /// </summary>
        public Double Montant { get => montant; }
    }
}
