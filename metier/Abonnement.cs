using System;

/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un Abonnement. Classe fille de la classe Commande
    /// </summary>
    public class Abonnement : Commande
    {
        /// <summary>
        /// Date de fin d'abonnement
        /// </summary>
        private readonly DateTime dateFinAbonnement;

        /// <summary>
        /// Identifiant de la revue concernée par l'abonnement
        /// </summary>
        private readonly string idRevue;

        /// <summary>
        /// Constructueur, valorise les propriétés de la classe et appelle le constructeur de la classe mère
        /// </summary>
        /// <param name="id">Identifiant de l'abonnement</param>
        /// <param name="dateCommande">Date de commande de l'abonnement</param>
        /// <param name="montant">Montant de l'abonnement</param>
        /// <param name="dateFinAbonnement">Date de fin d'abonnement</param>
        /// <param name="idRevue">Identifiant de la revue concernée par l'abonnement</param>
        public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue) : base(id, dateCommande, montant)
        {
            this.dateFinAbonnement = dateFinAbonnement;
            this.idRevue = idRevue;
        }

        /// <summary>
        /// Getter pour la date de fin d'abonnement
        /// </summary>
        public DateTime DateFinAbonnement { get => dateFinAbonnement; }

        /// <summary>
        /// Getter pour l'identifiant de la revue concernée par l'abonnement
        /// </summary>
        public string IdRevue { get => idRevue; }
    }
}
