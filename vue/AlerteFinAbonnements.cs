using Mediatek86.controleur;
using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mediatek86.vue
{
    public partial class AlerteFinAbonnements : Form
    {
        private readonly BindingSource bdgAlerteAbonnements = new BindingSource();
        private List<FinAbonnement> lesFinAbonnement;

        /// <summary>
        /// Constructeur. Remplit le tableau des abonnements
        /// </summary>
        /// <param name="controle"></param>
        internal AlerteFinAbonnements(Controle controle)
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
