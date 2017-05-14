namespace Controle_Tranche
{
    partial class recherche
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.Rechercher = new Telerik.WinControls.UI.RadButton();
            this.Annuler = new Telerik.WinControls.UI.RadButton();
            this.numTitre = new System.Windows.Forms.TextBox();
            this.indicTitre = new System.Windows.Forms.TextBox();
            this.visualStudio2012LightTheme1 = new Telerik.WinControls.Themes.VisualStudio2012LightTheme();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.indicTitreSpe = new System.Windows.Forms.TextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rechercher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Annuler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(23, 38);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(78, 18);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "Numéro Titre :";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(23, 74);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(89, 18);
            this.radLabel2.TabIndex = 1;
            this.radLabel2.Text = "Indice de Titre :  ";
            // 
            // Rechercher
            // 
            this.Rechercher.Location = new System.Drawing.Point(107, 139);
            this.Rechercher.Name = "Rechercher";
            this.Rechercher.Size = new System.Drawing.Size(110, 24);
            this.Rechercher.TabIndex = 2;
            this.Rechercher.Text = "Rechercher";
            this.Rechercher.Click += new System.EventHandler(this.Rechercher_Click);
            // 
            // Annuler
            // 
            this.Annuler.Location = new System.Drawing.Point(257, 139);
            this.Annuler.Name = "Annuler";
            this.Annuler.Size = new System.Drawing.Size(92, 24);
            this.Annuler.TabIndex = 3;
            this.Annuler.Text = "Annuler";
            this.Annuler.ThemeName = "VisualStudio2012Light";
            this.Annuler.Click += new System.EventHandler(this.Annuler_Click);
            // 
            // numTitre
            // 
            this.numTitre.Location = new System.Drawing.Point(161, 36);
            this.numTitre.Name = "numTitre";
            this.numTitre.Size = new System.Drawing.Size(170, 20);
            this.numTitre.TabIndex = 4;
            // 
            // indicTitre
            // 
            this.indicTitre.Location = new System.Drawing.Point(161, 72);
            this.indicTitre.Name = "indicTitre";
            this.indicTitre.Size = new System.Drawing.Size(170, 20);
            this.indicTitre.TabIndex = 5;
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(23, 107);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(132, 18);
            this.radLabel3.TabIndex = 6;
            this.radLabel3.Text = "Indice spéciale de Titre :  ";
            // 
            // indicTitreSpe
            // 
            this.indicTitreSpe.Location = new System.Drawing.Point(161, 105);
            this.indicTitreSpe.Name = "indicTitreSpe";
            this.indicTitreSpe.Size = new System.Drawing.Size(170, 20);
            this.indicTitreSpe.TabIndex = 7;
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.ForeColor = System.Drawing.Color.Red;
            this.radLabel4.Location = new System.Drawing.Point(23, 12);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(2, 2);
            this.radLabel4.TabIndex = 8;
            // 
            // recherche
            // 
            this.AcceptButton = this.Rechercher;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 175);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.indicTitreSpe);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.indicTitre);
            this.Controls.Add(this.numTitre);
            this.Controls.Add(this.Annuler);
            this.Controls.Add(this.Rechercher);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Name = "recherche";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recherche";
            this.ThemeName = "ControlDefault";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rechercher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Annuler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton Rechercher;
        private Telerik.WinControls.UI.RadButton Annuler;
        private System.Windows.Forms.TextBox numTitre;
        private System.Windows.Forms.TextBox indicTitre;
        private Telerik.WinControls.Themes.VisualStudio2012LightTheme visualStudio2012LightTheme1;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.TextBox indicTitreSpe;
        private Telerik.WinControls.UI.RadLabel radLabel4;
    }
}
