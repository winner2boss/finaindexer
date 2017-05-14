using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Configuration;
using Controle_Tranche.CWS.DocumentManagement;
using Telerik.WinControls.UI;
using Controle_Tranche.Properties;
using System.Data.SqlClient;

namespace Controle_Tranche
{
    public partial class choix_unite : Telerik.WinControls.UI.RadForm
    {
        public string authToken { get; set; }
        public string user { set; get; }
        public int id_user { set; get; }
        public long id_tranche { get; set; }
            //= long.Parse(ConfigurationManager.AppSettings["id_tranche"]);
        private Service_GED service_ged = new Service_GED();
        public RadTreeNode treeUnite { set; get; }
        public RadTreeNode treeTranche { set; get; }
        Service_GED service_GED = new Service_GED();
        Service_data service_data = new Service_data();
        public bool charger_uniter = false;
        public int phase { get; set; }
        private Node[] nodes;
        Node node;
        DataTable dtUnite;
        SqlDataAdapter da_unite;
        public choix_unite()
        {
            InitializeComponent();
        }

        private void choix_unite_Load(object sender, EventArgs e)
        {
            radGroupBoxphase.Text = "Phase "+phase;
            if (phase.Equals(3))
            {
                try
                {
                    dtUnite = new DataTable();
                    da_unite = new SqlDataAdapter("select distinct TB_U.* from TB_Unite TB_U ,TB_DossierF TB_D where TB_D.status=5 and TB_D.id_unite=TB_U.id_unite and TB_U.id_tranche=" + id_tranche + " order by TB_U.nom_unite", service_data.getConnextion());
                    da_unite.Fill(dtUnite);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (ConfigurationManager.AppSettings["role"].Equals("2"))
            {
                try
                {
                    dtUnite = new DataTable();
                    da_unite = new SqlDataAdapter("select distinct TB_U.* from TB_Unite TB_U ,TB_DossierF TB_D where TB_D.status=4 and TB_D.id_unite=TB_U.id_unite and TB_U.status=1 and TB_U.id_tranche=" + id_tranche + " order by TB_U.nom_unite", service_data.getConnextion());
                    da_unite.Fill(dtUnite);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            load_unite();
            this.tree_unite_phase1.ItemHeight = 25;
            btncharger.Enabled = false;
            tree_unite_phase1.Nodes.Add(this.treeTranche);
            tree_unite_phase1.Nodes[0].Expand();
        }

        public void load_unite()
        {
            node = service_GED.get_Node(this.authToken, id_tranche);
            treeTranche = new RadTreeNode();
            treeTranche.Value = node.ID;
            treeTranche.Name = node.Name;
            treeTranche.Text = node.Name;
            treeTranche.Image = Resources.tranche;
            
            if (ConfigurationManager.AppSettings["role"].Equals("1"))
            {
                if (phase.Equals(1))
                {
                    nodes = service_GED.get_List_Nodes(authToken, id_tranche);
                    var _nodesTri = nodes.OrderBy(x => x.Name);
                    foreach (Node unite in _nodesTri)
                    {
                        if (!unite.Name.Contains("ECH"))
                        {
                            node = service_GED.get_Node(authToken, unite.ID);
                            treeUnite = new RadTreeNode();
                            treeUnite.Value = node.ID;
                            treeUnite.Name = node.Name;
                            treeUnite.Text = node.Name;
                            treeUnite.Image = Resources.unite;
                            treeTranche.Nodes.Add(treeUnite);
                        }
                    }
                }
                else if (phase.Equals(3))
                {
                    if (!dtUnite.Rows.Count.Equals(0))
                    {
                        foreach (DataRow dr in dtUnite.Rows)
                        {
                            node = service_GED.get_Node(authToken, (int)dr[0]);
                            treeUnite = new RadTreeNode();
                            treeUnite.Value = node.ID;
                            treeUnite.Name = node.Name;
                            treeUnite.Text = node.Name;
                            treeUnite.Image = Resources.unite;
                            treeTranche.Nodes.Add(treeUnite);
                        }
                    }
                }
            }
            else if (ConfigurationManager.AppSettings["role"].Equals("2"))
            {
                if (!dtUnite.Rows.Count.Equals(0))
                {
                    foreach (DataRow dr in dtUnite.Rows)
                    {
                        //int id_unite = (int)dr[0];
                        node = service_GED.get_Node(authToken, (int)dr[0]);
                        treeUnite = new RadTreeNode();
                        treeUnite.Value = node.ID;
                        treeUnite.Name = node.Name;
                        treeUnite.Text = node.Name;
                        treeUnite.Image = Resources.unite;
                        treeTranche.Nodes.Add(treeUnite);
                    }
                }
            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            treeUnite = tree_unite_phase1.SelectedNode;
            if (ConfigurationManager.AppSettings["role"].Equals("1"))
            {
                
                if (service_data.getValue("select count(*) from TB_Unite where id_unite=" + treeUnite.Value).Equals(0))
                {
                    service_data.setData("insert into TB_Unite VALUES(" + treeUnite.Value + "," + id_tranche + ",1,'" + treeUnite.Name + "',null,null)");
                }
            }
 
            Traitement_Controle tc = new Traitement_Controle();
            tc.id_unite = (long)treeUnite.Value;
            tc.authToken = authToken;
            tc.user = this.user;
            tc.id_user = this.id_user;
            tc.phase = this.phase;
            tc.id_tranche = this.id_tranche;
            tc.loadTree();
            charger_uniter = true;
            tc.Show();
            this.Close();
            Cursor.Current = Cursors.Default;
        }

        private void unite_phase1_SelectedNodeChanged(object sender, Telerik.WinControls.UI.RadTreeViewEventArgs e)
        {
            RadTreeNode treeUnite;
            treeUnite = tree_unite_phase1.SelectedNode;
            if (!treeUnite.Level.Equals(0))
            {
                btncharger.Enabled = true;

            }else
                btncharger.Enabled = false;
        }

        private void choix_unite_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!charger_uniter)
                Application.Exit();
        }
    }
}
