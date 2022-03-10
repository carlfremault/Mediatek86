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
        private List<Livre> lesLivres = new List<Livre>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        private List<CommandeDocument> lesCommandeDocument = new List<CommandeDocument>();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();

        #endregion


        internal FrmMediatek(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
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
        }

        /// <summary>
        /// Evénement de changement d'onglet. Vérifie si une saisie est en cours
        /// Si oui, demande validation de l'utilisateur avant d'abandonner saisie et changer d'onglet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOngletsApplication_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if ((saisieLivre ||saisieDvd || saisieRevue) && !VerifAbandonSaisie())
            {
                e.Cancel = true;
            }
            else
            {
                CancelAllSaisies();
            }            
        }

        #endregion


        #region Revues
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
        /// Désactive boutons 'enregistrer' et 'annuler' qui ne doivent être actifs qu'en cas de modification/ajout d'une revue. 
        /// Met le booleen 'saisieRevue' en 'false'
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
            ActiverBoutonEnregRevue(false);
            ActiverBoutonAnnulerSaisieRevue(false);
            AutoriserModifRevue(false);
            AutoriserModifRevueId(false);
            CancelAllSaisies();
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
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
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
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
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
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

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter une revue
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAjoutRevue(Boolean actif)
        {
            btnAjoutRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier une revue
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonModifRevue(Boolean actif)
        {
            btnModifRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer une revue
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonSupprRevue(Boolean actif)
        {
            btnSupprRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'enregistrer une revue
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonEnregRevue(Boolean actif)
        {
            btnEnregistrerRevue.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'annuler une saisie d'une revue
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAnnulerSaisieRevue(Boolean actif)
        {
            btnAnnulerSaisieRevue.Enabled = actif;
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
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que le bouton 'enregistrer'
        /// </summary>
        /// <param name="actif"></param>
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
            ActiverBoutonEnregRevue(actif);
            ActiverBoutonAnnulerSaisieRevue(actif);
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif"></param>
        private void AutoriserModifRevueId(Boolean actif)
        {
            txbRevuesNumero.ReadOnly = !actif;
        }

        #endregion


        #region Livres

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
        /// Désactive boutons 'enregistrer' et 'annuler' qui ne doivent être actifs qu'en cas de modification/ajout d'un livre. 
        /// Met le booleen 'saisieLivre' en 'false'
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
            ActiverBoutonEnregLivre(false);
            ActiverBoutonAnnulerSaisieLivre(false);
            AutoriserModifLivre(false);
            AutoriserModifLivreId(false);
            CancelAllSaisies();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
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
                    List<Livre> livres = new List<Livre>();
                    livres.Add(livre);
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
                if (cbxLivresGenres.SelectedIndex != -1 &&  VerifAbandonSaisie()) // Le champ de recherche n'est pas vidé, on vérifie si l'utilisateur veut abandonner la saisie
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
        private void FiltreLivresPublic ()
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
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
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
                if(VerifAbandonSaisie())
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
        private void LivresListeSortColumns (DataGridViewCellMouseEventArgs e)
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

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter un livre
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAjoutLivre(Boolean actif)
        {
            btnAjoutLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier un livre
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonModifLivre(Boolean actif)
        {
            btnModifLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer un livre
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonSupprLivre(Boolean actif)
        {
            btnSupprLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'enregistrer un livre
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonEnregLivre(Boolean actif)
        {
            btnEnregistrerLivre.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'annuler une saisie d'un livre
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAnnulerSaisieLivre(Boolean actif)
        {
            btnAnnulerSaisieLivre.Enabled = actif;
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
        /// Evénement clic sur le bouton 'Supprimer'. Vérifie validation de l'utilisateur avant de procéder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprLivre_Click(object sender, EventArgs e)
        {
            if(ValidationSuppression(txbLivresTitre.Text))
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
        /// Evénement clic sur le bouton 'Enregistrer'
        /// Vérifie si les champs requis (numéro, genre, public, rayon) sont saisies.
        /// Si oui procède à l'ajout ou modification du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnregistrerLivre_Click(object sender, EventArgs e)
        {
            if(cbxInfosLivresGenres.SelectedIndex == -1 || cbxInfosLivresPublics.SelectedIndex == -1 || cbxInfosLivresRayons.SelectedIndex == -1 || txbLivresNumero.Text == "" || txbLivresTitre.Text == "")
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
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que le bouton 'enregistrer'
        /// </summary>
        /// <param name="actif"></param>
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
            ActiverBoutonEnregLivre(actif);
            ActiverBoutonAnnulerSaisieLivre(actif);
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif"></param>
        private void AutoriserModifLivreId(Boolean actif)
        {
            txbLivresNumero.ReadOnly = !actif;
        }



        #endregion


        #region Dvd
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
            ActiverBoutonEnregDvd(false);
            ActiverBoutonAnnulerSaisieDvd(false);
            AutoriserModifDvd(false);
            AutoriserModifDvdId(false);
            CancelAllSaisies();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
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
                    List<Dvd> Dvd = new List<Dvd>();
                    Dvd.Add(dvd);
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
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
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

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'ajouter un DVD
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAjoutDvd(Boolean actif)
        {
            btnAjoutDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de modifier un DVD
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonModifDvd(Boolean actif)
        {
            btnModifDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet de supprimer un DVD
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonSupprDvd(Boolean actif)
        {
            btnSupprDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'enregistrer un DVD
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonEnregDvd(Boolean actif)
        {
            btnEnregistrerDvd.Enabled = actif;
        }

        /// <summary>
        /// (Dés)Activer le bouton qui permet d'annuler une saisie d'un DVD
        /// </summary>
        /// <param name="actif"></param>
        private void ActiverBoutonAnnulerSaisieDvd(Boolean actif)
        {
            btnAnnulerSaisieDvd.Enabled = actif;
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
        /// (Dés)activation de La protection des différents champs 'informations détaillées' ainsi que le bouton 'enregistrer'
        /// </summary>
        /// <param name="actif"></param>
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
            ActiverBoutonEnregDvd(actif);
            ActiverBoutonAnnulerSaisieDvd(actif);
        }

        /// <summary>
        /// (Dés)activation de la protection du champ 'Numéro du document'
        /// </summary>
        /// <param name="actif"></param>
        private void AutoriserModifDvdId(Boolean actif)
        {
            txbDvdNumero.ReadOnly = !actif;
        }

        #endregion


        #region Réception Exemplaire de presse
        //-----------------------------------------------------------
        // ONGLET "RECEPTION DE REVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesRevues = controle.GetAllRevues();
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgExemplairesListe.DataSource = exemplaires;
            dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
            dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
            dgvReceptionExemplairesListe.Columns["idDocument"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text.Trim()));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideReceptionRevueInfos();
                }
            }
            else
            {
                VideReceptionRevueInfos();
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            accesReceptionExemplaireGroupBox(false);
            VideReceptionRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            chkReceptionRevueEmpruntable.Checked = revue.Empruntable;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            afficheReceptionExemplairesRevue();
            // accès à la zone d'ajout d'un exemplaire
            accesReceptionExemplaireGroupBox(true);
        }

        private void afficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text.Trim();
            lesExemplaires = controle.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideReceptionRevueInfos()
        {
            txbReceptionRevuePeriodicite.Text = "";
            chkReceptionRevueEmpruntable.Checked = false;
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            lesExemplaires = new List<Exemplaire>();
            RemplirReceptionExemplairesListe(lesExemplaires);
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        private void VideReceptionExemplaireInfos()
        {
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void accesReceptionExemplaireGroupBox(bool acces)
        {
            VideReceptionExemplaireInfos();
            grpReceptionExemplaire.Enabled = acces;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
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
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text.Trim();
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
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
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
        }

        #endregion

        #region Commande de livres
        //-----------------------------------------------------------
        // ONGLET "COMMANDE DE LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivres_Enter(object sender, EventArgs e)
        {
            CancelAllSaisies();
            lesLivres = controle.GetAllLivres();
            // accesCommandeLivresGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommandeLivresListe(List<CommandeDocument> lesCommandeDocument)
        {
            
            bdgCommandesLivresListe.DataSource = lesCommandeDocument;
            dgvCommandeLivresListe.DataSource = bdgCommandesLivresListe;
            dgvCommandeLivresListe.Columns["id"].Visible = false;
            dgvCommandeLivresListe.Columns["idSuivi"].Visible = false;
            dgvCommandeLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeLivresListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeLivresListe.Columns["montant"].DisplayIndex = 1;
            dgvCommandeLivresListe.Columns[4].HeaderCell.Value = "Date";
            dgvCommandeLivresListe.Columns[0].HeaderCell.Value = "Exemplaires";
            dgvCommandeLivresListe.Columns[2].HeaderCell.Value = "Etat";
        }

        /// <summary>
        /// Recherche d'un numéro de livre et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivreRechercher_Click(object sender, EventArgs e)
        {
            if (!txbCommandeLivreNumero.Text.Equals(""))
                {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandeLivreNumero.Text.Trim()));
                if (livre != null)
                {
                    AfficheCommandeLivreInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideCommandeLivresInfos();
                }
            }
            else
            {
                VideCommandeLivresInfos();
            }
        }

        /// <summary>
        /// Si le numéro de livre est modifié, la zone de commande est vidée et inactive
        /// les informations du livre son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbCommandeLivreNumero_TextChanged(object sender, EventArgs e)
        {
            // accesCommandeLivreGroupBox(false);
            VideCommandeLivresInfos();
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné et les commandes
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheCommandeLivreInfos(Livre livre)
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
            AfficheCommandeLivresLivre();

            // accès à la zone d'ajout d'une commande
            // accesCommandeLivresGroupBox(true);
        }

        /// <summary>
        /// Récupération de la liste de commandes d'un livre puis affichage dans la liste
        /// </summary>
        private void AfficheCommandeLivresLivre()
        {
            string idDocument = txbCommandeLivreNumero.Text.Trim();
            lesCommandeDocument = controle.GetCommandeDocument(idDocument);
            RemplirCommandeLivresListe(lesCommandeDocument);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations du livre
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
            // accesCommandeLivresGroupBox(false);
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

        private void dgvCommandeLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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



        #endregion

        #region Commande de DVD
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
            if (!txbCommandeDvdNumero.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandeDvdNumero.Text.Trim()));
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
            // informations sur le livre
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdImage.Text = dvd.Image;
            pcbCommandeDvdImage.Image = null;
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
            AfficheCommandeDvdDvd();

            // accès à la zone d'ajout d'un exemplaire
            // accesCommandeDvdGroupBox(true);
        }

        /// <summary>
        /// Récupération de la liste de commandes d'un DVD puis affichage dans la liste
        /// </summary>
        private void AfficheCommandeDvdDvd()
        {
            string idDocument = txbCommandeDvdNumero.Text.Trim();
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



        #endregion

        #region Abonnements Revue
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
            if (!txbCommandeDvdNumero.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandeDvdNumero.Text.Trim()));
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
            // informations sur le livre
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdImage.Text = dvd.Image;
            pcbCommandeDvdImage.Image = null;
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
            AfficheCommandeDvdDvd();

            // accès à la zone d'ajout d'un exemplaire
            // accesCommandeDvdGroupBox(true);
        }

        /// <summary>
        /// Récupération de la liste de commandes d'un DVD puis affichage dans la liste
        /// </summary>
        private void AfficheCommandeDvdDvd()
        {
            string idDocument = txbCommandeDvdNumero.Text.Trim();
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



        #endregion


    }
}
