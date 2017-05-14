namespace Controle_Tranche
{
    partial class choix_unite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(choix_unite));
            this.radGroupBoxphase = new Telerik.WinControls.UI.RadGroupBox();
            this.tree_unite_phase1 = new Telerik.WinControls.UI.RadTreeView();
            this.btncharger = new Telerik.WinControls.UI.RadButton();
            this.visualStudio2012LightTheme1 = new Telerik.WinControls.Themes.VisualStudio2012LightTheme();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBoxphase)).BeginInit();
            this.radGroupBoxphase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tree_unite_phase1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btncharger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGroupBoxphase
            // 
            this.radGroupBoxphase.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBoxphase.Controls.Add(this.tree_unite_phase1);
            this.radGroupBoxphase.HeaderText = "Phase 1 ( Réception )";
            this.radGroupBoxphase.Location = new System.Drawing.Point(12, 12);
            this.radGroupBoxphase.Name = "radGroupBoxphase";
            // 
            // 
            // 
            this.radGroupBoxphase.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBoxphase.Size = new System.Drawing.Size(382, 326);
            this.radGroupBoxphase.TabIndex = 0;
            this.radGroupBoxphase.Text = "Phase 1 ( Réception )";
            this.radGroupBoxphase.ThemeName = "VisualStudio2012Light";
            // 
            // tree_unite_phase1
            // 
            this.tree_unite_phase1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.tree_unite_phase1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tree_unite_phase1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree_unite_phase1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.tree_unite_phase1.ForeColor = System.Drawing.Color.Black;
            this.tree_unite_phase1.Location = new System.Drawing.Point(2, 18);
            this.tree_unite_phase1.Name = "tree_unite_phase1";
            this.tree_unite_phase1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tree_unite_phase1.Size = new System.Drawing.Size(378, 306);
            this.tree_unite_phase1.SpacingBetweenNodes = -1;
            this.tree_unite_phase1.TabIndex = 0;
            this.tree_unite_phase1.Text = "radTreeView1";
            this.tree_unite_phase1.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(this.unite_phase1_SelectedNodeChanged);
            // 
            // btncharger
            // 
            this.btncharger.Location = new System.Drawing.Point(284, 344);
            this.btncharger.Name = "btncharger";
            this.btncharger.Size = new System.Drawing.Size(110, 24);
            this.btncharger.TabIndex = 1;
            this.btncharger.Text = "&Charger l\'unité";
            this.btncharger.ThemeName = "VisualStudio2012Light";
            this.btncharger.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // choix_unite
            // 
            this.AcceptButton = this.btncharger;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 387);
            this.Controls.Add(this.btncharger);
            this.Controls.Add(this.radGroupBoxphase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "choix_unite";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choisir l\'unité ";
            this.ThemeName = "VisualStudio2012Light";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.choix_unite_FormClosing);
            this.Load += new System.EventHandler(this.choix_unite_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBoxphase)).EndInit();
            this.radGroupBoxphase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tree_unite_phase1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btncharger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGroupBox radGroupBoxphase;
        private Telerik.WinControls.UI.RadTreeView tree_unite_phase1;
        private Telerik.WinControls.UI.RadButton btncharger;
        private Telerik.WinControls.Themes.VisualStudio2012LightTheme visualStudio2012LightTheme1;
    }
}
