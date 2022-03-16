using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Les vues de l'application
/// </summary>
namespace Mediatek86.vue
{
    /// <summary>
    /// Classe partielle représentant l'onglet DVD
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de faire une saisie pour ajouter/modifier un DVD
        /// </summary>
        private bool saisieDvd = false;

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de modifier un DVD
        /// </summary>
        private bool modifDvd = false;

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// Active la protection 'readonly' des champs d'information et désactive boutons 'enregistrer' et 'annuler' qui ne doivent être actifs qu'en cas de modification/ajout d'un DVD. 
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboCategorie(controle.GetAllGenres(), bdgInfosGenres, cbxInfosDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgInfosPublics, cbxInfosDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgInfosRayons, cbxInfosDvdRayons);
            RemplirDvdListeComplete();
            DvdListeSelection();
            AutoriserModifDvd(false);
            AutoriserModifDvdId(false);
            CancelAllSaisies();
        }

        #region dataGridView + fonctions et événements associées

        /// <summary>
        /// Remplit le dategrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="Dvds">La collection de DVD</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// Vérifie si on est en train de faire une saisie
        /// Si non, affiche infos livre sélectionné
        /// Si oui, vérifie si l'utilisateur veut abandonner la saisie d'abord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieDvd)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieDvd();
                    DvdListeSelection();
                }
            }
            else
            {
                DvdListeSelection();
            }
        }

        /// <summary>
        /// Affichage des informations du DVD sélectionné dans le grid
        /// Désactive saisie et verouille les champs infos
        /// </summary>
        private void DvdListeSelection()
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    ActiverBoutonModifDvd(true);
                    ActiverBoutonSupprDvd(true);
                    AutoriserModifDvd(false);
                    saisieDvd = false;
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
                ActiverBoutonModifDvd(false);
                ActiverBoutonSupprDvd(false);
            }
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// Vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Vérification si on est en train de saisir. 
        /// Si non lance le tri sur les colonnes.
        /// Si oui demande confirmation d'abandonner saisie d'abord.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (saisieDvd)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieDvd();
                    DvdListeSortColumns(e);
                }
            }
            else
            {
                DvdListeSortColumns(e);
            }
        }

        /// <summary>
        /// Tri sur les colonnes.
        /// </summary>
        /// <param name="e"></param>
        private void DvdListeSortColumns(DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        #endregion

        #region infos DVD

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            cbxInfosDvdGenres.SelectedIndex = cbxInfosDvdGenres.FindStringExact(dvd.Genre);
            cbxInfosDvdPublics.SelectedIndex = cbxInfosDvdPublics.FindStringExact(dvd.Public);
            cbxInfosDvdRayons.SelectedIndex = cbxInfosDvdRayons.FindStringExact(dvd.Rayon);
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            cbxInfosDvdGenres.SelectedIndex = -1;
            cbxInfosDvdPublics.SelectedIndex = -1;
            cbxInfosDvdRayons.SelectedIndex = -1;
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        #endregion

        #region événements et fonctions associées

        /// <summary>
        /// Evénement clic sur le bouton 'Rechercher'. Vérifie si on est en train de faire une saisie (ajout ou modif de DVD)
        /// Si non : effectue la recherche
        /// Si oui : demande si l'utilisateur veut abandonner la saisie, si oui  : abandon, vide les champs 'infos' et effectue la recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (saisieDvd && VerifAbandonSaisie()) // On est en train de saisir, et on décide d'abandonner
            {
                StopSaisieDvd();
                DvdNumRecherche();
            }
            else if (saisieDvd) // On est en train de saisir, et on n'abandonne pas
            {
                txbDvdNumRecherche.Text = "";
            }
            else // On n'est pas en train de saisir
            {
                DvdNumRecherche();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ recherche déclenche la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdNumRecherche_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnDvdNumRecherche_Click(sender, e);
            }
        }

        /// <summary>
        /// Recherche et affichage du DVD dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        private void DvdNumRecherche()
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text.Trim()));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>
                    {
                        dvd
                    };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère dans le textBox de recherche de titre.
        /// Vérifie si on est en train de faire une saisie (ajout ou modif de DVD)
        /// Si non : effectue la recherche
        /// Si oui : vérification si l'utilisateur veut abandonner la saisie et si le champs n'est pas vidé (pour éviter double trigger)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (saisieDvd) // On est en train de saisir
            {
                if (txbDvdTitreRecherche.Text != "" && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieDvd();
                    DvdTitreRecherche();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    txbDvdTitreRecherche.Text = "";
                }
            }
            else // On n'est pas en train de saisir
            {
                DvdTitreRecherche();
            }
        }

        /// <summary>
        /// Recherche et affichage des DVD dont le titre matche acec la saisie.
        /// </summary>
        private void DvdTitreRecherche()
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Evénement changement de sélection combobox Genres
        /// On vérifie si l'utilisateur est en train de faire une saisie
        /// Si non, le filtre s'exécute
        /// Si oui, vérification s'il veut abandonner la saisie, et si la sélection n'est pas vidée (pour éviter double trigger)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieDvd) // On est en train de saisir
            {
                if (cbxDvdGenres.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieDvd();
                    VideDvdInfos();
                    FiltreDvdGenre();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxDvdGenres.SelectedIndex = -1;
                }

            }
            else // On n'est pas en train de saisir
            {
                FiltreDvdGenre();
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        private void FiltreDvdGenre()
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Evénement changement de sélection combobox Publics
        /// On vérifie si l'utilisateur est en train de faire une saisie
        /// Si non, le filtre s'exécute
        /// Si oui, vérification s'il veut abandonner la saisie, et si la sélection n'est pas vidée (pour éviter double trigger)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieDvd) // On est en train de saisir
            {
                if (cbxDvdPublics.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieDvd();
                    VideDvdInfos();
                    FiltreDvdPublic();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxDvdPublics.SelectedIndex = -1;
                }
            }
            else // On n'est pas en train de saisir
            {
                FiltreDvdPublic();
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        private void FiltreDvdPublic()
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Evénement changement de sélection combobox Rayons
        /// On vérifie si l'utilisateur est en train de faire une saisie
        /// Si non, le filtre s'exécute
        /// Si oui, vérification s'il veut abandonner la saisie, et si la sélection n'est pas vidée (pour éviter double trigger)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieDvd) // On est en train de saisir
            {
                if (cbxDvdRayons.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieDvd();
                    VideDvdInfos();
                    FiltreDvdRayon();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxDvdRayons.SelectedIndex = -1;
                }

            }
            else // On n'est pas en train de saisir
            {
                FiltreDvdRayon();
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        private void FiltreDvdRayon()
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboDvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboDvd();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboDvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboDvd();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboDvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboDvd();
        }

        /// <summary>
        /// Sur le clic d'un bouton d'annulation, vérification si on est en train de faire une saisie.
        /// Si non affichage de la liste complète des DVD.
        /// Si oui, verification si l'utilisateur veut abandonner sa saisie d'abord.
        /// </summary>
        private void AnnulFiltreCboDvd()
        {
            if (saisieDvd)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieDvd();
                    RemplirDvdListeComplete();
                    DvdListeSelection();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Ajouter'. Vide les champs 'infos', déverrouille le champ 'Numéro de document', Démarre la saisie d'un DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjoutDvd_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
            txbDvdRealisateur.Text = "";
            AutoriserModifDvdId(true);
            StartSaisieDvd();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Modifier'. Verrouille le champ 'Numéro de document'. Démarre la saisie d'un DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifDvd_Click(object sender, EventArgs e)
        {
            modifDvd = true;
            AutoriserModifDvdId(false);
            StartSaisieDvd();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Supprimer'. Vérifie validation de l'utilisateur avant de procéder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprDvd_Click(object sender, EventArgs e)
        {
            if (ValidationSuppression(txbDvdTitre.Text))
            {
                if (controle.SupprDvd(txbDvdNumero.Text.Trim()))
                {
                    controle.RefreshAllDvd();
                    lesDvd = controle.GetAllDvd();
                    RemplirDvdListeComplete();
                }
                else
                {
                    MessageBox.Show("Il n'est pas possible de supprimer ce DVD car ils existent un ou plusieurs exemplaires ou commandes le concernant.", "Erreur");
                }
            }
        }

        /// <summary>
        /// Recherche image du DVD sur disque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdRechercheImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = DOSSIERINITIALRECHERCHEIMAGE,
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbDvdImage.Text = filePath;
            try
            {
                pcbDvdImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Enregistrer'
        /// Vérifie si les champs requis (numéro, genre, public, rayon) sont saisies.
        /// Si oui procède à l'ajout ou modification du DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerDvd_Click(object sender, EventArgs e)
        {
            if (cbxInfosDvdGenres.SelectedIndex == -1 || cbxInfosDvdPublics.SelectedIndex == -1 || cbxInfosDvdRayons.SelectedIndex == -1 || txbDvdNumero.Text == "" || txbDvdRealisateur.Text == "" || txbDvdTitre.Text == "" || txbDvdDuree.Text == "")
            {
                MessageBox.Show("Les champs marqués d'un astérisque (*) sont obligatoires.", "Information");
                return;
            }

            Genre leGenre = (Genre)bdgInfosGenres.List[bdgInfosGenres.Position];
            String idGenre = leGenre.Id;
            String genre = leGenre.ToString();
            Public lePublic = (Public)bdgInfosPublics.List[bdgInfosPublics.Position];
            String idPublic = lePublic.Id;
            String unPublic = lePublic.ToString();
            Rayon leRayon = (Rayon)bdgInfosRayons.List[bdgInfosRayons.Position];
            String idRayon = leRayon.Id;
            String rayon = leRayon.ToString();
            String id = txbDvdNumero.Text.Trim();
            String titre = txbDvdTitre.Text;
            String realisateur = txbDvdRealisateur.Text;
            String synopsis = txbDvdSynopsis.Text;
            String image = txbDvdImage.Text;
            int duree = 0;
            if (txbDvdDuree.Text != "")
            {
                bool success = int.TryParse(txbDvdDuree.Text, out duree);
                if (!success)
                {
                    MessageBox.Show("La valeur saisie pour la durée doit être un entier.", "Erreur");
                    txbDvdDuree.Text = "";
                    txbDvdDuree.Focus();
                    return;
                }
            }
            Dvd leDvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, unPublic, idRayon, rayon);

            if (modifDvd)
            {
                if (!controle.ModifDvd(leDvd))
                {
                    MessageBox.Show("Une erreur est survenue.", "Erreur");
                    return;
                }
            }
            else
            {
                String message = controle.CreerDvd(leDvd);
                if (message.Substring(0, 5) == "Ajout")
                {
                    MessageBox.Show(message, "Information");
                }
                else if (message.Substring(0, 9) == "Duplicate")
                {
                    MessageBox.Show("Ce numéro de publication existe déjà.", "Erreur");
                    txbDvdNumero.Text = "";
                    txbDvdNumero.Focus();
                    return;
                }
                else
                {
                    MessageBox.Show(message, "Erreur");
                    return;
                }
            }

            VideDvdInfos();
            StopSaisieDvd();
            controle.RefreshAllDvd();
            lesDvd = controle.GetAllDvd();
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Evénement clic sur le bouton annuler lors d'une saisie
        /// Vide les champs infos, arrête la saisie et affiche les infos du DVD sélectionné dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerSaisieDvd_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                VideDvdInfos();
                StopSaisieDvd();
                DvdListeSelection();
            }
        }

        #endregion

        #region sécurisation

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter un DVD
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonAjoutDvd(Boolean actif)
        {
            btnAjoutDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier un DVD
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonModifDvd(Boolean actif)
        {
            btnModifDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer un DVD
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonSupprDvd(Boolean actif)
        {
            btnSupprDvd.Enabled = actif;
        }

        /// <summary>
        /// Démarre la saisie d'un DVD, déverouille les champs 'infos'
        /// </summary>
        private void StartSaisieDvd()
        {
            saisieDvd = true;
            AutoriserModifDvd(true);
            ActiverBoutonAjoutDvd(false);
            ActiverBoutonModifDvd(false);
            ActiverBoutonSupprDvd(false);
        }

        /// <summary>
        /// Arrête la saisie d'un DVD, verouille les champs 'infos'
        /// </summary>
        private void StopSaisieDvd()
        {
            CancelAllSaisies();
            AutoriserModifDvd(false);
            ActiverBoutonAjoutDvd(true);
            ActiverBoutonModifDvd(true);
            ActiverBoutonSupprDvd(true);
        }

        /// <summary>
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que les boutons 'enregistrer', 'annuler' et de recherche d'image
        /// </summary>
        /// <param name="actif">'True' déverrouille les champs, active les boutons</param>
        private void AutoriserModifDvd(Boolean actif)
        {
            txbDvdTitre.ReadOnly = !actif;
            txbDvdRealisateur.ReadOnly = !actif;
            txbDvdSynopsis.ReadOnly = !actif;
            txbDvdImage.ReadOnly = !actif;
            txbDvdDuree.ReadOnly = !actif;
            cbxInfosDvdGenres.Enabled = actif;
            cbxInfosDvdPublics.Enabled = actif;
            cbxInfosDvdRayons.Enabled = actif;
            btnEnregistrerDvd.Enabled = actif;
            btnAnnulerSaisieDvd.Enabled = actif;
            btnDvdRechercheImage.Enabled = actif;
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif">'True' enlève la protection</param>
        private void AutoriserModifDvdId(Boolean actif)
        {
            txbDvdNumero.ReadOnly = !actif;
        }

        #endregion
    }
}
