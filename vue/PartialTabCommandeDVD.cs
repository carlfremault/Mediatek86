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
    /// Classe partielle représentant l'onglet de commande de DVD
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "COMMANDE DE DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Boolean true si on est en train de faire une saisie de commande de DVD
        /// </summary>
        private bool saisieCommandeDvd = false;

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de la commande
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// Récupération des DVD et suivis depuis le contrôleur
        /// Désactivation de groupBox de gestion de commandes
        /// Vide les champs des infos des DVD et des détails de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeDVD_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesDvd = controle.GetAllDvd();
            lesSuivis = controle.GetAllSuivis();
            AccesGestionCommandeDvdGroupBox(false);
            txbCommandeDvdNumeroDvd.Text = "";
            VideCommandeDvdInfos();
            VideDetailsCommandeDvd();
        }

        /// <summary>
        /// Remplit le dategrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="lesCommandeDocument">La collection des CommandeDocument</param>
        private void RemplirCommandeDvdListe(List<CommandeDocument> lesCommandeDocument)
        {

            bdgCommandesDvdListe.DataSource = lesCommandeDocument;
            dgvCommandeDvdListe.DataSource = bdgCommandesDvdListe;
            dgvCommandeDvdListe.Columns["id"].Visible = false;
            dgvCommandeDvdListe.Columns["idSuivi"].Visible = false;
            dgvCommandeDvdListe.Columns["idLivreDvd"].Visible = false;
            dgvCommandeDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeDvdListe.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCommandeDvdListe.Columns[6].DefaultCellStyle.Format = "c2";
            dgvCommandeDvdListe.Columns[6].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("fr-FR");
            dgvCommandeDvdListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeDvdListe.Columns["montant"].DisplayIndex = 1;
            dgvCommandeDvdListe.Columns[5].HeaderCell.Value = "Date";
            dgvCommandeDvdListe.Columns[0].HeaderCell.Value = "Exemplaires";
            dgvCommandeDvdListe.Columns[2].HeaderCell.Value = "Etat";
        }

        /// <summary>
        /// Recherche d'un numéro de DVD et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdRechercher_Click(object sender, EventArgs e)
        {
            if (saisieCommandeDvd && VerifAbandonSaisie())
            {
                FinSaisieCommandeDvd();
                CommandeDvdRechercher();
            }
            else if (!saisieCommandeDvd)
            {
                CommandeDvdRechercher();
            }
        }

        /// <summary>
        /// Recherche d'un numéro de DVD et affichage des informations
        /// </summary>
        private void CommandeDvdRechercher()
        {
            if (!txbCommandeDvdNumeroDvd.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandeDvdNumeroDvd.Text.Trim()));
                if (dvd != null)
                {
                    AfficheCommandeDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("Numéro introuvable");
                    txbCommandeDvdNumeroDvd.Text = "";
                    txbCommandeDvdNumeroDvd.Focus();
                    VideCommandeDvdInfos();
                }
            }
            else
            {
                VideCommandeDvdInfos();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ de recherche déclence la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeDvdNumero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnCommandeDvdRechercher_Click(sender, e);
            }
        }

        /// <summary>
        /// Si le numéro de DVD est modifié, la zone de commande est désactivé
        /// et les informations du DVD sont effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeDvdNumero_TextChanged(object sender, EventArgs e)
        {
            if (!saisieCommandeDvd)
            {
                AccesGestionCommandeDvdGroupBox(false);
                VideCommandeDvdInfos();
            }
        }

        /// <summary>
        /// Affichage des informations du DVD sélectionné et les exemplaires
        /// </summary>
        /// <param name="dvd">Le DVD sélectionné</param>
        private void AfficheCommandeDvdInfos(Dvd dvd)
        {
            // informations sur le DVD
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdImage.Text = dvd.Image;
            txbCommandeDvdDuree.Text = dvd.Duree.ToString();
            string image = dvd.Image;
            try
            {
                pcbCommandeDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCommandeDvdImage.Image = null;
            }
            // affiche la liste des commandes du DVD
            AfficheCommandeDocumentDvd();

            // accès à la zone d'ajout d'un exemplaire
            AccesGestionCommandeDvdGroupBox(true);
        }

        /// <summary>
        /// Affichage des détails d'une commande de DVD
        /// </summary>
        /// <param name="commandeDocument">La commande concernée</param>
        private void AfficheCommandeDvdCommande(CommandeDocument commandeDocument)
        {
            txbCommandeDvdNumeroCommande.Text = commandeDocument.Id;
            dtpCommandeDvdDateCommande.Value = commandeDocument.DateCommande;
            nudCommandeDvdExemplaires.Value = commandeDocument.NbExemplaires;
            txbCommandeDvdMontant.Text = commandeDocument.Montant.ToString("C2",
                  CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        /// <summary>
        /// Récupération de la liste de commandes d'un DVD puis affichage dans la liste
        /// </summary>
        private void AfficheCommandeDocumentDvd()
        {
            string idDocument = txbCommandeDvdNumeroDvd.Text.Trim();
            lesCommandeDocument = controle.GetCommandeDocument(idDocument);
            RemplirCommandeDvdListe(lesCommandeDocument);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations du DVD
        /// </summary>
        private void VideCommandeDvdInfos()
        {
            txbCommandeDvdTitre.Text = "";
            txbCommandeDvdRealisateur.Text = "";
            txbCommandeDvdSynopsis.Text = "";
            txbCommandeDvdGenre.Text = "";
            txbCommandeDvdPublic.Text = "";
            txbCommandeDvdRayon.Text = "";
            txbCommandeDvdImage.Text = "";
            txbCommandeDvdDuree.Text = "";
            pcbCommandeDvdImage.Image = null;
            lesCommandeDocument = new List<CommandeDocument>();
            RemplirCommandeDvdListe(lesCommandeDocument);
            AccesGestionCommandeDvdGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des détails de commande.
        /// </summary>
        private void VideDetailsCommandeDvd()
        {
            txbCommandeDvdNumeroCommande.Text = "";
            dtpCommandeDvdDateCommande.Value = DateTime.Now;
            nudCommandeDvdExemplaires.Value = 1;
            txbCommandeDvdMontant.Text = "";
        }

        /// <summary>
        /// (Dés)active la zone de gestion de commandes et le bouton 'Ajouter'
        /// </summary>
        /// <param name="acces">'True' autorise l'accès</param>
        private void AccesGestionCommandeDvdGroupBox(bool acces)
        {
            grpGestionCommandeDvd.Enabled = acces;
            btnCommandeDvdAjouter.Enabled = acces;
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandeDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = SortCommandeDocumentList(titreColonne);
            RemplirCommandeDvdListe(sortedList);
        }

        /// <summary>
        /// Evénement sélection d'une ligne dans la liste des commandes
        /// Vérifie si une saisie est en cours avant de procéder
        /// Demande validation d'abandon si une saisie est en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieCommandeDvd)
            {
                if (VerifAbandonSaisie())
                {
                    FinSaisieCommandeDvd();
                    CommandeDvdListeSelection();
                }
            }
            else
            {
                CommandeDvdListeSelection();
            }
        }

        /// <summary>
        /// Affichage des infos de la commande sélectionnée dans la liste
        /// </summary>
        private void CommandeDvdListeSelection()
        {
            if (dgvCommandeDvdListe.CurrentCell != null)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                AfficheCommandeDvdCommande(commandeDocument);
                ActivationModificationCommandeDvd(commandeDocument);
            }
            else
            {
                DesActivationModificationCommandeDvd();
                VideDetailsCommandeDvd();
            }
        }

        /// <summary>
        /// Activation des boutons de gestion de commande en fonction de l'état de suivi
        /// </summary>
        /// <param name="commandeDocument">La CommandeDocument concernée</param>
        private void ActivationModificationCommandeDvd(CommandeDocument commandeDocument)
        {
            string etatSuivi = commandeDocument.LibelleSuivi;
            switch (etatSuivi)
            {
                case "En cours":
                case "Relancée":
                    btnCommandeDvdRelancer.Enabled = true;
                    btnCommandeDvdConfirmerLivraison.Enabled = true;
                    btnCommandeDvdRegler.Enabled = false;
                    btnCommandeDvdSupprimer.Enabled = true;
                    break;
                case "Livrée":
                    btnCommandeDvdRelancer.Enabled = false;
                    btnCommandeDvdConfirmerLivraison.Enabled = false;
                    btnCommandeDvdRegler.Enabled = true;
                    btnCommandeDvdSupprimer.Enabled = false;
                    break;
                case "Réglée":
                    DesActivationModificationCommandeDvd();
                    break;
            }
        }

        /// <summary>
        /// Désactivation des boutons de gestion de commande (sauf ajout)
        /// </summary>
        private void DesActivationModificationCommandeDvd()
        {
            btnCommandeDvdRelancer.Enabled = false;
            btnCommandeDvdConfirmerLivraison.Enabled = false;
            btnCommandeDvdRegler.Enabled = false;
            btnCommandeDvdSupprimer.Enabled = false;
        }

        /// <summary>
        /// Début de saisie de commande de DVD. 
        /// </summary>
        private void DebutSaisieCommandeDvd()
        {
            AccesSaisieCommandeDvd(true);
        }

        /// <summary>
        /// Fin de saisie de commande de DVD
        /// Affiche les informations de la commande sélectionnée dans la liste
        /// </summary>
        private void FinSaisieCommandeDvd()
        {
            AccesSaisieCommandeDvd(false);
            CommandeDvdListeSelection();
        }

        /// <summary>
        /// Actionne le booleen saisieCommandeDvd
        /// Vide les champs de détails d'une commande
        /// (Dés)active la protection readonly des champs de détails de commande
        /// (Dés)active les boutons concernant l'ajout, validation et annulation de saisie de commande
        /// </summary>
        /// <param name="accces">'True' active les boutons 'Valider' et 'Annuler', désactive le bouton 'Ajouter', déverrouille les champs des détails de commande</param>
        private void AccesSaisieCommandeDvd(bool accces)
        {
            saisieCommandeDvd = accces;
            VideDetailsCommandeDvd();
            btnCommandeDvdValider.Enabled = accces;
            btnCommandeDvdAnnuler.Enabled = accces;
            btnCommandeDvdAjouter.Enabled = !accces;
            txbCommandeDvdNumeroCommande.Enabled = accces;
            dtpCommandeDvdDateCommande.Enabled = accces;
            nudCommandeDvdExemplaires.Enabled = accces;
            txbCommandeDvdMontant.Enabled = accces;
            grpCommandeDvd.Enabled = accces;
        }

        /// <summary>
        /// Evénement clic sur le bouton d'ajout de commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdAjouter_Click(object sender, EventArgs e)
        {
            DesActivationModificationCommandeDvd();
            DebutSaisieCommandeDvd();
        }

        /// <summary>
        /// Evénement clic sur le bouton d'annulation d'une saisie de commande
        /// Demande validation de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdAnnuler_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                FinSaisieCommandeDvd();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton de validation d'une commande
        /// Vérifie si tous les champs sont remplis et la validité du champ 'montant'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdValider_Click(object sender, EventArgs e)
        {
            if (txbCommandeDvdNumeroCommande.Text == "" || txbCommandeDvdMontant.Text == "")
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
                return;
            }

            String id = txbCommandeDvdNumeroCommande.Text;
            DateTime dateCommande = dtpCommandeDvdDateCommande.Value;
            int nbExemplaires = (int)nudCommandeDvdExemplaires.Value;
            string idLivreDvd = txbCommandeDvdNumeroDvd.Text.Trim();
            int idSuivi = lesSuivis[0].Id;
            string libelleSuivi = lesSuivis[0].Libelle;
            String montantSaisie = txbCommandeDvdMontant.Text.Replace(',', '.');
            bool success = Double.TryParse(montantSaisie, out double montant);
            if (!success)
            {
                MessageBox.Show("La valeur saisie pour le montant doit être numérique.", "Erreur");
                txbCommandeDvdMontant.Text = "";
                txbCommandeDvdMontant.Focus();
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
                txbCommandeDvdNumeroCommande.Text = "";
                txbCommandeDvdNumeroCommande.Focus();
                return;
            }
            else
            {
                MessageBox.Show(message, "Erreur");
                return;
            }
            FinSaisieCommandeDvd();
            AfficheCommandeDocumentDvd();
        }

        /// <summary>
        /// Evénement clic sur le bouton de suppression d'une commande de DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (ValidationSuppressionCommande())
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                if (controle.SupprCommandeDocument(commandeDocument.Id))
                {
                    AfficheCommandeDocumentDvd();
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
        private void btnCommandeDvdRelancer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Relancée");
            ModifEtatSuiviCommandeDocumentDvd(commandeDocument.Id, nouveauSuivi);
        }

        /// <summary>
        /// Modification d'état de suivi d'une commande livre : étape 2 "réglée"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdConfirmerLivraison_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Livrée");
            if (ModifEtatSuiviCommandeDocumentDvd(commandeDocument.Id, nouveauSuivi))
            {
                MessageBox.Show("Les exemplaires ont été ajoutés dans la base de données.", "Information");
            }
        }

        /// <summary>
        /// Modification d'état de suivi d'une commande livre : étape 3 "réglée"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdRegler_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
            Suivi nouveauSuivi = lesSuivis.Find(suivi => suivi.Libelle == "Réglée");
            ModifEtatSuiviCommandeDocumentDvd(commandeDocument.Id, nouveauSuivi);
        }

        /// <summary>
        /// Demande de modification de l'état de suivi au contrôleur après validation utilisateur
        /// </summary>
        /// <param name="idCommandeDocument">identifiant du document concerné</param>
        /// <param name="nouveauSuivi">nouvel état de suivi</param>
        /// <returns>True si modification a réussi</returns>
        private bool ModifEtatSuiviCommandeDocumentDvd(string idCommandeDocument, Suivi nouveauSuivi)
        {
            if (ValidationModifEtatSuivi(nouveauSuivi.Libelle))
            {
                if (controle.ModifSuiviCommandeDocument(idCommandeDocument, nouveauSuivi.Id))
                {
                    AfficheCommandeDocumentDvd();
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
