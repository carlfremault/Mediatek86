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
    /// Classe partielle représentant l'onglet Revues
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "Revues"
        //------------------------------------------------------------

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de faire une saisie pour ajouter/modifier une revue
        /// </summary>
        private bool saisieRevue = false;

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de modifier une revue
        /// </summary>
        private bool modifRevue = false;

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// Active la protection 'readonly' des champs d'information et désactive boutons 'enregistrer' et 'annuler' qui ne doivent être actifs qu'en cas de modification/ajout d'une Revue.
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirComboCategorie(controle.GetAllGenres(), bdgInfosGenres, cbxInfosRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgInfosPublics, cbxInfosRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgInfosRayons, cbxInfosRevuesRayons);
            RemplirRevuesListeComplete();
            RevuesListeSelection();
            AutoriserModifRevue(false);
            AutoriserModifRevueId(false);
            CancelAllSaisies();
        }

        #region dataGridView + fonctions et événements associées

        /// <summary>
        /// Remplit le datagrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="revues">La collection de revues</param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["empruntable"].Visible = false;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// Vérifie si on est en train de faire une saisie
        /// Si non, affiche infos livre sélectionné
        /// Si oui, vérifie si l'utilisateur veut abandonner la saisie d'abord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieRevue)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieRevue();
                    RevuesListeSelection();
                }
            }
            else
            {
                RevuesListeSelection();
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée dans le grid
        /// Désactive saisie et verouille les champs infos
        /// </summary>
        private void RevuesListeSelection()
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                    ActiverBoutonModifRevue(true);
                    ActiverBoutonSupprRevue(true);
                    AutoriserModifRevue(false);
                    saisieRevue = false;
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
                ActiverBoutonModifRevue(false);
                ActiverBoutonSupprRevue(false);
            }
        }

        /// <summary>
        /// Affichage de la liste complète des revues et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// Vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (saisieRevue)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieRevue();
                    RevuesListeSortColumns(e);
                }
            }
            else
            {
                RevuesListeSortColumns(e);
            }
        }

        /// <summary>
        /// Tri sur les colonnes.
        /// </summary>
        /// <param name="e"></param>
        private void RevuesListeSortColumns(DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        #endregion

        #region infos revue

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            chkRevuesEmpruntable.Checked = revue.Empruntable;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            cbxInfosRevuesGenres.SelectedIndex = cbxInfosRevuesGenres.FindStringExact(revue.Genre);
            cbxInfosRevuesPublics.SelectedIndex = cbxInfosRevuesPublics.FindStringExact(revue.Public);
            cbxInfosRevuesRayons.SelectedIndex = cbxInfosRevuesRayons.FindStringExact(revue.Rayon);
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            chkRevuesEmpruntable.Checked = false;
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            cbxInfosRevuesGenres.SelectedIndex = -1;
            cbxInfosRevuesPublics.SelectedIndex = -1;
            cbxInfosRevuesRayons.SelectedIndex = -1;
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        #endregion

        #region événements et fonctions associées

        /// <summary>
        /// Evénement clic sur le bouton 'Rechercher'. Vérifie si on est en train de faire une saisie (ajout ou modif de revue)
        /// Si non : effectue la recherche
        /// Si oui : demande si l'utilisateur veut abandonner la saisie, si oui  : abandon, vide les champs 'infos' et effectue la recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (saisieRevue && VerifAbandonSaisie()) // On est en train de saisir, et on décide d'abandonner
            {
                StopSaisieRevue();
                RevuesNumRecherche();
            }
            else if (saisieRevue) // On est en train de saisir, et on n'abandonne pas
            {
                txbRevuesNumRecherche.Text = "";
            }
            else // On n'est pas en train de saisir
            {
                RevuesNumRecherche();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ recherche déclenche la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesNumRecherche_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnRevuesNumRecherche_Click(sender, e);
            }
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        private void RevuesNumRecherche()
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text.Trim()));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>
                    {
                        revue
                    };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (saisieRevue) // On est en train de saisir
            {
                if (txbRevuesTitreRecherche.Text != "" && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieRevue();
                    RevuesTitreRecherche();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    txbRevuesTitreRecherche.Text = "";
                }
            }
            else // On n'est pas en train de saisir
            {
                RevuesTitreRecherche();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// </summary>
        private void RevuesTitreRecherche()
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
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
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieRevue) // On est en train de saisir
            {
                if (cbxRevuesGenres.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieRevue();
                    VideRevuesInfos();
                    FiltreRevuesGenre();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxRevuesGenres.SelectedIndex = -1;
                }
            }
            else // On n'est pas en train de saisir
            {
                FiltreRevuesGenre();
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        private void FiltreRevuesGenre()
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
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
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieRevue) // On est en train de saisir
            {
                if (cbxRevuesPublics.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieRevue();
                    VideRevuesInfos();
                    FiltreRevuesPublic();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxRevuesPublics.SelectedIndex = -1;
                }
            }
            else // On n'est pas en train de saisir
            {
                FiltreRevuesPublic();
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        private void FiltreRevuesPublic()
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
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
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieRevue) // On est en train de saisir
            {
                if (cbxRevuesRayons.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieRevue();
                    VideRevuesInfos();
                    FiltreRevuesRayon();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxRevuesRayons.SelectedIndex = -1;
                }
            }
            else // On n'est pas en train de saisir
            {
                FiltreRevuesRayon();
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        private void FiltreRevuesRayon()
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation,  appel de la fonction AnnulFiltreCboRevues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboRevues();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboRevues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboRevues();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboRevues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboRevues();
        }

        /// <summary>
        /// Sur le clic d'un bouton d'annulation, vérification si on est en train de faire une saisie.
        /// Si non affichage de la liste complète des revues.
        /// Si oui, verification si l'utilisateur veut abandonner sa saisie d'abord.
        /// </summary>
        private void AnnulFiltreCboRevues()
        {
            if (saisieRevue)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieRevue();
                    RemplirRevuesListeComplete();
                    RevuesListeSelection();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Ajouter'. Vide les champs 'infos', déverrouille le champ 'Numéro de document', Démarre la saisie d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjoutRevue_Click(object sender, EventArgs e)
        {
            VideRevuesInfos();
            AutoriserModifRevueId(true);
            StartSaisieRevue();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Modifier'. Verrouille le champ 'Numéro de document'. Démarre la saisie d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifRevue_Click(object sender, EventArgs e)
        {
            modifRevue = true;
            AutoriserModifRevueId(false);
            StartSaisieRevue();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Supprimer'. Vérifie validation de l'utilisateur avant de procéder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprRevue_Click(object sender, EventArgs e)
        {
            if (ValidationSuppression(txbRevuesTitre.Text))
            {
                if (controle.SupprRevue(txbRevuesNumero.Text.Trim()))
                {
                    controle.RefreshAllRevues();
                    lesRevues = controle.GetAllRevues();
                    RemplirRevuesListeComplete();
                }
                else
                {
                    MessageBox.Show("Il n'est pas possible de supprimer cette revue car ils existent un ou plusieurs exemplaires ou commandes le concernant.", "Erreur");
                }
            }
        }

        /// <summary>
        /// Recherche image de la revue sur disque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesRechercheImage_Click(object sender, EventArgs e)
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
            txbRevuesImage.Text = filePath;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Enregistrer'
        /// Vérifie si les champs requis (numéro, genre, public, rayon) sont saisies.
        /// Si oui procède à l'ajout ou modification de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerRevue_Click(object sender, EventArgs e)
        {
            if (cbxInfosRevuesGenres.SelectedIndex == -1 || cbxInfosRevuesPublics.SelectedIndex == -1 || cbxInfosRevuesRayons.SelectedIndex == -1 || txbRevuesNumero.Text == "" || txbRevuesTitre.Text == "" || txbRevuesPeriodicite.Text == "" || txbRevuesDateMiseADispo.Text == "")
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
            String id = txbRevuesNumero.Text.Trim();
            String titre = txbRevuesTitre.Text;
            String periodicite = txbRevuesPeriodicite.Text;
            String image = txbRevuesImage.Text;
            Boolean empruntable = chkRevuesEmpruntable.Checked;
            int delaiMiseADispo = 0;
            if (txbRevuesDateMiseADispo.Text != "")
            {
                bool success = int.TryParse(txbRevuesDateMiseADispo.Text, out delaiMiseADispo);
                if (!success)
                {
                    MessageBox.Show("La valeur saisie pour le délai de mise à dispo doit être un entier.", "Erreur");
                    txbRevuesDateMiseADispo.Text = "";
                    txbRevuesDateMiseADispo.Focus();
                    return;
                }
            }
            Revue laRevue = new Revue(id, titre, image, idGenre, genre, idPublic, unPublic, idRayon, rayon, empruntable, periodicite, delaiMiseADispo);

            if (modifRevue)
            {
                if (!controle.ModifRevue(laRevue))
                {
                    MessageBox.Show("Une erreur est survenue.", "Erreur");
                    return;
                }
            }
            else
            {
                String message = controle.CreerRevue(laRevue);
                if (message.Substring(0, 5) == "Ajout")
                {
                    MessageBox.Show(message, "Information");
                }
                else if (message.Substring(0, 9) == "Duplicate")
                {
                    MessageBox.Show("Ce numéro de publication existe déjà.", "Erreur");
                    txbRevuesNumero.Text = "";
                    txbRevuesNumero.Focus();
                    return;
                }
                else
                {
                    MessageBox.Show(message, "Erreur");
                    return;
                }
            }

            VideRevuesInfos();
            StopSaisieRevue();
            controle.RefreshAllRevues();
            lesRevues = controle.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Evénement clic sur le bouton annuler lors d'une saisie
        /// Vide les champs infos, arrête la saisie et affiche les infos de la revue sélectionnée dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerSaisieRevue_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                VideRevuesInfos();
                StopSaisieRevue();
                RevuesListeSelection();
            }
        }

        #endregion

        #region sécurisation

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter une revue
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonAjoutRevue(Boolean actif)
        {
            btnAjoutRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier une revue
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonModifRevue(Boolean actif)
        {
            btnModifRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer une revue
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonSupprRevue(Boolean actif)
        {
            btnSupprRevue.Enabled = actif;
        }

        /// <summary>
        /// Démarre la saisie d'une revue, déverouille les champs 'infos'
        /// </summary>
        private void StartSaisieRevue()
        {
            saisieRevue = true;
            AutoriserModifRevue(true);
            ActiverBoutonAjoutRevue(false);
            ActiverBoutonModifRevue(false);
            ActiverBoutonSupprRevue(false);
        }

        /// <summary>
        /// Arrête la saisie d'une revue, verouille les champs 'infos'
        /// </summary>
        private void StopSaisieRevue()
        {
            CancelAllSaisies();
            AutoriserModifRevue(false);
            ActiverBoutonAjoutRevue(true);
            ActiverBoutonModifRevue(true);
            ActiverBoutonSupprRevue(true);
        }

        /// <summary>
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que les boutons 'enregistrer', 'annuler' et de recherche d'image
        /// </summary>
        /// <param name="actif">'True' déverrouille les champs, active les boutons</param>
        private void AutoriserModifRevue(Boolean actif)
        {
            txbRevuesTitre.ReadOnly = !actif;
            txbRevuesPeriodicite.ReadOnly = !actif;
            txbRevuesDateMiseADispo.ReadOnly = !actif;
            txbRevuesImage.ReadOnly = !actif;
            chkRevuesEmpruntable.Enabled = actif;
            cbxInfosRevuesGenres.Enabled = actif;
            cbxInfosRevuesPublics.Enabled = actif;
            cbxInfosRevuesRayons.Enabled = actif;
            btnEnregistrerRevue.Enabled = actif;
            btnAnnulerSaisieRevue.Enabled = actif;
            btnRevuesRechercheImage.Enabled = actif;
            
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif">'True' enlève la protection</param>
        private void AutoriserModifRevueId(Boolean actif)
        {
            txbRevuesNumero.ReadOnly = !actif;
        }

        #endregion     
    }
}
