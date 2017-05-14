using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Controle_Tranche.CWS.DocumentManagement;
using Telerik.WinControls.UI;
using System.Configuration;
using Controle_Tranche.Properties;
using System.Data.SqlClient;

namespace Controle_Tranche
{
    public partial class Authentification : Telerik.WinControls.UI.RadForm
    {
        Service_GED service_GED = new Service_GED();
        Service_data service_data = new Service_data();
        private string user, pass,authtoken=null;
        private int id_user { set; get; }
        //private long id_tranche = long.Parse(ConfigurationManager.AppSettings["id_tranche"]);
        public Authentification()
        {
            InitializeComponent();
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["role"].Equals("1"))
            {
                if (comboBox1.Text.Equals(""))
                {
                    MessageBox.Show("Merci de sélectionner une phase", "Erreur d'authentification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
                Cursor.Current = Cursors.WaitCursor;
                user = txtuser.Text;
                pass = txtpass.Text;
                authtoken = service_GED.Connexion(user, pass);
                if (authtoken != null)
                {
                    id_user = service_data.getValue("SELECT [ID] FROM [GED].[GED].[KUAF] where Name='" + user + "'");
                    choix_unite ch_u = new choix_unite();
                    ch_u.authToken = authtoken;
                    ch_u.user = user;
                    ch_u.id_user = this.id_user;
                    //ch_u.load_unite();
                    switch (comboBox1.Text)
                    {
                        case "Phase 1":
                            ch_u.phase = 1;
                            break;
                        case "Phase 3":
                            ch_u.phase = 3;
                            break;
                        default:
                            ch_u.phase = 2;
                            break;
                    }
                    ch_u.id_tranche = Convert.ToInt64(cBox_Tranche.SelectedValue);
                    ch_u.Show();
                    this.Close();
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("Le nom d'utilisateur ou le mot de passe spécifié n'est pas valable", "Erreur d'authentification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
       }

        private void Authentifcation_Load(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringGED"].ConnectionString);
            conn.Open();
            SqlCommand sc = new SqlCommand("select tr.id_Tranche, tree.Name from dbo.TB_Tranche tr, GED.DTree tree where tr.id_Tranche=tree.DataID", conn);
            SqlDataReader reader;
            reader = sc.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("id_Tranche", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Load(reader);
            cBox_Tranche.ValueMember = "id_Tranche";
            cBox_Tranche.DisplayMember = "Name";
            cBox_Tranche.DataSource = dt;
            conn.Close();

            if (ConfigurationManager.AppSettings["role"].Equals("2"))
            {
                radLabel3.Visible = false;
                comboBox1.Visible=false;
            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
            
        }
    }

