
namespace Mediatek86.vue
{
    partial class FrmAuthentification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txbAuthUtilisateur = new System.Windows.Forms.TextBox();
            this.txbAuthMdp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAuthValider = new System.Windows.Forms.Button();
            this.btnAuthAnnuler = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txbAuthUtilisateur
            // 
            this.txbAuthUtilisateur.Location = new System.Drawing.Point(108, 19);
            this.txbAuthUtilisateur.Name = "txbAuthUtilisateur";
            this.txbAuthUtilisateur.Size = new System.Drawing.Size(251, 20);
            this.txbAuthUtilisateur.TabIndex = 0;
            // 
            // txbAuthMdp
            // 
            this.txbAuthMdp.Location = new System.Drawing.Point(108, 45);
            this.txbAuthMdp.Name = "txbAuthMdp";
            this.txbAuthMdp.Size = new System.Drawing.Size(251, 20);
            this.txbAuthMdp.TabIndex = 1;
            this.txbAuthMdp.UseSystemPasswordChar = true;
            this.txbAuthMdp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txbAuthMdp_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nom d\'utilisateur :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mot de passe :";
            // 
            // btnAuthValider
            // 
            this.btnAuthValider.Location = new System.Drawing.Point(108, 71);
            this.btnAuthValider.Name = "btnAuthValider";
            this.btnAuthValider.Size = new System.Drawing.Size(122, 23);
            this.btnAuthValider.TabIndex = 4;
            this.btnAuthValider.Text = "Valider";
            this.btnAuthValider.UseVisualStyleBackColor = true;
            this.btnAuthValider.Click += new System.EventHandler(this.btnAuthValider_Click);
            // 
            // btnAuthAnnuler
            // 
            this.btnAuthAnnuler.Location = new System.Drawing.Point(237, 71);
            this.btnAuthAnnuler.Name = "btnAuthAnnuler";
            this.btnAuthAnnuler.Size = new System.Drawing.Size(122, 23);
            this.btnAuthAnnuler.TabIndex = 5;
            this.btnAuthAnnuler.Text = "Annuler";
            this.btnAuthAnnuler.UseVisualStyleBackColor = true;
            this.btnAuthAnnuler.Click += new System.EventHandler(this.btnAuthAnnuler_Click);
            // 
            // Authentification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 105);
            this.Controls.Add(this.btnAuthAnnuler);
            this.Controls.Add(this.btnAuthValider);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbAuthMdp);
            this.Controls.Add(this.txbAuthUtilisateur);
            this.Name = "Authentification";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Authentification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbAuthUtilisateur;
        private System.Windows.Forms.TextBox txbAuthMdp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAuthValider;
        private System.Windows.Forms.Button btnAuthAnnuler;
    }
}