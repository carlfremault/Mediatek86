using Mediatek86.metier;
using Mediatek86.modele;
using Mediatek86.vue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Serilog;
using Serilog.Formatting.Json;

namespace Mediatek86.controleur
{
    public class Controle
    {
        private List<Livre> lesLivres;
        private List<Dvd> lesDvd;
        private List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;
        private readonly List<Suivi> lesSuivis;

        /// <summary>
        /// Le service dont dépend l'utilisateur connecté
        /// </summary>
        public Service LeService { get; private set; }

       
        /// <summary>
        /// Ouverture de la fenêtre d'authentification
        /// Initialisation des différentes collections
        /// Initialisation de la journalisation
        /// Si l'authentification a réussi, ouverture de l'application
        /// </summary>
        public Controle()
        {
     /*       Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(new JsonFormatter(), "logs/log.txt",
                rollingInterval : RollingInterval.Day)
                .CreateLogger();*/
            lesLivres = Dao.GetAllLivres();
            lesDvd = Dao.GetAllDvd();
            lesRevues = Dao.GetAllRevues();
            lesGenres = Dao.GetAllGenres();
            lesRayons = Dao.GetAllRayons();
            lesPublics = Dao.GetAllPublics();
            lesSuivis = Dao.GetAllSuivis();
            Authentification authentification = new Authentification(this);
            Application.Run(authentification);
            if (authentification.AuthentificationSucces)
            {
                FrmMediatek frmMediatek = new FrmMediatek(this);
                Application.Run(frmMediatek);
            }
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }

        /// <summary>
        /// Getter sur les suivis
        /// </summary>
        /// <returns>Collection d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            return lesSuivis;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return lesLivres;
        }

        /// <summary>
        /// Recupère la liste des livres depuis la bdd
        /// </summary>
        public void RefreshAllLivres()
        {
            lesLivres = Dao.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return lesDvd;
        }

        /// <summary>
        /// Recupère la liste des DVD depuis la bdd
        /// </summary>
        public void RefreshAllDvd()
        {
            lesDvd = Dao.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return lesRevues;
        }

        /// <summary>
        /// Recupère la liste des revues depuis la bdd
        /// </summary>
        public void RefreshAllRevues()
        {
            lesRevues = Dao.GetAllRevues();
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// récupère les commandes d'un livre ou d'un DVD
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandeDocument(string idDocument)
        {
            return Dao.GetCommandeDocument(idDocument);
        }

        /// <summary>
        /// récupère les abonnements d'une revue
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnement(string idDocument)
        {
            return Dao.GetAbonnement(idDocument);
        }

        /// <summary>
        /// récupère les abonnements avec date d'expiration à moins de 30 jours
        /// </summary>
        /// <returns></returns>
        public List<FinAbonnement> GetFinAbonnement()
        {
            return Dao.GetFinAbonnement();
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée un livre dans la bdd
        /// </summary>
        /// <param name="livre">L'objet Livre concerné</param>
        /// <returns>Le message de confirmation ou d'erreur</returns>
        public string CreerLivre(Livre livre)
        {
            return Dao.CreerLivre(livre);
        }

        /// <summary>
        /// Modifie un livre dans la bdd
        /// </summary>
        /// <param name="livre">L'objet Livre concerné</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifLivre(Livre livre)
        {
            return Dao.ModifLivre(livre);
        }

        /// <summary>
        /// Supprime un livre dans la bdd
        /// </summary>
        /// <param name="id">L'id du livre à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprLivre(string id)
        {
            return Dao.SupprLivre(id);
        }

        /// <summary>
        /// Crée une revue dans la bdd
        /// </summary>
        /// <param name="revue">L'objet revue concernée</param>
        /// <returns>Le message de confirmation ou d'erreur</returns>
        public string CreerRevue(Revue revue)
        {
            return Dao.CreerRevue(revue);
        }

        /// <summary>
        /// Modifie une revue dans la bdd
        /// </summary>
        /// <param name="revue">L'objet revue concernée</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifRevue(Revue revue)
        {
            return Dao.ModifRevue(revue);
        }

        /// <summary>
        /// Supprime une revue dans la bdd
        /// </summary>
        /// <param name="id">L'id de la revue à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprRevue(string id)
        {
            return Dao.SupprRevue(id);
        }

        /// <summary>
        /// Crée un DVD dans la bdd
        /// </summary>
        /// <param name="dvd">L'objet DVD concernée</param>
        /// <returns>Le message de confirmation ou d'erreur</returns>
        public string CreerDvd(Dvd dvd)
        {
            return Dao.CreerDvd(dvd);
        }

        /// <summary>
        /// Modifie un DVD dans la bdd
        /// </summary>
        /// <param name="dvd">L'objet DVD concernée</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifDvd(Dvd dvd)
        {
            return Dao.ModifDvd(dvd);
        }

        /// <summary>
        /// Supprime un DVD dans la bdd
        /// </summary>
        /// <param name="id">L'id du DVD à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprDvd(string id)
        {
            return Dao.SupprDvd(id);
        }

        /// <summary>
        /// Crée une CommandeDocument dans la bdd
        /// </summary>
        /// <param name="commandeDocument">L'objet CommandeDocument concerné</param>
        /// <returns>Le message de confirmation ou d'erreur</returns>
        public string CreerCommandeDocument(CommandeDocument commandeDocument)
        {
            return Dao.CreerCommandeDocument(commandeDocument);
        }

        /// <summary>
        /// Supprime une CommandeDocument de la bdd
        /// </summary>
        /// <param name="id">Identifiant de la CommandeDocument à supprimer</param>
        /// <returns>True si la suppression a réussi</returns>
        public bool SupprCommandeDocument(string id)
        {
            return Dao.SupprCommandeDocument(id);
        }

        /// <summary>
        /// Modification d'état de suivi d'une CommandeDocument
        /// </summary>
        /// <param name="idCommandeDocument">identifiant de la CommandeDocument à modifier</param>
        /// <param name="idSuivi">identifiant du nouveau état de suivi</param>
        /// <returns>True si la modification a réussi</returns>
        public bool ModifSuiviCommandeDocument(string idCommandeDocument, int idSuivi)
        {
            return Dao.ModifSuiviCommandeDocument(idCommandeDocument, idSuivi);
        }

        /// <summary>
        /// Crée un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement">L'objet Abonnement concerné</param>
        /// <returns>Le message de confirmation ou d'erreur</returns>
        public string CreerAbonnement(Abonnement abonnement)
        {
            return Dao.CreerAbonnement(abonnement);
        }

        /// <summary>
        /// Récupère les exemplaires rattachés à la revue concerné par un abonnement
        /// puis demande vérification s'ils font partie de l'abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns>True si un exemplaire est rattaché à l'abonnement</returns>
        public bool VerifSuppressionAbonnement(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplaires = GetExemplairesRevue(abonnement.IdRevue);
            bool parution = false;
            foreach (Exemplaire exemplaire in lesExemplaires.Where(ex => ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, ex.DateAchat)))
            {
                parution = true;
            }
            return parution;
        }

        /// <summary>
        /// Vérifie si la dateParution est comprise entre dateCommande et dateFinAbonnement
        /// </summary>
        /// <param name="dateCommande"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="dateParution"></param>
        /// <returns>True si la date est comprise</returns>
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }

        /// <summary>
        /// Demande la suppression d'un abonnement de la bdd
        /// </summary>
        /// <param name="idAbonnement">L'identifiant de l'abonnement à supprimer</param>
        /// <returns>True si l'opération a réussi, sinon false</returns>
        public bool SupprAbonnement(string idAbonnement)
        {
            return Dao.SupprAbonnement(idAbonnement);
        }

        /// <summary>
        /// Calcul du hash MD5 d'une chaîne de caractères
        /// </summary>
        /// <param name="mdp">la chaîne d'entrée</param>
        /// <returns>Le hash calculé</returns>
        public string CreateMD5(string mdp)
        {
            // Utilisation du string mdp pour calculer MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(mdp);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Conversion du vecteur vers un string hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Récupère le service de l'utilisateur qui essaye de se connecter depuis la bdd
        /// Valorise la propriété 'service'
        /// </summary>
        /// <param name="utilisateur">L'identifiant de l'utilisateur</param>
        /// <param name="mdp">Le mot de passe de l'utilisateur</param>
        /// <returns>Le service de l'utilisateur s'il est trouvé dans la bdd, et le mdp est correct. Sinon retourne null</returns>
        public Service Authentification(string utilisateur, string mdp)
        {
            Service service = Dao.Authentification(utilisateur, CreateMD5(mdp));
            LeService = service;
            return service;
        }
    }

}

