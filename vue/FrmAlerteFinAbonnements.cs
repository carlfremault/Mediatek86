using Mediatek86.controleur;
using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

/// <summary>
/// Les vues de l'application
/// </summary>
namespace Mediatek86.vue
{
    /// <summary>
    /// Vue pour l'alerte d'abonnements qui expirent dans moins de 30 jours
    /// </summary>
    public partial class FrmAlerteFinAbonnements : Form
    {
        /// <summary>
        /// BindingSource pour la liste des abonnements arrivant à expiration
        /// </summary>
        private readonly BindingSource bdgAlerteAbonnements = new BindingSource();

        /// <summary>
        /// Collection des abonnements arrivant à expiration
        /// </summary>
        private readonly List<FinAbonnement> lesFinAbonnement;

        /// <summary>
        /// Constructeur. Valorise la propriété contrôleur avec le contrôleur reçu en paramètre.
        /// Remplit le tableau des abonnements
        /// </summary>
        /// <param name="controle">Instance du contrôleur</param>
        internal FrmAlerteFinAbonnements(Controle controle)
        {
            InitializeComponent();
            lesFinAbonnement = controle.GetFinAbonnement();
            bdgAlerteAbonnements.DataSource = lesFinAbonnement;
            dgvAlerteFinAbonnements.DataSource = bdgAlerteAbonnements;
            dgvAlerteFinAbonnements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAlerteFinAbonnements.Columns["idRevue"].DisplayIndex = 2;
            dgvAlerteFinAbonnements.Columns[0].HeaderCell.Value = "Date fin d'abonnement";
            dgvAlerteFinAbonnements.Columns[1].HeaderCell.Value = "Identifiant Revue";
            dgvAlerteFinAbonnements.Columns[2].HeaderCell.Value = "Revue";
            btnAlerteFinAbonnements.Focus();
        }

        /// <summary>
        /// Désactivation de sélection d'une ligne dans le tableau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAlerteFinAbonnements_SelectionChanged(object sender, EventArgs e)
        {
            dgvAlerteFinAbonnements.ClearSelection();
        }

        /// <summary>
        /// Evénement clic sur le bouton 'continuer', ferme la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAlerteFinAbonnements_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
