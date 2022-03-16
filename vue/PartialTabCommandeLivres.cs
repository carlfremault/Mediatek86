using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

/// <summary>
/// Les vues de l'application
/// </summary>
namespace Mediatek86.vue
{
    /// <summary>
    /// Classe partielle représentant l'onglet de commande de livres
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "COMMANDE DE LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Boolean true si on est en train de faire une saisie de commande de livre
        /// </summary>
        private bool saisieCommandeLivres = false;

        /// <summary>
        /// Ouverture de l'onglet : 
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// Récupération des livres et suivis depuis le contrôleur
        /// Désactivation de groupBox de gestion de commandes
        /// Vide les champs des infos des livres et des détails de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivres_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesLivres = controle.GetAllLivres();
            lesSuivis = controle.GetAllSuivis();
            AccesGestionCommandeLivresGroupBox(false);
            txbCommandeLivresNumeroLivre.Text = "";
            VideCommandeLivresInfos();
            VideDetailsCommandeLivres();
        }

        /// <summary>
        /// Remplit le dategrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="lesCommandeDocument">La collection de CommandeDocument</param>
        private void RemplirCommandeLivresListe(List<CommandeDocument> lesCommandeDocument)
        {
            bdgCommandesLivresListe.DataSource = lesCommandeDocument;
            dgvCommandeLivresListe.DataSource = bdgCommandesLivresListe;
            dgvCommandeLivresListe.Columns["id"].Visible = false;
            dgvCommandeLivresListe.Columns["idSuivi"].Visible = false;
            dgvCommandeLivresListe.Columns["idLivreDvd"].Visible = false;
            dgvCommandeLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeLivresListe.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCommandeLivresListe.Columns[6].DefaultCellStyle.Format = "c2";
            dgvCommandeLivresListe.Columns[6].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("fr-FR");
            dgvCommandeLivresListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeLivresListe.Columns["montant"].DisplayIndex = 1;
            dgvCommandeLivresListe.Columns[5].HeaderCell.Value = "Date";
            dgvCommandeLivresListe.Columns[0].HeaderCell.Value = "Exemplaires";
            dgvCommandeLivresListe.Columns[2].HeaderCell.Value = "Etat";
        }

        /// <summary>
        /// Evénement clic sur le bouton de recherche de livre. 
        /// Vérifie si on est en train de saisier une commande avant de procéder
        /// Demande confirmation d'abandon si une saisie est en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivreRechercher_Click(object sender, EventArgs e)
        {
            if (saisieCommandeLivres && VerifAbandonSaisie())
            {
                FinSaisieCommandeLivres();
                CommandeLivresRechercher();
            }
            else if (!saisieCommandeLivres)
            {
                CommandeLivresRechercher();
            }
        }

        /// <summary>
        /// Recherche d'un numéro de livre et affichage des informations
        /// </summary>
        private void CommandeLivresRechercher()
        {
            if (!txbCommandeLivresNumeroLivre.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandeLivresNumeroLivre.Text.Trim()));
                if (livre != null)
                {
                    AfficheCommandeLivresInfos(livre);
                }
                else
                {
                    MessageBox.Show("Numéro introuvable");
                    txbCommandeLivresNumeroLivre.Text = "";
                    txbCommandeLivresNumeroLivre.Focus();
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ de recherche déclenche la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeLivreNumero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnCommandeLivreRechercher_Click(sender, e);
            }
        }

        /// <summary>
        /// Si le numéro de livre est modifié, la zone de commande est désactivée
        /// et les informations du livre sont effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeLivreNumero_TextChanged(object sender, EventArgs e)
        {
            if (!saisieCommandeLivres)
            {
                AccesGestionCommandeLivresGroupBox(false);
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné et les commandes
        /// </summary>
        /// <param name="livre">Le livre sélectionné</param>
        private void AfficheCommandeLivresInfos(Livre livre)
        {
            // informations sur le livre
            txbCommandeLivresTitre.Text = livre.Titre;
            txbCommandeLivresAuteur.Text = livre.Auteur;
            txbCommandeLivresCollection.Text = livre.Collection;
            txbCommandeLivresGenre.Text = livre.Genre;
            txbCommandeLivresPublic.Text = livre.Public;
            txbCommandeLivresRayon.Text = livre.Rayon;
            txbCommandeLivresImage.Text = livre.Image;
            txbCommandeLivresISBN.Text = livre.Isbn;
            string image = livre.Image;
            try
            {
                pcbCommandeLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCommandeLivresImage.Image = null;
            }
            // affiche la liste des commandes du livre
            AfficheCommandeDocumentLivre();

            // accès à la zone de gestion de commande
            AccesGestionCommandeLivresGroupBox(true);
        }

        /// <summary>
        /// Affichage des détails d'une commande de livre
        /// </summary>
        /// <param name="commandeDocument">La commande concernée</param>
        private void AfficheCommandeLivresCommande(CommandeDocument commandeDocument)
        {
            txbCommandeLivresNumeroCommande.Text = commandeDocument.Id;
            dtpCommandeLivresDateCommande.Value = commandeDocument.DateCommande;
            nudCommandeLivresExemplaires.Value = commandeDocument.NbExemplaires;
            txbCommandeLivresMontant.Text = commandeDocument.Montant.ToString("C2",
                  CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        /// <summary>
        /// Récupération de la liste de commandes d'un livre puis affichage dans la liste
        /// </summary>
        private void AfficheCommandeDocumentLivre()
        {
            string idDocument = txbCommandeLivresNumeroLivre.Text.Trim();
            lesCommandeDocument = controle.GetCommandeDocument(idDocument);
            RemplirCommandeLivresListe(lesCommandeDocument);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations du livre et de commande
        /// </summary>
        private void VideCommandeLivresInfos()
        {
            txbCommandeLivresTitre.Text = "";
            txbCommandeLivresAuteur.Text = "";
            txbCommandeLivresCollection.Text = "";
            txbCommandeLivresGenre.Text = "";
            txbCommandeLivresPublic.Text = "";
            txbCommandeLivresRayon.Text = "";
            txbCommandeLivresImage.Text = "";
            txbCommandeLivresISBN.Text = "";
            pcbCommandeLivresImage.Image = null;
            lesCommandeDocument = new List<CommandeDocument>();
            RemplirCommandeLivresListe(lesCommandeDocument);
            AccesGestionCommandeLivresGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des détails de commande.
        /// </summary>
        private void VideDetailsCommandeLivres()
        {
            txbCommandeLivresNumeroCommande.Text = "";
            dtpCommandeLivresDateCommande.Value = DateTime.Now;
            nudCommandeLivresExemplaires.Value = 1;
            txbCommandeLivresMontant.Text = "";
        }

        /// <summary>
        /// (Dés)active la zone de gestion de commandes et le bouton 'Ajouter'
        /// </summary>
        /// <param name="acces">'True' autorise l'accès</param>
        private void AccesGestionCommandeLivresGroupBox(bool acces)
        {
            grpGestionCommandeLivres.Enabled = acces;
            btnCommandeLivresAjouter.Enabled = acces;
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandeLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = SortCommandeDocumentList(titreColonne);
            RemplirCommandeLivresListe(sortedList);
        }

        /// <summary>
        /// Evénement sélection d'une ligne dans la liste des commandes
        /// Vérifie si une saisie est en cours avant de procéder
        /// Demande validation d'abandon si une saisie est en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieCommandeLivres)
            {
                if (VerifAbandonSaisie())
                {
                    FinSaisieCommandeLivres();
                    CommandeLivresListeSelection();
                }
            }
            else
            {
                CommandeLivresListeSelection();
            }
        }

        /// <summary>
        /// Affichage des infos de la commande sélectionnée dans la liste
        /// </summary>
        private void CommandeLivresListeSelection()
        {
            if (dgvCommandeLivresListe.CurrentCell != null)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
                AfficheCommandeLivresCommande(commandeDocument);
                ActivationModificationCommandeLivres(commandeDocument);
            }
            else
            {
                DesActivationModificationCommandeLivres();
                VideDetailsCommandeLivres();
            }
        }

        /// <summary>
        /// Activation des boutons de gestion de commande en fonction de l'état de suivi
        /// </summary>
        /// <param name="commandeDocument">La CommandeDocument concernée</param>
        private void ActivationModificationCommandeLivres(CommandeDocument commandeDocument)
        {
            string etatSuivi = commandeDocument.LibelleSuivi;
            switch (etatSuivi)
            {
                case "En cours":
                case "Relancée":
                    btnCommandeLivresRelancer.Enabled = true;
                    btnCommandeLivresConfirmerLivraison.Enabled = true;
                    btnCommandeLivresRegler.Enabled = false;
                    btnCommandeLivresSupprimer.Enabled = true;
                    break;
                case "Livrée":
                    btnCommandeLivresRelancer.Enabled = false;
                    btnCommandeLivresConfirmerLivraison.Enabled = false;
                    btnCommandeLivresRegler.Enabled = true;
                    btnCommandeLivresSupprimer.Enabled = false;
                    break;
                case "Réglée":
                    DesActivationModificationCommandeLivres();
                    break;
            }
        }

        /// <summary>
        /// Désactivation des boutons de gestion de commande (sauf ajout)
        /// </summary>
        private void DesActivationModificationCommandeLivres()
        {
            btnCommandeLivresRelancer.Enabled = false;
            btnCommandeLivresConfirmerLivraison.Enabled = false;
            btnCommandeLivresRegler.Enabled = false;
            btnCommandeLivresSupprimer.Enabled = false;
        }

        /// <summary>
        /// Début de saisie de commande de livre 
        /// </summary>
        private void DebutSaisieCommandeLivres()
        {
            AccesSaisieCommandeLivre(true);
        }

        /// <summary>
        /// Fin de saisie de commande de livre
        /// Affiche les informations de la commande sélectionnée dans la liste
        /// </summary>
        private void FinSaisieCommandeLivres()
        {
            AccesSaisieCommandeLivre(false);
            CommandeLivresListeSelection();
        }

        /// <summary>
        /// Actionne le booleen saisieCommandeLivres
        /// Vide les champs de détails d'une commande
        /// (Dés)active la protection readonly des champs de détails de commande
        /// (Dés)active les boutons concernant l'ajout, validation et annulation de saisie de commande
        /// </summary>
        /// <param name="acces">'True' active les boutons 'Valider' et 'Annuler', désactive le bouton 'Ajouter', déverrouille les champs des détails de commande</param>
        private void AccesSaisieCommandeLivre(bool acces)
        {
            saisieCommandeLivres = acces;
            VideDetailsCommandeLivres();
            btnCommandeLivresValider.Enabled = acces;
            btnCommandeLivresAnnuler.Enabled = acces;
            btnCommandeLivresAjouter.Enabled = !acces;
            txbCommandeLivresNumeroCommande.Enabled = acces;
            dtpCommandeLivresDateCommande.Enabled = acces;
            nudCommandeLivresExemplaires.Enabled = acces;
            txbCommandeLivresMontant.Enabled = acces;
            grpCommandeLivres.Enabled = acces;
        }

        /// <summary>
        /// Evénement clic sur le bouton d'ajout de commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresAjouter_Click(object sender, EventArgs e)
        {
            DesActivationModificationCommandeLivres();
            DebutSaisieCommandeLivres();
        }

        /// <summary>
        /// Evénement clic sur le bouton d'annulation d'une saisie de commande
        /// Demande validation de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresAnnuler_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                FinSaisieCommandeLivres();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton de validation d'une commande
        /// Vérifie si tous les champs sont remplis et la validité du champ 'montant'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresValider_Click(object sender, EventArgs e)
        {
            if (txbCommandeLivresNumeroCommande.Text == "" || txbCommandeLivresMontant.Text == "")
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
                return;
            }

            String id = txbCommandeLivresNumeroCommande.Text;
            DateTime dateCommande = dtpCommandeLivresDateCommande.Value;
            int nbExemplaires = (int)nudCommandeLivresExemplaires.Value;
            string idLivreDvd = txbCommandeLivresNumeroLivre.Text.Trim();
            int idSuivi = lesSuivis[0].Id;
            string libelleSuivi = lesSuivis[0].Libelle;
            String montantSaisie = txbCommandeLivresMontant.Text.Replace(',', '.');
            bool success = Double.TryParse(montantSaisie, out double montant);
            if (!success)
            {
                MessageBox.Show("La valeur saisie pour le montant doit être numérique.", "Erreur");
                txbCommandeLivresMontant.Text = "";
                txbCommandeLivresMontant.Focus();
                return;
            }
            CommandeDocument laCommandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaires, idLivreDvd, idSuivi, libelleSuivi);

            String message = controle.CreerCommandeDocument(laCommandeDocument);
            if (message.Substring(0, 2) == "OK")
            {
                MessageBox.Show("Commande validée!", "Information");
            }
            else if (message.Substring(0, 9) == "Duplicate")
            {
                MessageBox.Show("Ce numéro de commande existe déjà.", "Erreur");
                txbCommandeLivresNumeroCommande.Text = "";
                txbCommandeLivresNumeroCommande.Focus();
                return;
            }
            else
            {
                MessageBox.Show(message, "Erreur");
                return;
            }
            FinSaisieCommandeLivres();
            AfficheCommandeDocumentLivre();
        }

        /// <summary>
        /// Evénement clic sur le bouton de suppression d'une commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresSupprimer_Click(object sender, EventArgs e)
        {
            if (ValidationSuppressionCommande())
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
                if (controle.SupprCommandeDocument(commandeDocument.Id))
                {
                    AfficheCommandeDocumentLivre();
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite.", "Erreur");
                }
            }
        }

        /// <summary>
        /// Modification d'état de suivi d'une commande livre : étape 1 "relancée"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresRelancer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Relancée");
            ModifEtatSuiviCommandeDocumentLivre(commandeDocument.Id, nouveauSuivi);
        }

        /// <summary>
        /// Modification d'état de suivi d'une commande livre  : étape 2 "livrée"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresConfirmerLivraison_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Livrée");
            if (ModifEtatSuiviCommandeDocumentLivre(commandeDocument.Id, nouveauSuivi))
            {
                MessageBox.Show("Les exemplaires ont été ajoutés dans la base de données.", "Information");
            }
        }

        /// <summary>
        /// Modification d'état de suivi d'une commande livre  : étape 3 "réglée"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivresRegler_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Réglée");
            ModifEtatSuiviCommandeDocumentLivre(commandeDocument.Id, nouveauSuivi);
        }

        /// <summary>
        /// Demande de modification de l'état de suivi au contrôleur après validation utilisateur
        /// </summary>
        /// <param name="idCommandeDocument">identifiant du document concerné</param>
        /// <param name="nouveauSuivi">nouvel état de suivi</param>
        /// <returns>True si modification a réussi</returns>
        private bool ModifEtatSuiviCommandeDocumentLivre(string idCommandeDocument, Suivi nouveauSuivi)
        {
            if (ValidationModifEtatSuivi(nouveauSuivi.Libelle))
            {
                if (controle.ModifSuiviCommandeDocument(idCommandeDocument, nouveauSuivi.Id))
                {
                    AfficheCommandeDocumentLivre();
                    return true;
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    return false;
                }
            }
            return false;
        }
    }
}
