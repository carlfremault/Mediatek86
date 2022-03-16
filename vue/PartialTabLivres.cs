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
    /// Classe partielle représentant l'onglet Livres
    /// </summary>
    public partial class FrmMediatek : Form
    {
        //-----------------------------------------------------------
        // ONGLET "LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de faire une saisie pour ajouter/modifier un livre
        /// </summary>
        private bool saisieLivre = false;

        /// <summary>
        /// Booléen, validé comme 'true' si on est en train de modifier un livre
        /// </summary>
        private bool modifLivre = false;

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// Active la protection 'readonly' des champs d'information et désactive boutons 'enregistrer' et 'annuler' qui ne doivent être actifs qu'en cas de modification/ajout d'un Livre.
        /// Tous les booléens concernant une saisie sont mis en false (validation d'abandon a été demandé avant changement d'onglet)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboCategorie(controle.GetAllGenres(), bdgInfosGenres, cbxInfosLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgInfosPublics, cbxInfosLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgInfosRayons, cbxInfosLivresRayons);
            RemplirLivresListeComplete();
            LivresListeSelection();
            AutoriserModifLivre(false);
            AutoriserModifLivreId(false);
            CancelAllSaisies();
        }

        #region dataGridView + fonctions et événements associées

        /// <summary>
        /// Remplit le dategrid avec la collection reçue en paramètre
        /// </summary>
        /// <param name="livres">Collection des livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// Vérifie si on est en train de faire une saisie
        /// Si non, affiche infos livre sélectionné
        /// Si oui, vérifie si l'utilisateur veut abandonner la saisie d'abord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (saisieLivre)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieLivre();
                    LivresListeSelection();
                }
            }
            else
            {
                LivresListeSelection();
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné dans le grid
        /// Désactive saisie et verouille les champs infos
        /// </summary>
        private void LivresListeSelection()
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    ActiverBoutonModifLivre(true);
                    ActiverBoutonSupprLivre(true);
                    AutoriserModifLivre(false);
                    saisieLivre = false;
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
                ActiverBoutonModifLivre(false);
                ActiverBoutonSupprLivre(false);
            }
        }

        /// <summary>
        /// Affichage de la liste complète des livres et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// Vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Vérification si on est en train de saisir. 
        /// Si non lance le tri sur les colonnes.
        /// Si oui demande confirmation d'abandonner saisie d'abord.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (saisieLivre)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieLivre();
                    LivresListeSortColumns(e);
                }
            }
            else
            {
                LivresListeSortColumns(e);
            }
        }

        /// <summary>
        /// Tri sur les colonnes.
        /// </summary>
        /// <param name="e"></param>
        private void LivresListeSortColumns(DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        #endregion

        #region infos livre

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            cbxInfosLivresGenres.SelectedIndex = cbxInfosLivresGenres.FindStringExact(livre.Genre);
            cbxInfosLivresPublics.SelectedIndex = cbxInfosLivresPublics.FindStringExact(livre.Public);
            cbxInfosLivresRayons.SelectedIndex = cbxInfosLivresRayons.FindStringExact(livre.Rayon);
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            cbxInfosLivresGenres.SelectedIndex = -1;
            cbxInfosLivresPublics.SelectedIndex = -1;
            cbxInfosLivresRayons.SelectedIndex = -1;
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        #endregion

        #region événements et fonctions associées

        /// <summary>
        /// Evénement clic sur le bouton 'Rechercher'. Vérifie si on est en train de faire une saisie (ajout ou modif de livre)
        /// Si non : effectue la recherche
        /// Si oui : demande si l'utilisateur veut abandonner la saisie, si oui  : abandon, vide les champs 'infos' et effectue la recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (saisieLivre && VerifAbandonSaisie()) // On est en train de saisir, et on décide d'abandonner
            {
                StopSaisieLivre();
                LivresNumRecherche();
            }
            else if (saisieLivre) // On est en train de saisir, et on n'abandonne pas
            {
                txbLivresNumRecherche.Text = "";
            }
            else // On n'est pas en train de saisir
            {
                LivresNumRecherche();
            }
        }

        /// <summary>
        /// Taper Entrée dans champ recherche déclenche la recherche aussi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbLivresNumRecherche_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnLivresNumRecherche_Click(sender, e);
            }
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        private void LivresNumRecherche()
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text.Trim()));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>
                    {
                        livre
                    };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère dans le textBox de recherche de titre.
        /// Vérifie si on est en train de faire une saisie (ajout ou modif de livre)
        /// Si non : effectue la recherche
        /// Si oui : vérification si l'utilisateur veut abandonner la saisie et si le champs n'est pas vidé (pour éviter double trigger)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (saisieLivre) // On est en train de saisir
            {
                if (txbLivresTitreRecherche.Text != "" && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieLivre();
                    LivresTitreRecherche();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    txbLivresTitreRecherche.Text = "";
                }
            }
            else // On n'est pas en train de saisir
            {
                LivresTitreRecherche();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// </summary>
        private void LivresTitreRecherche()
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
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
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieLivre) // On est en train de saisir
            {
                if (cbxLivresGenres.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieLivre();
                    VideLivresInfos();
                    FiltreLivresGenre();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxLivresGenres.SelectedIndex = -1;
                }

            }
            else // On n'est pas en train de saisir
            {
                FiltreLivresGenre();
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        private void FiltreLivresGenre()
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
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
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieLivre) // On est en train de saisir
            {
                if (cbxLivresPublics.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieLivre();
                    VideLivresInfos();
                    FiltreLivresPublic();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxLivresPublics.SelectedIndex = -1;
                }
            }
            else // On n'est pas en train de saisir
            {
                FiltreLivresPublic();
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        private void FiltreLivresPublic()
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
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
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (saisieLivre) // On est en train de saisir
            {
                if (cbxLivresRayons.SelectedIndex != -1 && VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
                {
                    StopSaisieLivre();
                    VideLivresInfos();
                    FiltreLivresRayon();
                }
                else // L'utilisateur ne veut pas abandonner la saisie
                {
                    cbxLivresRayons.SelectedIndex = -1;
                }

            }
            else // On n'est pas en train de saisir
            {
                FiltreLivresRayon();
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        private void FiltreLivresRayon()
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboLivres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboLivres();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboLivres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboLivres();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, appel de la fonction AnnulFiltreCboLivres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            AnnulFiltreCboLivres();
        }

        /// <summary>
        /// Sur le clic d'un bouton d'annulation, vérification si on est en train de faire une saisie.
        /// Si non affichage de la liste complète des livres.
        /// Si oui, verification si l'utilisateur veut abandonner sa saisie d'abord.
        /// </summary>
        private void AnnulFiltreCboLivres()
        {
            if (saisieLivre)
            {
                if (VerifAbandonSaisie())
                {
                    StopSaisieLivre();
                    RemplirLivresListeComplete();
                    LivresListeSelection();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Ajouter'. Vide les champs 'infos', déverrouille le champ 'Numéro de document', Démarre la saisie d'un livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjoutLivre_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
            AutoriserModifLivreId(true);
            StartSaisieLivre();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Modifier'. Verrouille le champ 'Numéro de document'. Démarre la saisie d'un livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifLivre_Click(object sender, EventArgs e)
        {
            modifLivre = true;
            AutoriserModifLivreId(false);
            StartSaisieLivre();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Supprimer'. Demande validation de l'utilisateur avant de procéder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprLivre_Click(object sender, EventArgs e)
        {
            if (ValidationSuppression(txbLivresTitre.Text))
            {
                if (controle.SupprLivre(txbLivresNumero.Text.Trim()))
                {
                    controle.RefreshAllLivres();
                    lesLivres = controle.GetAllLivres();
                    RemplirLivresListeComplete();
                }
                else
                {
                    MessageBox.Show("Il n'est pas possible de supprimer ce livre car ils existent un ou plusieurs exemplaires ou commandes le concernant.", "Erreur");
                }
            }
        }

        /// <summary>
        /// Recherche image du livre sur disque 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreRechercheImage_Click(object sender, EventArgs e)
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
            txbLivresImage.Text = filePath;
            try
            {
                pcbLivresImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Evénement clic sur le bouton 'Enregistrer'
        /// Vérifie si les champs requis (numéro, genre, public, rayon) sont saisies.
        /// Si oui procède à l'ajout ou modification du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerLivre_Click(object sender, EventArgs e)
        {
            if (cbxInfosLivresGenres.SelectedIndex == -1 || cbxInfosLivresPublics.SelectedIndex == -1 || cbxInfosLivresRayons.SelectedIndex == -1 || txbLivresNumero.Text == "" || txbLivresTitre.Text == "")
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
            String id = txbLivresNumero.Text.Trim();
            String titre = txbLivresTitre.Text;
            String image = txbLivresImage.Text;
            String isbn = txbLivresIsbn.Text;
            String auteur = txbLivresAuteur.Text;
            String collection = txbLivresCollection.Text;
            Livre leLivre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, unPublic, idRayon, rayon);

            if (modifLivre)
            {
                if (!controle.ModifLivre(leLivre))
                {
                    MessageBox.Show("Une erreur est survenue.", "Erreur");
                    return;
                }
            }
            else
            {
                String message = controle.CreerLivre(leLivre);
                if (message.Substring(0, 5) == "Ajout")
                {
                    MessageBox.Show(message, "Information");
                }
                else if (message.Substring(0, 9) == "Duplicate")
                {
                    MessageBox.Show("Ce numéro de publication existe déjà.", "Erreur");
                    txbLivresNumero.Text = "";
                    txbLivresNumero.Focus();
                    return;
                }
                else
                {
                    MessageBox.Show(message, "Erreur");
                    return;
                }
            }
            VideLivresInfos();
            StopSaisieLivre();
            controle.RefreshAllLivres();
            lesLivres = controle.GetAllLivres();
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Evénement clic sur le bouton annuler lors d'une saisie
        /// Vide les champs infos, arrête la saisie et affiche les infos du livre sélectionné dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerSaisieLivre_Click(object sender, EventArgs e)
        {
            if (VerifAbandonSaisie())
            {
                VideLivresInfos();
                StopSaisieLivre();
                LivresListeSelection();
            }
        }

        #endregion

        #region sécurisation

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter un livre
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonAjoutLivre(Boolean actif)
        {
            btnAjoutLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier un livre
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonModifLivre(Boolean actif)
        {
            btnModifLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer un livre
        /// </summary>
        /// <param name="actif">'True' active, 'False' désactive</param>
        private void ActiverBoutonSupprLivre(Boolean actif)
        {
            btnSupprLivre.Enabled = actif;
        }

        /// <summary>
        /// Démarre la saisie d'un livre, déverouille les champs 'infos'
        /// </summary>
        private void StartSaisieLivre()
        {
            saisieLivre = true;
            AutoriserModifLivre(true);
            ActiverBoutonAjoutLivre(false);
            ActiverBoutonModifLivre(false);
            ActiverBoutonSupprLivre(false);
        }

        /// <summary>
        /// Arrête la saisie d'un livre, verouille les champs 'infos'
        /// </summary>
        private void StopSaisieLivre()
        {
            CancelAllSaisies();
            AutoriserModifLivre(false);
            ActiverBoutonAjoutLivre(true);
            ActiverBoutonModifLivre(true);
            ActiverBoutonSupprLivre(true);
        }

        /// <summary>
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que les boutons 'enregistrer', 'annuler' et de recherche d'image
        /// </summary>
        /// <param name="actif">'True' déverrouille les champs, active les boutons</param>
        private void AutoriserModifLivre(Boolean actif)
        {
            txbLivresTitre.ReadOnly = !actif;
            txbLivresAuteur.ReadOnly = !actif;
            txbLivresCollection.ReadOnly = !actif;
            txbLivresImage.ReadOnly = !actif;
            txbLivresIsbn.ReadOnly = !actif;
            cbxInfosLivresGenres.Enabled = actif;
            cbxInfosLivresPublics.Enabled = actif;
            cbxInfosLivresRayons.Enabled = actif;
            btnEnregistrerLivre.Enabled = actif;
            btnAnnulerSaisieLivre.Enabled = actif;
            btnLivreRechercheImage.Enabled = actif;
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif">'True' enlève la protection</param>
        private void AutoriserModifLivreId(Boolean actif)
        {
            txbLivresNumero.ReadOnly = !actif;
        }

        #endregion
    }
}
