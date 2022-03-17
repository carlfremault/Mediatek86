using Mediatek86.controleur;
using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Les vues de l'application
/// </summary>
namespace Mediatek86.vue
{
    /// <summary>
    /// Vue principale de l'application.
    /// </summary>
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        /// <summary>
        /// Instance du contrôleur
        /// </summary>
        private readonly Controle controle;

        /// <summary>
        /// L'état initial d'un document
        /// </summary>
        const string ETATNEUF = "00001";

        /// <summary>
        /// Chemin initial lors de la recherche d'un image sur son disque dur
        /// </summary>
        const string DOSSIERINITIALRECHERCHEIMAGE = "c:\\MediatekImages";

        /// <summary>
        /// BindingSource pour le DataGridView des livres
        /// </summary>
        private readonly BindingSource bdgLivresListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des DVD
        /// </summary>
        private readonly BindingSource bdgDvdListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des Revues
        /// </summary>
        private readonly BindingSource bdgRevuesListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Genres dans le recherche des documents
        /// </summary>
        private readonly BindingSource bdgGenres = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Publics dans le recherche des documents
        /// </summary>
        private readonly BindingSource bdgPublics = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Rayons dans le recherche des documents
        /// </summary>
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Genres dans les infos détaillées des documents
        /// </summary>
        private readonly BindingSource bdgInfosGenres = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Publics dans les infos détaillées des documents
        /// </summary>
        private readonly BindingSource bdgInfosPublics = new BindingSource();

        /// <summary>
        /// BindingSource pour le Combobox des Rayons dans les infos détaillées des documents
        /// </summary>
        private readonly BindingSource bdgInfosRayons = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des Exemplaires
        /// </summary>
        private readonly BindingSource bdgExemplairesListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des Commandes de livres
        /// </summary>
        private readonly BindingSource bdgCommandesLivresListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des Commandes de DDV
        /// </summary>
        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();

        /// <summary>
        /// BindingSource pour le DataGridView des Abonnements des revues
        /// </summary>
        private readonly BindingSource bdgAbonnementRevueListe = new BindingSource();

        /// <summary>
        /// Collection des livres présents dans la bdd
        /// </summary>
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Collection des DVD présents dans la bdd
        /// </summary>
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Collection des revues présentes dans la bdd
        /// </summary>
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Collection des Exemplaires de revue présents dans la bdd
        /// </summary>
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();

        /// <summary>
        /// Collection des CommandeDocument présentes dans la bdd
        /// </summary>
        private List<CommandeDocument> lesCommandeDocument = new List<CommandeDocument>();

        /// <summary>
        /// Collection des Abonnements présents dans la bdd
        /// </summary>
        private List<Abonnement> lesAbonnements = new List<Abonnement>();

        /// <summary>
        /// Collection des différents états de suivi des commandes
        /// </summary>
        private List<Suivi> lesSuivis = new List<Suivi>();

        #endregion

        /// <summary>
        /// Constructeur. Rend invisible certaines fonctionnalités en cas de connection
        /// d'un utilisateur du service "prêt" : 
        /// - Onglets de gestion des commandes livres et DVD, et des abonnements Revue
        /// - GroupBoxes permettant de saisir la réception d'exemplaires et l'ajout/modification/suppression de livres, DVD et revues
        /// </summary>
        /// <param name="controle">Le contrôleur</param>
        public FrmMediatek(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
            if (controle.LeService.Libelle == "prêt")
            {                
                tabOngletsApplication.TabPages.Remove(tabCommandeLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandeDVD);
                tabOngletsApplication.TabPages.Remove(tabAbonnementRevue);
                grpReceptionExemplaire.Visible = false;
                grpGestionLivres.Visible = false;
                grpGestionDVD.Visible = false;
                grpGestionRevues.Visible = false;
                btnEnregistrerLivre.Visible = false;
                btnEnregistrerDvd.Visible = false;
                btnEnregistrerRevue.Visible = false;
                btnAnnulerSaisieLivre.Visible = false;
                btnAnnulerSaisieDvd.Visible = false;
                btnAnnulerSaisieRevue.Visible = false;
            }
        }

        /// <summary>
        /// Dès l'ouverture de l'application la vue d'alerte de fin d'abonnements s'ouvre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Shown(object sender, EventArgs e)
        {
            if (controle.LeService.Libelle != "prêt")
            {
                FrmAlerteFinAbonnements alerteFinAbonnements = new FrmAlerteFinAbonnements(controle)
                {
                    StartPosition = FormStartPosition.CenterParent
                };
                alerteFinAbonnements.ShowDialog();
            }
        }

        #region modules communs

        /// <summary>
        /// Remplit un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">La liste qui remplit le combobox</param>
        /// <param name="bdg">La BindingSource du combobox concerné</param>
        /// <param name="cbx">Le Combobox concerné</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Affichage d'un MessageBox pour vérifier si l'utilisateur veut bien abandonner sa saisie
        /// </summary>
        /// <returns>True si abandon saisie confirmé, sinon false</returns>
        private bool VerifAbandonSaisie()
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir abandonner votre saisie ?", "Confirmation d'abandon de saisie", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de suppression d'un document
        /// </summary>
        /// <param name="titre">Le titre du document concerné</param>
        /// <returns>True si suppression confirmée, sinon false</returns>
        private bool ValidationSuppression(string titre)
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir supprimer '" + titre + "' ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de suppression d'un abonnement
        /// </summary>
        /// <returns>True si suppression confirmée, sinon false</returns>
        private bool ValidationSuppressionAbonnement()
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir supprimer cet abonnement ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de suppression d'une commande
        /// </summary>
        /// <returns>True si suppression confirmée, sinon false</returns>
        private bool ValidationSuppressionCommande()
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir supprimer cette commande ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de changement d'état de suivi
        /// </summary>
        /// <param name="libelleSuivi">Le nouvel état de suivi</param>
        /// <returns>True si changement confirmée, sinon false</returns>
        private bool ValidationModifEtatSuivi(string libelleSuivi)
        {
            return (MessageBox.Show("Confirmez-vous le passage de cette commande en l'état '" + libelleSuivi + "' ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Mets tous les booléens concernant saisies et modifications en 'false'
        /// </summary>
        private void CancelAllSaisies()
        {
            modifDvd = false;
            modifLivre = false;
            modifRevue = false;
            saisieDvd = false;
            saisieLivre = false;
            saisieRevue = false;
            saisieCommandeLivres = false;
            saisieCommandeDvd = false;
            saisieAbonnementRevue = false;
        }

        /// <summary>
        /// Evénement de changement d'onglet. Vérifie si une saisie est en cours
        /// Si oui, demande validation de l'utilisateur avant d'abandonner saisie et changer d'onglet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOngletsApplication_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if ((saisieLivre || saisieDvd || saisieRevue || saisieCommandeLivres || saisieCommandeDvd || saisieAbonnementRevue) && !VerifAbandonSaisie())
            {
                e.Cancel = true;
            }
            else
            {
                CancelAllSaisies();
            }
        }

        /// <summary>
        /// Tri sur une colonne pour les listes CommandeDocument
        /// </summary>
        /// <param name="titreColonne">Le titre de la colonne concernée</param>
        /// <returns>La liste triée</returns>
        private List<CommandeDocument> SortCommandeDocumentList(string titreColonne)
        {
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date":
                    sortedList = lesCommandeDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandeDocument.OrderBy(o => o.Montant).Reverse().ToList();
                    break;
                case "Exemplaires":
                    sortedList = lesCommandeDocument.OrderBy(o => o.NbExemplaires).Reverse().ToList();
                    break;
                case "Etat":
                    sortedList = lesCommandeDocument.OrderBy(o => o.IdSuivi).ToList();
                    break;
            }
            return sortedList;
        }

        #endregion
    }
}
