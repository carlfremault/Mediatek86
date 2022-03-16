using System;

/// <summary>
/// Classes métier
/// </summary>
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe métier représentant un fin d'abonnement
    /// </summary>
    public class FinAbonnement
    {
        /// <summary>
        /// Date de fin d'abonnement à une rvue
        /// </summary>
        private readonly DateTime dateFinAbonnement;

        /// <summary>
        /// Identifiant de la revue concernée
        /// </summary>
        private readonly string idRevue;

        /// <summary>
        /// Titre de la revue concernée
        /// </summary>
        private readonly string titreRevue;

        /// <summary>
        /// Constructeur. Valorise les propriétés de la classe
        /// </summary>
        /// <param name="dateFinAbonnement">Date de fin d'abonnement à une revue</param>
        /// <param name="idRevue">Identifiant de la revue concernée</param>
        /// <param name="titreRevue">Titre de la revue concernée</param>
        public FinAbonnement(DateTime dateFinAbonnement, string idRevue, string titreRevue)
        {
            this.dateFinAbonnement = dateFinAbonnement;
            this.idRevue = idRevue;
            this.titreRevue = titreRevue;
        }

        /// <summary>
        /// Getter pour la date de fin d'abonnement
        /// </summary>
        public DateTime DateFinAbonnement => dateFinAbonnement;

        /// <summary>
        /// Getter pour l'identifiant de la revue concernée
        /// </summary>
        public string IdRevue => idRevue;

        /// <summary>
        /// Getter pour le titre de la revue concernée
        /// </summary>
        public string TitreRevue => titreRevue;
    }
}
