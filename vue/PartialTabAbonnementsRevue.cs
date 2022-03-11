using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.metier;
using Mediatek86.controleur;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.vue
{
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "ABONNEMENTS REVUE"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnementRevue_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesRevues = controle.GetAllRevues();
            // accesCommandeDvdGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirAbonnementRevueListe(List<Abonnement> lesAbonnements)
        {
            bdgAbonnementRevueListe.DataSource = lesAbonnements;
            dgvAbonnementRevueListe.DataSource = bdgAbonnementRevueListe;
            dgvAbonnementRevueListe.Columns["id"].Visible = false;
            dgvAbonnementRevueListe.Columns["idRevue"].Visible = false;
            dgvAbonnementRevueListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
                    VideAbonnementRevueInfos();
                }
            }
            else
            {
                VideAbonnementRevueInfos();
            }
        }

        /// <summary>
        /// Entrée dans champ de recherche déclenche la recherche aussi
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
            // accesAbonnementRevueGroupBox(false);
            VideAbonnementRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
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
            // accesAbonnementRevueGroupBox(true);
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
            // accesAbonnementRevueGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        /*        private void VideReceptionExemplaireInfos()
                {
                    txbReceptionExemplaireImage.Text = "";
                    txbReceptionExemplaireNumero.Text = "";
                    pcbReceptionExemplaireImage.Image = null;
                    dtpReceptionExemplaireDate.Value = DateTime.Now;
                }*/

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        /*        private void accesReceptionExemplaireGroupBox(bool acces)
                {
                    VideReceptionExemplaireInfos();
                    grpReceptionExemplaire.Enabled = acces;
                }*/

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
                {
                    string filePath = "";
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                    }
                    txbReceptionExemplaireImage.Text = filePath;
                    try
                    {
                        pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
                    }
                    catch
                    {
                        pcbReceptionExemplaireImage.Image = null;
                    }
                }*/

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
                {
                    if (!txbReceptionExemplaireNumero.Text.Equals(""))
                    {
                        try
                        {
                            int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                            DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                            string photo = txbReceptionExemplaireImage.Text;
                            string idEtat = ETATNEUF;
                            string idDocument = txbReceptionRevueNumero.Text;
                            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                            if (controle.CreerExemplaire(exemplaire))
                            {
                                VideReceptionExemplaireInfos();
                                afficheReceptionExemplairesRevue();
                            }
                            else
                            {
                                MessageBox.Show("numéro de publication déjà existant", "Erreur");
                            }
                        }
                        catch
                        {
                            MessageBox.Show("le numéro de parution doit être numérique", "Information");
                            txbReceptionExemplaireNumero.Text = "";
                            txbReceptionExemplaireNumero.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("numéro de parution obligatoire", "Information");
                    }
                }*/

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementRevueListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            /*            string titreColonne = dgvCommandeLivresListe.Columns[e.ColumnIndex].HeaderText;
                        List<Commande> sortedList = new List<Commande>();
                        switch (titreColonne)
                        {
                            case "Numero":
                                sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                                break;
                            case "DateAchat":
                                sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                                break;
                            case "Photo":
                                sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                                break;
                        }
                        RemplirCommandeLivresListe(sortedList);*/
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
                {
                    if (dgvReceptionExemplairesListe.CurrentCell != null)
                    {
                        Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                        string image = exemplaire.Photo;
                        try
                        {
                            pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                        }
                        catch
                        {
                            pcbReceptionExemplaireRevueImage.Image = null;
                        }
                    }
                    else
                    {
                        pcbReceptionExemplaireRevueImage.Image = null;
                    }
                }*/
    }
}
