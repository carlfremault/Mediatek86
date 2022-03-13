using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.metier;
using Mediatek86.controleur;
using System.Drawing;
using System.Linq;

namespace Mediatek86.vue
{
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        private readonly Controle controle;
        const string ETATNEUF = "00001";

        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgInfosGenres = new BindingSource();
        private readonly BindingSource bdgInfosPublics = new BindingSource();
        private readonly BindingSource bdgInfosRayons = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private readonly BindingSource bdgCommandesLivresListe = new BindingSource();
        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();
        private readonly BindingSource bdgAbonnementRevueListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        private List<CommandeDocument> lesCommandeDocument = new List<CommandeDocument>();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        private List<Suivi> lesSuivis = new List<Suivi>();

        #endregion

        internal FrmMediatek(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
        }

        /// <summary>
        /// Dès l'ouverture de l'application la vue d'alerte de fin d'abonnements s'ouvre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Shown(object sender, EventArgs e)
        {
            AlerteFinAbonnements alerteFinAbonnements = new AlerteFinAbonnements(controle);
            alerteFinAbonnements.StartPosition = FormStartPosition.CenterParent;
            alerteFinAbonnements.ShowDialog();
        }

        #region modules communs

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
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
        /// <returns></returns>
        private bool ValidationSuppressionAbonnement()
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir supprimer cet abonnement ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de suppression d'une commande
        /// </summary>
        /// <returns></returns>
        private bool ValidationSuppressionCommande()
        {
            return (MessageBox.Show("Etes-vous sûr de vouloir supprimer cette commande ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Affichage d'un MessageBox pour demander validation de changement d'état de suivi
        /// </summary>
        /// <param name="libelleSuivi"></param>
        /// <returns></returns>
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
            if ((saisieLivre ||saisieDvd || saisieRevue || saisieCommandeLivres) && !VerifAbandonSaisie())
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
        /// <param name="titreColonne"></param>
        /// <returns></returns>
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
