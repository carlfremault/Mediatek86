
namespace Mediatek86.vue
{
    partial class FrmAlerteFinAbonnements
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvAlerteFinAbonnements = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAlerteFinAbonnements = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerteFinAbonnements)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAlerteFinAbonnements
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAlerteFinAbonnements.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAlerteFinAbonnements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAlerteFinAbonnements.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAlerteFinAbonnements.Location = new System.Drawing.Point(12, 71);
            this.dgvAlerteFinAbonnements.Name = "dgvAlerteFinAbonnements";
            this.dgvAlerteFinAbonnements.RowHeadersVisible = false;
            this.dgvAlerteFinAbonnements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlerteFinAbonnements.Size = new System.Drawing.Size(418, 150);
            this.dgvAlerteFinAbonnements.TabIndex = 0;
            this.dgvAlerteFinAbonnements.TabStop = false;
            this.dgvAlerteFinAbonnements.SelectionChanged += new System.EventHandler(this.dgvAlerteFinAbonnements_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Attention ! Ces abonnements expirent dans moins de 30 jours :";
            // 
            // btnAlerteFinAbonnements
            // 
            this.btnAlerteFinAbonnements.Location = new System.Drawing.Point(12, 227);
            this.btnAlerteFinAbonnements.Name = "btnAlerteFinAbonnements";
            this.btnAlerteFinAbonnements.Size = new System.Drawing.Size(418, 29);
            this.btnAlerteFinAbonnements.TabIndex = 1;
            this.btnAlerteFinAbonnements.Text = "Continuer";
            this.btnAlerteFinAbonnements.UseVisualStyleBackColor = true;
            this.btnAlerteFinAbonnements.Click += new System.EventHandler(this.btnAlerteFinAbonnements_Click);
            // 
            // FrmAlerteFinAbonnements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 270);
            this.Controls.Add(this.btnAlerteFinAbonnements);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvAlerteFinAbonnements);
            this.Name = "FrmAlerteFinAbonnements";
            this.Text = "Alerte abonnements";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerteFinAbonnements)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAlerteFinAbonnements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAlerteFinAbonnements;
    }
}