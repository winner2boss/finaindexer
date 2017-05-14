using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Data.SqlClient;
using System.Configuration;

namespace Controle_Tranche
{
    public partial class recherche : Telerik.WinControls.UI.RadForm
    {
        private string num, indic, indic_spe;
        private Service_data service_data = new Service_data();
        private int id_dos=0;
        public Traitement_Controle control;
        public recherche()
        {
            InitializeComponent();
        }

        private void Annuler_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Rechercher_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            num = numTitre.Text;
            indic = indicTitre.Text;
            if (String.IsNullOrEmpty(indicTitreSpe.Text))
            {
                id_dos = service_data.getValue("select ParentID from V_FICHE_DOSSIER where N_TITRE =" + num + "and INDICE_TITRE ='" + indic + "' and Name like '%PAGE DE GARDE DU DOSSIER%'");
            }
            else
            {
                indic_spe = indicTitreSpe.Text;
                id_dos = service_data.getValue("select ParentID from V_FICHE_DOSSIER where N_TITRE =" + num + "and INDICE_TITRE ='" + indic + "'and INDICE_SPC_TITRE=" + indic_spe + "and Name like '%PAGE DE GARDE DU DOSSIER%'");
            }
            if (id_dos == 0)
            {
                radLabel4.Text = "Le dossier n'existe pas";
            }
            else 
            {
                control.loadTree_recherche(id_dos);
                this.Hide();
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
