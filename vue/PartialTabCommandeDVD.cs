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
        // ONGLET "COMMANDE DE DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeDVD_Enter_1(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesDvd = controle.GetAllDvd();
            // accesCommandeDvdGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommandeDvdListe(List<CommandeDocument> lesCommandeDocument)
        {

            bdgCommandesDvdListe.DataSource = lesCommandeDocument;
            dgvCommandeDvdListe.DataSource = bdgCommandesDvdListe;
            dgvCommandeDvdListe.Columns["id"].Visible = false;
            dgvCommandeDvdListe.Columns["idSuivi"].Visible = false;
            dgvCommandeDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeDvdListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeDvdListe.Columns["montant"].DisplayIndex = 1;
            dgvCommandeDvdListe.Columns[4].HeaderCell.Value = "Date";
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
            if (!txbCommandeDvdNumeroDvd.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandeDvdNumeroDvd.Text.Trim()));
                if (dvd != null)
                {
                    AfficheCommandeDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideCommandeDvdInfos();
                }
            }
            else
            {
                VideCommandeDvdInfos();
            }
        }

        /// <summary>
        /// Entrée dans champ de recherche déclence la recherche aussi
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
        /// Si le numéro de DVD est modifié, la zone de commande est vidée et inactive
        /// les informations du DVD son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeDvdNumero_TextChanged(object sender, EventArgs e)
        {
            // accesCommandeDvdGroupBox(false);
            VideCommandeDvdInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
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
            // accesCommandeDvdGroupBox(true);
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
            // accesCommandeDvdGroupBox(false);
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
        /*        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                {
                    string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
                    List<Exemplaire> sortedList = new List<Exemplaire>();
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
                    RemplirReceptionExemplairesListe(sortedList);
                }*/

        private void dgvCommandeDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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
