using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Les vues de l'application
/// </summary>
namespace Mediatek86.vue
{
    /// <summary>
    /// Classe partielle représentant l'onglet d'abonnements aux revues
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "ABONNEMENTS REVUE"
        //-----------------------------------------------------------

        /// <summary>
        /// Boolean true si on est en train de faire une saisie d'abonnement à une revue
        /// </summary>
        private bool saisieAbonnementRevue = false;

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'abonnement
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// Récupération des revues depuis le contrôleur
        /// Désactivation de groupBox de gestion d'abonnement
        /// Vide les champs de détails de commande et de revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnementRevue_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesRevues = controle.GetAllRevues();
            AccesGestionAbonnementRevueGroupBox(false);
            txbAbonnementRevueNumeroRevue.Text = "";
            VideAbonnementRevueInfos();
            VideDetailsAbonnementRevue();
        }

        /// <summary>
        /// Remplit le dategrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="lesAbonnements">La collection d'abonnements</param>
        private void RemplirAbonnementRevueListe(List<Abonnement> lesAbonnements)
        {
            bdgAbonnementRevueListe.DataSource = lesAbonnements;
            dgvAbonnementRevueListe.DataSource = bdgAbonnementRevueListe;
            dgvAbonnementRevueListe.Columns["id"].Visible = false;
            dgvAbonnementRevueListe.Columns["idRevue"].Visible = false;
            dgvAbonnementRevueListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAbonnementRevueListe.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvAbonnementRevueListe.Columns[4].DefaultCellStyle.Format = "c2";
            dgvAbonnementRevueListe.Columns[4].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("fr-FR");
            dgvAbonnementRevueListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvAbonnementRevueListe.Columns["montant"].DisplayIndex = 1;
            dgvAbonnementRevueListe.Columns[3].HeaderCell.Value = "Date commande";
            dgvAbonnementRevueListe.Columns[0].HeaderCell.Value = "Date fin abonnement";
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueRechercher_Click(object sender, EventArgs e)
        {
            if (saisieAbonnementRevue && VerifAbandonSaisie())
            {
                FinSaisieAbonnementRevue();
                AbonnementRevueRechercher();
            }
            else if (!saisieCommandeDvd)
            {
                AbonnementRevueRechercher();
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affichage des informations
        /// </summary>
        private void AbonnementRevueRechercher()
        {
            if (!txbAbonnementRevueNumeroRevue.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbAbonnementRevueNumeroRevue.Text.Trim()));
                if (revue != null)
                {
                    AfficheAbonnementRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    txbAbonnementRevueNumeroRevue.Text = "";
                    txbAbonnementRevueNumeroRevue.Focus();
                    VideAbonnementRevueInfos();
                }
            }
            else
            {
                VideAbonnementRevueInfos();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ de recherche déclenche la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbAbonnementRevueNumero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnAbonnementRevueRechercher_Click(sender, e);
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone d'abonnements est vidée et inactive
        /// les informations de la revue sont aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbAbonnementRevueRecherche_TextChanged(object sender, EventArgs e)
        {
            AccesGestionAbonnementRevueGroupBox(false);
            VideAbonnementRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">La revue sélectionnée</param>
        private void AfficheAbonnementRevueInfos(Revue revue)
        {
            // informations sur l'abonnement
            txbAbonnementRevueTitre.Text = revue.Titre;
            txbAbonnementRevuePeriodicite.Text = revue.Periodicite;
            txbAbonnementRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbAbonnementRevueGenre.Text = revue.Genre;
            txbAbonnementRevuePublic.Text = revue.Public;
            txbAbonnementRevueRayon.Text = revue.Rayon;
            txbAbonnementRevueImage.Text = revue.Image;
            chkAbonnementRevueEmpruntable.Checked = revue.Empruntable;
            string image = revue.Image;
            try
            {
                pcbAbonnementRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbAbonnementRevueImage.Image = null;
            }
            // affiche la liste des abonnements de la revue
            AfficheAbonnementsRevue();

            // accès à la zone d'ajout d'abonnement
            AccesGestionAbonnementRevueGroupBox(true);
        }

        /// <summary>
        /// Affichage des détails d'un abonne;ent
        /// </summary>
        /// <param name="abonnement">L'abonnement concerné</param>
        private void AfficheAbonnementRevueAbonnement(Abonnement abonnement)
        {
            txbAbonnementRevueNumeroAbonnement.Text = abonnement.Id;
            dtpAbonnementRevueDateCommande.Value = abonnement.DateCommande;
            dtpAbonnementRevueFinAbonnement.Value = abonnement.DateFinAbonnement;
            txbAbonnementRevueMontant.Text = abonnement.Montant.ToString("C2",
                  CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        /// <summary>
        /// Récupération de la liste des abonnements à une revue puis affichage dans la liste
        /// </summary>
        private void AfficheAbonnementsRevue()
        {
            string idDocument = txbAbonnementRevueNumeroRevue.Text.Trim();
            lesAbonnements = controle.GetAbonnement(idDocument);
            RemplirAbonnementRevueListe(lesAbonnements);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideAbonnementRevueInfos()
        {
            txbAbonnementRevueTitre.Text = "";
            txbAbonnementRevuePeriodicite.Text = "";
            txbAbonnementRevueDelaiMiseADispo.Text = "";
            txbAbonnementRevueGenre.Text = "";
            txbAbonnementRevuePublic.Text = "";
            txbAbonnementRevueRayon.Text = "";
            txbAbonnementRevueImage.Text = "";
            chkAbonnementRevueEmpruntable.Checked = false;
            pcbAbonnementRevueImage.Image = null;
            lesAbonnements = new List<Abonnement>();
            RemplirAbonnementRevueListe(lesAbonnements);
            AccesGestionAbonnementRevueGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des détails d'un abonnement
        /// </summary>
        private void VideDetailsAbonnementRevue()
        {
            txbAbonnementRevueNumeroAbonnement.Text = "";
            dtpAbonnementRevueDateCommande.Value = DateTime.Now;
            dtpAbonnementRevueFinAbonnement.Value = DateTime.Now.AddYears(1);
            txbAbonnementRevueMontant.Text = "";
        }

        /// <summary>
        /// (Dés)active la zone de gestion de commandes et le bouton 'Ajouter' 
        /// </summary>
        /// <param name="acces">'True' autorise l'accès</param>
        private void AccesGestionAbonnementRevueGroupBox(bool acces)
        {
            grpGestionAbonnementRevue.Enabled = acces;
            btnAbonnementRevueAjouter.Enabled = acces;
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementRevueListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titrecolonne = dgvAbonnementRevueListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titrecolonne)
            {
                case "Date commande":
                    sortedList = lesAbonnements.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnements.OrderBy(o => o.Montant).Reverse().ToList();
                    break;
                case "Date fin abonnement":
                    sortedList = lesAbonnements.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementRevueListe(sortedList);
        }

        /// <summary>
        /// Evénement sélection d'une ligne dans la liste des abonnements 
        /// Vérifie si une saisie est en cours avant de procéder
        /// Demande validation d'abandon si une saisie est en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementRevueListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieAbonnementRevue)
            {
                if (VerifAbandonSaisie())
                {
                    FinSaisieAbonnementRevue();
                    AbonnementRevueListeSelection();
                }
            }
            else
            {
                AbonnementRevueListeSelection();
            }
        }

        /// <summary>
        /// Affichage des infos de l'abonnement sélectionnée dans la liste
        /// </summary>
        private void AbonnementRevueListeSelection()
        {
            if (dgvAbonnementRevueListe.CurrentCell != null)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnementRevueListe.List[bdgAbonnementRevueListe.Position];
                AfficheAbonnementRevueAbonnement(abonnement);
                btnAbonnementRevueSupprimer.Enabled = true;
            }
            else
            {
                btnAbonnementRevueSupprimer.Enabled = false;
                VideDetailsAbonnementRevue();
            }
        }

        /// <summary>
        /// Début de saisie d'abonnement
        /// </summary>
        private void DebutSaisieAbonnementRevue()
        {
            AccesSaisieAbonnement(true);
        }

        /// <summary>
        /// Fin de saisie d'abonnement
        /// Affiche les informations de l'abonnement sélectionnée dans la liste
        /// </summary>
        private void FinSaisieAbonnementRevue()
        {
            AccesSaisieAbonnement(false);
            AbonnementRevueListeSelection();
        }

        /// <summary>
        /// Actionne le booleen saisieAbonnementRevue
        /// Vide les champs de détails d'un abonnement
        /// (Dés)active la protection readonly des champs de détails d'abonnement
        /// (Dés)active les boutons concernant l'ajout, validation et annulation de saisie d'abonnement
        /// </summary>
        /// <param name="acces">'True' active les boutons 'Valider' et 'Annuler', désactive le bouton 'Ajouter', déverrouille les champs des détails de commande</param>
        private void AccesSaisieAbonnement(bool acces)
        {
            saisieAbonnementRevue = acces;
            VideDetailsAbonnementRevue();
            btnAbonnementRevueValider.Enabled = acces;
            btnAbonnementRevueAnnuler.Enabled = acces;
            btnAbonnementRevueAjouter.Enabled = !acces;
            txbAbonnementRevueNumeroAbonnement.Enabled = acces;
            dtpAbonnementRevueDateCommande.Enabled = acces;
            dtpAbonnementRevueFinAbonnement.Enabled = acces;
            txbAbonnementRevueMontant.Enabled = acces;
            grpAbonnementRevue.Enabled = acces;
        }

        /// <summary>
        /// Evénement clic sur le bouton d'ajout d'abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueAjouter_Click(object sender, EventArgs e)
        {
            DebutSaisieAbonnementRevue();
        }

        /// <summary>
        /// Evénement clic sur le bouton d'annulation d'une saisie d'abonnement
        /// Demande validation de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueAnnuler_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                FinSaisieAbonnementRevue();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton de validation d'un abonnement
        /// Vérifie si tous les champs sont remplis et la validité du champ 'montant'
        /// Vérifie si la date de fin d'abonnement est ultérieur à la date de souscription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueValider_Click(object sender, EventArgs e)
        {
            if (txbAbonnementRevueNumeroAbonnement.Text == "" || txbAbonnementRevueMontant.Text == "")
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
                return;
            }

            if (DateTime.Compare(dtpAbonnementRevueDateCommande.Value, dtpAbonnementRevueFinAbonnement.Value) >= 0)
            {
                MessageBox.Show("La date de fin d'abonnement ne peut être antérieur ou égal à la date de souscription à l'abonnement.", "Information");
                dtpAbonnementRevueFinAbonnement.Value = DateTime.Now.AddYears(1);
                dtpAbonnementRevueFinAbonnement.Focus();
                return;
            }

            String id = txbAbonnementRevueNumeroAbonnement.Text;
            DateTime dateCommande = dtpAbonnementRevueDateCommande.Value;
            DateTime dateFinAbonnement = dtpAbonnementRevueFinAbonnement.Value;
            string idRevue = txbAbonnementRevueNumeroRevue.Text.Trim();
            String montantSaisie = txbAbonnementRevueMontant.Text.Replace(',', '.');
            bool success = Double.TryParse(montantSaisie, out double montant);
            if (!success)
            {
                MessageBox.Show("La valeur saisie pour le montant doit être numérique.", "Erreur");
                txbAbonnementRevueMontant.Text = "";
                txbAbonnementRevueMontant.Focus();
                return;
            }
            Abonnement nouvelAbonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);

            String message = controle.CreerAbonnement(nouvelAbonnement);
            if (message.Substring(0, 2) == "OK")
            {
                MessageBox.Show("Abonnement validée!", "Information");
            }
            else if (message.Substring(0, 9) == "Duplicate")
            {
                MessageBox.Show("Ce numéro d'abonnement existe déjà.", "Erreur");
                txbAbonnementRevueNumeroAbonnement.Text = "";
                txbAbonnementRevueNumeroAbonnement.Focus();
                return;
            }
            else
            {
                MessageBox.Show(message, "Erreur");
                return;
            }
            FinSaisieAbonnementRevue();
            AfficheAbonnementsRevue();
        }

        /// <summary>
        /// Evénement clic sur le bouton supprimer
        /// Vérifie s'il n'y a pas d'exemplaires rattachés à l'abonnement
        /// Si non demande validation à l'utilisateur avant de procéder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueSupprimer_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgAbonnementRevueListe.List[bdgAbonnementRevueListe.Position];
            if (!controle.VerifSuppressionAbonnement(abonnement))
            {
                if (ValidationSuppressionAbonnement())
                {
                    if (controle.SupprAbonnement(abonnement.Id))
                    {
                        AfficheAbonnementsRevue();
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Impossible de supprimer cet abonnement, des exemplaires rattachés existent.", "Information");
            }
        }
    }
}
