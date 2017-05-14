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
using Controle_Tranche.Properties;
using Telerik.WinControls.UI;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;

namespace Controle_Tranche
{
    public partial class Traitement_Controle : Telerik.WinControls.UI.RadForm
    {
        bool deconnexion = false;
        public string authToken { get; set; }
        public string user { get; set; }
        public int id_user { set; get; }
        public long id_tranche { set; get; }
            //= long.Parse(ConfigurationManager.AppSettings["id_tranche"]);
        public long id_unite { set; get; }
        private Stream image_stream;
        private Node[] nodes=null;
        private Node node=null,vues=null;
        bool supprimer = false;
        /*Déclaration des service*/
        private Service_GED service_ged = new Service_GED();
        private Service_data service_data = new Service_data();
        /*Déclaration treeNode*/
        public RadTreeNode treeUnite{set;get;}
        public RadTreeNode treeTranche { set; get; }
        public RadTreeNode treeDossier { set; get; }
        public RadTreeNode treeSousDossier { set; get; }
        public RadTreeNode treePiece { set; get; }
        List<RadTreeNode> treePieces { set; get; }
        List<RadTreeNode> treeSousDossiers;
        /*Déclaration de thread*/
        BackgroundWorker _bWChargerdossier, _bWchargerImageVues, _bWchargerIndexVues, _bWfermerDossier;
        /*Déclaration arboresence*/
        long idDossier=0, idSDDossier=0, idDocument=0;
        public int phase { get; set; }
        /*Image*/
        Bitmap image = null;
        double zoom = 1;
        int defImgWidth;
        int defImgHeight;
        string type_image;
        long? taille_image;
        /*les indexs de document*/
        Elements element;
        DataRow[] dr_vues_index;
        DataRow[] dr_vues_index_memo;
        /*Gestion de la base de donnée*/
        DataTable dtformalite,dtpiece,dt_vues,dt_dossier,dt_unite,dt_dossier_historique,dt_vues_historique;
        SqlDataAdapter da_vues, da_dossier, da_sd_manquent, da_piece_manquent, da_unite, da_dossier_historique,da_vues_historique;
        SqlCommandBuilder cmdBuilder_vues, cmdBuilder_dossier, cmdBuilder_unite, cmdBuilder_sd_manquent, cmdBuilder_piece_manquent, cmdBuilder_dossier_historique,cmdBuilder_historique_vues;
        /*Déclaration traitement de validation*/
        int nombre_document_charge = 0, nombre_document_valider = 0, nombre_document_rejet = 0, nombre_document_corriger = 0;
        int nbrVue_trait = 0;
        string type_dossier="";
        /*Déclaration Unité*/
        int nbr_dossier=0,nbr_dossier_valider=0,nbr_dossier_rejeter=0;
        bool cloture_uniter=false,isPhase=true;
        int ph = 0;
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringGED"].ConnectionString);
       //05-04-2017
        Form1 form1 = new Form1();
        String intitule_Sous_Dossier = "";
        String intitule_Dossier = "";
        String intitule_Piece;

        public Traitement_Controle()
        {
            InitializeComponent();
        }

        private void Traitement_Controle_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!cloture_uniter && !deconnexion)
            {
                if (phase.Equals(1))
                {
                    if (nombre_document_charge > 0)
                    {
                        int nombre_document_traiter = nombre_document_rejet + nombre_document_valider;
                        if (nombre_document_traiter == nombre_document_charge)
                        {
                            int porcentage = nombre_document_rejet * 100 / nombre_document_charge;
                            DataRow[] dr_dossier = dt_dossier.Select("id_DF=" + idDossier);
                            if (dr_dossier.Count().Equals(0))
                            {
                                if (porcentage>double.Parse(ConfigurationManager.AppSettings["pourcentage_validation"].ToString()))
                                {
                                    dr_dossier[0][2] = 4;
                                }
                                else
                                {
                                    dr_dossier[0][2] = 3;
                                }
                            }
                        }
                    }
                }
                /*Mise a jour*/
                if (dt_dossier != null)
                    da_dossier.Update(dt_dossier);
                if (dt_vues != null)
                    da_vues.Update(dt_vues);
                /*da_sd_manquent.Update(dtformalite);
                da_piece_manquent.Update(dtpiece);*/
                Application.Exit();
            }
        }

        private void Traitement_Controle_Load(object sender, EventArgs e)
        {
            // TODO: cette ligne de code charge les données dans la table 'gEDDataSet1.PIECE'. Vous pouvez la déplacer ou la supprimer selon vos besoins.
            this.pIECETableAdapter.Fill(this.gEDDataSet1.PIECE);
            // TODO: cette ligne de code charge les données dans la table 'gEDDataSet.LISTE_FORMALITES'. Vous pouvez la déplacer ou la supprimer selon vos besoins.
            this.lISTE_FORMALITESTableAdapter.Fill(this.gEDDataSet.LISTE_FORMALITES);

            // Action de Chargement de dossier 
            this.radMenuItem6.Click += new EventHandler(radMenuItem6_Click);
            this.radMenuItem7.Click += new EventHandler(radMenuItem7_Click);
            this.radMenuItem8.Click += new EventHandler(radMenuItem8_Click);
            this.tree_unite_dossier.ItemHeight = 25;
            activeCheckbox(ConfigurationManager.AppSettings["role"], true);
            lblnbr_valider.Visible = true;
            lblnbr_vue.Visible = true;
            lblvue_rejeter.Visible = true;


           
            if (phase.Equals(1))
            {
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
                
            }
            else if (phase.Equals(2))
            {
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues corrigées : {0}", nombre_document_corriger);
                //frocer_correct
                cb_forc.Visible = true;
                //
                //supprimer
                cb_suppri_cr.Visible = true;
                //
                
            }
            else if (phase.Equals(3))
            {
                ck_valider_vue.Visible = true;
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
           
        }

        //Chargement de l'arborescence
        public void loadTree()
        {
            da_unite = new SqlDataAdapter("select * from TB_Unite where id_unite=" + id_unite, service_data.getConnextion());
            da_dossier = new SqlDataAdapter("select * from TB_DossierF where id_unite="+id_unite, service_data.getConnextion());
            da_dossier_historique = new SqlDataAdapter("select * from TB_Historique_Dos where id_unite=" + id_unite, service_data.getConnextion());
            cmdBuilder_unite = new SqlCommandBuilder(da_unite);
            cmdBuilder_dossier = new SqlCommandBuilder(da_dossier);
            cmdBuilder_dossier_historique = new SqlCommandBuilder(da_dossier_historique);
            dt_dossier = new DataTable();
            dt_unite = new DataTable();
            dt_dossier_historique = new DataTable();
            try
            {
                da_unite.Fill(dt_unite);
                da_dossier.Fill(dt_dossier);
                da_dossier_historique.Fill(dt_dossier_historique);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            int i = 0;
            //nodes = service_ged.get_List_Nodes(this.authToken, id_tranche);

            /*Tranche*/
            node = service_ged.get_Node(authToken, id_tranche);
            treeTranche = new RadTreeNode();
            treeTranche.Value = node.ID;
            treeTranche.Name = node.Name;
            treeTranche.Text = node.Name;
            treeTranche.Image = Resources.tranche;
            this.tree_unite_dossier.Nodes.Add(treeTranche);

            /*Unité*/
            node = service_ged.get_Node(authToken, id_unite);
            treeUnite = new RadTreeNode();
            treeUnite.Value = node.ID;
            treeUnite.Name = node.Name;
            
            /*Dossier*/
            nodes = service_ged.get_List_Nodes(this.authToken, id_unite);
            treeUnite.Text = String.Format("{0}", node.Name);
            treeUnite.Image = Resources.unite;
            treeUnite.ContextMenu = contextMenuUnite;
            this.tree_unite_dossier.Nodes[0].Nodes.Add(treeUnite);
            this.tree_unite_dossier.Nodes[0].Expand();

            int nbrDossierNonCtrlPh1 = 0;
            int nbrDossierCtrlPh1 = 0;
            if (nodes != null)
            {
                var _nodesTri = nodes.OrderBy(x => x.Name);
                var folders = _nodesTri.Where(item => item.Type.Equals("Folder"));
                foreach (Node folder in folders)
                {
                    node = service_ged.get_Node(authToken, folder.ID);
                    DataRow[] drdossier = dt_dossier.Select("id_DF=" + node.ID);
                    treeDossier = new RadTreeNode();
                    treeDossier.Value = node.ID;
                    treeDossier.Name = node.Name;
                    treeDossier.Text = String.Format("{0}", node.Name);
                    if (drdossier.Count().Equals(0))
                    {
                        if (phase.Equals(1))
                        {
                            nbrDossierNonCtrlPh1++;
                            if (nbrDossierCtrlPh1 > 0 && nbrDossierNonCtrlPh1 >= double.Parse(ConfigurationManager.AppSettings["nbrDossierPh1"].ToString()))
                            {
                                break;
                            }
                            treeDossier.Image = Resources.folder;
                            this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes.Add(treeDossier);
                            this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[i].ContextMenu = menuAction;
                            i++;

                        }
                    }
                    else
                    {
                        if (drdossier[0][2].Equals(3))
                        {
                            treeDossier.Image = Resources.folder_ok;
                        }
                        else if (drdossier[0][2].Equals(4))
                        {
                            treeDossier.Image = Resources.folder_error;
                        }
                        else if (drdossier[0][2].Equals(5))
                        {
                            treeDossier.Image = Resources.folder_corriger;
                        }
                        else if (drdossier[0][2].Equals(2) || drdossier[0][2].Equals(1))
                        {
                            if (drdossier[0][7].Equals(id_user))
                            {
                                treeDossier.Text = String.Format("{0} ({1})", node.Name, drdossier[0][8].ToString());
                                treeDossier.Font = new Font(this.Font, FontStyle.Bold);
                                treeDossier.Image = Resources.folder_en_attendant;
                            }
                            else
                            {
                                treeDossier.Text = String.Format("{0} ({1})", node.Name, drdossier[0][8].ToString());
                                treeDossier.Font = new Font(this.Font, FontStyle.Bold);
                                treeDossier.Image = Resources.folder_lock;
                            }
                        }
                        else
                        {
                            treeDossier.Image = Resources.folder;
                        }
                       /* if (ConfigurationManager.AppSettings["role"].Equals("1"))
                        {*/
                        if (phase.Equals(1))
                        {
                            nbrDossierCtrlPh1++;
                            if (drdossier[0][2].Equals(3))
                            {
                                nbr_dossier_valider++;
                            }
                            else if (drdossier[0][2].Equals(4))
                            {
                                nbr_dossier_rejeter++;
                            }
                            if (!drdossier[0][2].Equals(4) && drdossier[0][6].Equals(1))
                            {
                                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes.Add(treeDossier);
                                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[i].ContextMenu = menuAction;
                                i++;
                            }     
                        }
                        else if (phase.Equals(3))
                        {
                            if (drdossier[0][2].Equals(5) && drdossier[0][6].Equals(3))
                            {
                                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes.Add(treeDossier);
                                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[i].ContextMenu = menuAction;
                                i++;
                            }
                        }
                        else if (phase.Equals(2))
                        {
                                if (drdossier[0][2].Equals(4) && drdossier[0][6].Equals(2))
                                {
                                    nbr_dossier_rejeter++;
                                    this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes.Add(treeDossier);
                                    this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[i].ContextMenu = menuAction;
                                    i++;
                                }
                        }   
                        }
                      
                    }  
                    nbr_dossier++;           
                }
                this.tree_unite_dossier.Nodes[0].Nodes[0].Expand();
        }

        //charger dossier
        private void radMenuItem6_Click(object sender, EventArgs e) 
        { 
            if (this.tree_unite_dossier.SelectedNode != null) 
            {
                _bWChargerdossier = getBWchargeDossier();
                isPhase = true;
                radGroupBoxPiece_rjt.Enabled = true;
                try
                {
                    if (!_bWChargerdossier.IsBusy)
                    {
                        WaitingBarTraitement.Visible = true;
                        WaitingBarTraitement.StartWaiting();
                        tree_unite_dossier.SelectedNode.Expanded = false;
                        tree_unite_dossier.Enabled = false;
                        treeDossier = tree_unite_dossier.SelectedNode;
                        idDossier = (long)treeDossier.Value;
                        _bWChargerdossier.RunWorkerAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                 
            } 
        }

        //fermer dossier
        private void radMenuItem7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!tree_unite_dossier.SelectedNode.Nodes.Count.Equals(0))
                {
                    //maria
                    SqlCommand cmd = new SqlCommand("select GETDATE()", con);
                    con.Open();
                    string date = cmd.ExecuteScalar().ToString();
                    con.Close();
                    
                    WaitingBarTraitement.StartWaiting();
                    WaitingBarTraitement.Visible = true;
                   da_vues.Update(dt_vues);

                    /*librer la mémoire*/
                    if (da_vues != null)
                        dt_vues.Dispose();
                    if (dtformalite != null)
                        dtformalite.Dispose();
                    if (dtpiece != null)
                        dtpiece.Dispose();
                    
                    /*traitement de validation de dossier*/
                    DataRow[] dr_dossier = dt_dossier.Select("id_DF=" + idDossier);
                    if (phase.Equals(1))
                    { 
                        int nombre_document_traiter = nombre_document_rejet + nombre_document_valider;
                        if (nombre_document_traiter == nombre_document_charge)
                        {
                            double porcentage = nombre_document_rejet * 100 / nombre_document_charge;
                            if (!dr_dossier.Count().Equals(0))
                            {
                                if (nombre_document_rejet > 0)
                                {
                                    dr_dossier[0][2] = 4;
                                    dr_dossier[0][6] = 2;
                                    tree_unite_dossier.SelectedNode.Image = Resources.folder_error;
                                    tree_unite_dossier.SelectedNode.Remove();
                                    historiserDossier(1, id_user, 4);
                                }
                                else
                                {
                                    dr_dossier[0][2] = 3;
                                    dr_dossier[0][6] = 4;
                                    tree_unite_dossier.SelectedNode.Image = Resources.folder_ok;
                                    tree_unite_dossier.SelectedNode.Remove();
                                    historiserDossier(1, id_user, 3);
                                }
                               
                            }
                        }
                        else
                        {
                            dr_dossier[0][2] = 2;
                            DialogResult result = MessageBox.Show(" Vous n'avez pas vérifié toutes les vues. Voulez-vous fermer le dossier? ", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (result == DialogResult.No)
                            {
                                return;
                            }

                            tree_unite_dossier.SelectedNode.Nodes.Clear();
                            tree_unite_dossier.SelectedNode.Image = Resources.folder_en_attendant;
                        }
                    }
                    else if (phase.Equals(2))
                    {
                        
                        if (nombre_document_corriger == nombre_document_charge)
                        {
                            if (supprimer == true && nombre_document_corriger == 1)
                            {
                                dr_dossier[0][6] = 4;
                                dr_dossier[0][2] = 3;
                                historiserDossier(2, id_user, 5);
                            }
                            else 
                            {
                                dr_dossier[0][6] = 3;
                                dr_dossier[0][2] = 5;
                                historiserDossier(2, id_user, 5);
                            }
                            tree_unite_dossier.SelectedNode.Image = Resources.folder_corriger;
                            tree_unite_dossier.SelectedNode.Remove();
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show(" Vous n'avez pas vérifié toutes les vues. Voulez-vous fermer le dossier? ", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (result == DialogResult.No)
                            {
                                return;
                            }
                            tree_unite_dossier.SelectedNode.Nodes.Clear();
                            tree_unite_dossier.SelectedNode.Image = Resources.folder_en_attendant;
                        }
                    }
                    else if (phase.Equals(3))
                    {
                        int nombre_document_traiter = nombre_document_rejet + nombre_document_valider;
                        if (nombre_document_traiter == nombre_document_charge)
                        {
                            double porcentage = nombre_document_rejet * 100 / nombre_document_charge;
                            if (!dr_dossier.Count().Equals(0))
                            {
                                if (porcentage > double.Parse(ConfigurationManager.AppSettings["pourcentage_validation"].ToString()))
                                {
                                    dr_dossier[0][6] = 2;
                                    dr_dossier[0][2] = 4;
                                    tree_unite_dossier.SelectedNode.Image = Resources.folder_error;
                                    tree_unite_dossier.SelectedNode.Remove();
                                    historiserDossier(3, id_user, 4);
                                }
                                else
                                {
                                    dr_dossier[0][6] = 4;
                                    dr_dossier[0][2] = 3;
                                    tree_unite_dossier.SelectedNode.Image = Resources.folder_ok;
                                    tree_unite_dossier.SelectedNode.Remove();
                                    historiserDossier(3, id_user, 3);
                                }
                            }
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show(" Vous n'avez pas vérifié toutes les vues. Voulez-vous fermer le dossier? ", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (result == DialogResult.No)
                            {
                                return;
                            }
                            tree_unite_dossier.SelectedNode.Nodes.Clear();
                            tree_unite_dossier.SelectedNode.Image = Resources.folder_en_attendant;
                        }
                    }

                    //maria 01 04 2016
                    if (phase.Equals(2))
                    {
                        dr_dossier[0][4] = DateTime.Now;//Convert.ToDateTime(date);
                    }
                    else if (phase.Equals(3))
                    {
                        dr_dossier[0][5] = DateTime.Now;//Convert.ToDateTime(date);
                    }
                    //Mise a jour de la table dossier
                    da_dossier.Update(dt_dossier);
                    
                    nombre_document_charge = 0;
                    nombre_document_valider = 0;
                    nombre_document_rejet = 0;

                   
                    menuAction.Items[0].Enabled = true;
                    menuAction.Items[1].Enabled = false;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                WaitingBarTraitement.StopWaiting();
                WaitingBarTraitement.Visible = false;
            }
        }

        // changer l'unité
        private void radMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                cloture_uniter = true;
                WaitingBarTraitement.StartWaiting();
                WaitingBarTraitement.Visible = true;
               
                    tree_unite_dossier.SelectedNode.Nodes.Clear();
                    choix_unite ch_u = new choix_unite();
                    ch_u.authToken = authToken;
                    ch_u.user = user;
                    ch_u.id_user = id_user;
                    ch_u.phase = phase;
                    ch_u.id_tranche = this.id_tranche;
                    ch_u.charger_uniter = false;
                    ch_u.Show();
                    this.Close();
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                WaitingBarTraitement.StopWaiting();
                WaitingBarTraitement.Visible = false;
            }
        }

        public BackgroundWorker getBWchargeDossier()
        {
            BackgroundWorker bWChDossier = new BackgroundWorker();
            bWChDossier.WorkerReportsProgress = true;
            bWChDossier.ProgressChanged += new ProgressChangedEventHandler(bWChargerdossier_ProgressChanged);
            bWChDossier.DoWork += new DoWorkEventHandler(bWChargerdossier_DoWork);
            bWChDossier.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWChargerdossier_RunWorkerCompleted);
            return bWChDossier;
        }

        public BackgroundWorker getBWFermerDossier()
        {
            BackgroundWorker bWChDossier = new BackgroundWorker();
            bWChDossier.WorkerReportsProgress = true;
            bWChDossier.DoWork += new DoWorkEventHandler(bWFermerdossier_DoWork);
            bWChDossier.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWFermerdossier_RunWorkerCompleted);
            return bWChDossier;
        }

        public BackgroundWorker getBWchargeImageVues()
        {
            BackgroundWorker bWChDossier = new BackgroundWorker();
            bWChDossier.WorkerReportsProgress = true;
            bWChDossier.DoWork += new DoWorkEventHandler(bWImage_DoWork);
            bWChDossier.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWImage_RunWorkerCompleted);
            return bWChDossier;
        }
        
        public BackgroundWorker getBWchargeIndexVues()
        {
            BackgroundWorker bWChDossier = new BackgroundWorker();
            bWChDossier.WorkerReportsProgress = true;
            bWChDossier.DoWork += new DoWorkEventHandler(bWIndex_DoWork);
            bWChDossier.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWIndex_RunWorkerCompleted);
            return bWChDossier;
        }

        //Selectionner un element de l'arborescence
        private void tree_unite_dossier_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (tree_unite_dossier.SelectedNode != null)
                {
                    cb_suppri_cr.Checked = false;
                    if (tree_unite_dossier.SelectedNode.Level.Equals(3))
                    {
                        _bWchargerImageVues = getBWchargeImageVues();
                        _bWchargerIndexVues = getBWchargeIndexVues();
                        
                        if ((node = service_ged.get_Node(authToken, Convert.ToInt64(tree_unite_dossier.SelectedNode.Value))).Type == "Document")
                        {
                            WaitingBarTraitement.StartWaiting();
                            WaitingBarTraitement.Visible = true;
                            idDocument = (long)tree_unite_dossier.SelectedNode.Value;
                            //non_heritage
                            intialiserControle(radGroupBoxDossier, ConfigurationManager.AppSettings["role"]);
                            intialiserControle(radGroupBoxSousDossier, ConfigurationManager.AppSettings["role"]);
                            //
                            intialiserControle(radGroupBoxPiece_rjt, ConfigurationManager.AppSettings["role"]);
                            if (tree_unite_dossier.SelectedNode.Name.Contains("FICHE DE CONTROLE"))
                            {
                                type_dossier = "DF";
                            }
                            else if (tree_unite_dossier.SelectedNode.Name.Contains("PAGE DE GARDE DU DOSSIER"))
                            {
                                intialiserControle(radGroupBoxDossier, ConfigurationManager.AppSettings["role"]);
                                type_dossier = "";
                            }
                            _bWchargerIndexVues.RunWorkerAsync();
                            _bWchargerImageVues.RunWorkerAsync();

                            if (ConfigurationManager.AppSettings["role"].Equals("1"))
                            {
                                if (isPhase)
                                {
                                    radGroupBoxDossier.Enabled = true;
                                    radGroupBoxSousDossier.Enabled = false;
                                }
                                else 
                                {
                                    radGroupBoxDossier.Enabled = false;
                                    radGroupBoxSousDossier.Enabled = false;
                                    radGroupBoxPiece_rjt.Enabled = false;
                                }
                            }
                            if(!isPhase)
                            {
                                radGroupBoxDossier.Enabled = false;
                                radGroupBoxSousDossier.Enabled = false;
                                radGroupBoxPiece_rjt.Enabled = false;
                            }
                        }
                        else
                        {
                            
                            toolImage.Enabled = false;
                            pb_vues.Image = null;
                            btnPageSuivant.Enabled = false;
                            //non_heritage
                            intialiserControle(radGroupBoxDossier, ConfigurationManager.AppSettings["role"]);
                            //

                            intialiserControle(radGroupBoxSousDossier, ConfigurationManager.AppSettings["role"]);
                            intialiserControle(radGroupBoxPiece_rjt, ConfigurationManager.AppSettings["role"]);
                            videchamp();
                            activeCheckbox(ConfigurationManager.AppSettings["role"], true);
                        }
                    }
                    else if (tree_unite_dossier.SelectedNode.Level.Equals(4))
                    {
                        WaitingBarTraitement.StartWaiting();
                        WaitingBarTraitement.Visible = true;
                        _bWchargerImageVues = getBWchargeImageVues();
                        _bWchargerIndexVues = getBWchargeIndexVues();
                        idDocument = (long)tree_unite_dossier.SelectedNode.Value;
                        idSDDossier = (long)tree_unite_dossier.SelectedNode.Parent.Value;
                        //non_heritage
                        intialiserControle(radGroupBoxDossier, ConfigurationManager.AppSettings["role"]);
                        intialiserControle(radGroupBoxSousDossier, ConfigurationManager.AppSettings["role"]);
                        //
                        intialiserControle(radGroupBoxPiece_rjt, ConfigurationManager.AppSettings["role"]);
                        if (tree_unite_dossier.SelectedNode.Name.Contains("PAGE DE GARDE DU SOUS DOSSIER"))
                        {
                           
                            if (ConfigurationManager.AppSettings["role"].Equals("1"))
                           {
                                if (isPhase)
                                {
                                    radGroupBoxDossier.Enabled = false;
                                    radGroupBoxSousDossier.Enabled = true;
                                }
                                else
                                {
                                    radGroupBoxDossier.Enabled = false;
                                    radGroupBoxSousDossier.Enabled = false;
                                    radGroupBoxPiece_rjt.Enabled = false;
                                }
                                
                            }
                            intialiserControle(radGroupBoxSousDossier, ConfigurationManager.AppSettings["role"]);
                            type_dossier = "SDF";
                        }
                        else
                        {
                            
                            if (ConfigurationManager.AppSettings["role"].Equals("1"))
                            {
                                if (isPhase)
                                {
                                    radGroupBoxDossier.Enabled = false;
                                    radGroupBoxSousDossier.Enabled = false;
                                }
                                else
                                {
                                    radGroupBoxDossier.Enabled = false;
                                    radGroupBoxSousDossier.Enabled = false;
                                    radGroupBoxPiece_rjt.Enabled = false;
                                }
                            }
                            else if (!isPhase)
                            {
                                radGroupBoxDossier.Enabled = false;
                                radGroupBoxSousDossier.Enabled = false;
                                radGroupBoxPiece_rjt.Enabled = false;
                            }
                            type_dossier = "";
                        }
                        _bWchargerIndexVues.RunWorkerAsync();
                        _bWchargerImageVues.RunWorkerAsync();
                    }
                    else
                    {
                        intialiserControle(radGroupBoxDossier, ConfigurationManager.AppSettings["role"]);
                        intialiserControle(radGroupBoxSousDossier, ConfigurationManager.AppSettings["role"]);
                        intialiserControle(radGroupBoxPiece_rjt, ConfigurationManager.AppSettings["role"]);
                        btnPageSuivant.Enabled = false;
                        toolImage.Enabled = false;
                        pb_vues.Image = null;
                        activeCheckbox(ConfigurationManager.AppSettings["role"], true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Bitmap ZoomImage(int ZoomValue, Bitmap pb_image)
        {
            if (pb_image != null)
            {
                Bitmap zoomImage = new Bitmap(pb_image, pb_image.Width * (ZoomValue) / 100, pb_image.Width * (ZoomValue) / 100);
                Graphics converted = Graphics.FromImage(zoomImage);
                converted.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                return zoomImage;
            }
            return null;
        }

        private void zoomToolStripButton_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                this.pb_vues.SizeMode = PictureBoxSizeMode.Zoom;
                Bitmap bmp = new Bitmap(image, Convert.ToInt32(this.pb_vues.Width), Convert.ToInt32(this.pb_vues.Width * defImgHeight / defImgWidth));
                this.pb_vues.Image = bmp;
            }
        }

        //Zoom
        private void zoominToolStripButton_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                this.pb_vues.SizeMode = PictureBoxSizeMode.Normal;
                zoom = zoom + 0.2;
                Bitmap bmp = new Bitmap(image, Convert.ToInt32(this.pb_vues.Width * zoom), Convert.ToInt32(this.pb_vues.Width * zoom * defImgHeight / defImgWidth));
                this.pb_vues.SizeMode = PictureBoxSizeMode.AutoSize;
                this.pb_vues.Image = bmp;
            }
        }

        //Zoom Out
        private void zoomoutToolStripButton_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                this.pb_vues.SizeMode = PictureBoxSizeMode.Normal;
                if (zoom > 0.3) //jangan sampai zoom - 0.2 hasilnya nol
                {
                    zoom = zoom - 0.2;
                    Bitmap bmp = new Bitmap(image, Convert.ToInt32(this.pb_vues.Width * zoom), Convert.ToInt32(this.pb_vues.Width * zoom * defImgHeight / defImgWidth));
                    this.pb_vues.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.pb_vues.Image = bmp;
                }
            }
        }

        //Pivotage à gauche
        private void leftToolStripButton_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                this.pb_vues.SizeMode = PictureBoxSizeMode.Zoom;
                this.pb_vues.Image = image;
                this.pb_vues.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
        }

        //Pivotage à droite
        private void rhgitStripButton1_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                this.pb_vues.SizeMode = PictureBoxSizeMode.Zoom;
                this.pb_vues.Image = image;
                this.pb_vues.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        //BW_Charger dossier
        private void bWChargerdossier_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                treePieces = new List<RadTreeNode>();
                treeSousDossiers = new List<RadTreeNode>();
                DataRow[] drvues;
                RadTreeNode treeDossier = new RadTreeNode();
                //maria
                SqlCommand cmd = new SqlCommand("select GETDATE()", con);
                con.Open();
                string date = cmd.ExecuteScalar().ToString();
                con.Close();

                // Partie Data
                DataRow[] dr_df = dt_dossier.Select("id_DF =" + idDossier);
                if (dr_df.Count().Equals(0))
                {
                    DataRow dr_dossier = dt_dossier.NewRow();
                    dr_dossier[0] = idDossier;
                    dr_dossier[1] = this.tree_unite_dossier.SelectedNode.Parent.Value;
                    //dr_dossier[1] = id_unite;
                    dr_dossier[2] = 1;
                    dr_dossier[3] = DateTime.Now;
                    //dr_dossier[3] = Convert.ToDateTime(date);
                    dr_dossier[6] = phase;
                    dr_dossier[7] = id_user;
                    dr_dossier[8] = user;
                    dt_dossier.Rows.Add(dr_dossier);
                    da_dossier.Update(dt_dossier);
                }
                else
                {
                    if (phase.Equals(1))
                    {
                        if (isReserver(id_user, idDossier))
                        {
                            treePieces = null;
                            treeSousDossiers = null;
                            treeDossier = null;
                            MessageBox.Show("Le dossier est déjà réservé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                //************************************

                dt_vues = new DataTable();
                da_vues = new SqlDataAdapter("select * from TB_Vues where id_DF=" + idDossier, service_data.getConnextion());
                cmdBuilder_vues = new SqlCommandBuilder(da_vues);
                nombre_document_charge = 0;
                nombre_document_corriger = 0;
                nombre_document_rejet = 0;
                nombre_document_valider = 0;

                /*Partie API CLM*/
                int i = 0;
                nodes = service_ged.get_List_Nodes(this.authToken, idDossier);
                da_vues.Fill(dt_vues);
                if (nodes != null)
                {
                    var documents = nodes.Where(item => item.Type.Equals("Document"));
                    var _DF_PG = documents.Where(x => x.Name.Contains(ConfigurationManager.AppSettings["PGD"]));
                    var _docTri = documents.OrderBy(x => x.Name);
                    var folders = nodes.Where(item => item.Type.Equals("Folder"));
                    var _nodesTri = folders.OrderBy(x => x.Name);
                    if (_DF_PG != null)
                    {
                        if (_DF_PG.Count() > 0)
                        {
                            /*Page de gard du dossier*/
                            foreach (Node document in _DF_PG)
                            {
                                treePiece = new RadTreeNode();
                                treePiece.Name = document.Name;
                                treePiece.Text = document.Name;
                                treePiece.Value = document.ID;
                                drvues = dt_vues.Select("id_vues=" + document.ID);
                                if (drvues.Count().Equals(0))
                                {
                                    if (phase.Equals(1))
                                    {
                                        nombre_document_charge++;
                                        treePiece.Image = Resources.document;
                                        treePieces.Add(treePiece);
                                    }
                                }
                                else
                                {
                                    if (phase.Equals(1))
                                    {
                                        if (drvues[0][3].Equals(3))
                                        {
                                            nombre_document_valider++;
                                            treePiece.Image = Resources.document_valider;
                                        }
                                        else if (drvues[0][3].Equals(4))
                                        {
                                            nombre_document_rejet++;
                                            treePiece.Image = Resources.document_rejeter;
                                        }
                                        else
                                        {
                                            treePiece.Image = Resources.document;
                                        }
                                        nombre_document_charge++;
                                        treePieces.Add(treePiece);
                                    }
                                    else if (phase.Equals(2))
                                    {
                                        if (drvues[0][3].Equals(5))
                                        {
                                            nombre_document_corriger++;
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document_corriger;
                                        }
                                        if (drvues[0][3].Equals(4))
                                        {
                                            nombre_document_rejet++;
                                            nombre_document_charge++;
                                            treePieces.Add(treePiece);
                                        }
                                    }
                                    else if (phase.Equals(3))
                                    {
                                        if (drvues[0][3].Equals(5))
                                        {
                                            nombre_document_corriger++;
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document_corriger;
                                            treePieces.Add(treePiece);
                                        }
                                        else if (drvues[0][3].Equals(4))
                                        {
                                            treePieces.Add(treePiece);
                                            nombre_document_charge++;
                                            nombre_document_rejet++;
                                            treePiece.Image = Resources.document_rejeter;
                                        }
                                    }
                                }
                                i++;
                            }
                        }
                    }

                    /*Fiche de contrôle */
                    if (_docTri != null)
                    {
                        foreach (Node document in _docTri)
                        {
                            if (!document.Name.Contains(ConfigurationManager.AppSettings["PGD"]))
                            {
                                treePiece = new RadTreeNode();
                                treePiece.Name = document.Name;
                                treePiece.Text = document.Name;
                                treePiece.Value = document.ID;
                                drvues = dt_vues.Select("id_vues=" + document.ID);
                                if (drvues.Count().Equals(0))
                                {
                                    if (phase.Equals(1))
                                    {
                                        nombre_document_charge++;
                                        treePiece.Image = Resources.document;
                                        treePieces.Add(treePiece);
                                    }
                                }
                                else
                                {
                                    if (phase.Equals(1))
                                    {
                                        if (drvues[0][3].Equals(3))
                                        {
                                            nombre_document_valider++;
                                            treePiece.Image = Resources.document_valider;
                                        }
                                        else if (drvues[0][3].Equals(4))
                                        {
                                            nombre_document_rejet++;
                                            treePiece.Image = Resources.document_rejeter;
                                        }
                                        else
                                        {
                                            treePiece.Image = Resources.document;
                                        }
                                        nombre_document_charge++;
                                        treePieces.Add(treePiece);
                                    }
                                    else if (phase.Equals(2))
                                    {
                                        if (drvues[0][3].Equals(5))
                                        {
                                            nombre_document_corriger++;
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document_corriger;
                                        }
                                        if (drvues[0][3].Equals(4))
                                        {
                                            treePiece.Image = Resources.document_rejeter;
                                            nombre_document_rejet++;
                                            nombre_document_charge++;
                                            treePieces.Add(treePiece);
                                        }

                                    }
                                    else if (phase.Equals(3))
                                    {
                                        if (drvues[0][3].Equals(5))
                                        {
                                            nombre_document_corriger++;
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document_corriger;
                                            treePieces.Add(treePiece);
                                        }
                                        
                                        else if (drvues[0][3].Equals(4))
                                        {
                                            treePieces.Add(treePiece);
                                            nombre_document_charge++;
                                            nombre_document_rejet++;
                                            treePiece.Image = Resources.document_rejeter;
                                        }
                                    }
                                }
                                i++;
                            }
                        }
                    }
                    /* Sous Dossier */
                    i = 0;
                    foreach (Node sub_folder in _nodesTri)
                    {
                        treeSousDossier = new RadTreeNode();
                        treeSousDossier.Name = sub_folder.Name;
                        treeSousDossier.Text = sub_folder.Name;
                        treeSousDossier.Value = sub_folder.ID;
                        treeSousDossier.Image = Resources.folder_open;
                        nodes = service_ged.get_List_Nodes(this.authToken, sub_folder.ID);
                        if (nodes != null)
                        {
                            var _SD_doc_Tri = nodes.OrderBy(x => x.Name);
                            var _SD_PG = _SD_doc_Tri.Where(x => x.Name.Contains(ConfigurationManager.AppSettings["PGSD"]));
                            /*Page de garde du sous dossier*/
                            if (_SD_PG != null)
                            {
                                foreach (Node document in _SD_PG)
                                {
                                    treePiece = new RadTreeNode();
                                    treePiece.Name = document.Name;
                                    treePiece.Text = document.Name;
                                    treePiece.Value = document.ID;
                                    drvues = dt_vues.Select("id_vues=" + document.ID);
                                    if (drvues.Count().Equals(0))
                                    {
                                        //maria 18 03 2016
                                        //if (phase.Equals(1))
                                        //{
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treeSousDossier.Nodes.Add(treePiece);
                                        //}
                                       
                                    }
                                    else
                                    {
                                        if (phase.Equals(1))
                                        {
                                            if (drvues[0][3].Equals(3))
                                            {
                                                nombre_document_valider++;
                                                treePiece.Image = Resources.document_valider;
                                            }
                                            else if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_rejet++;
                                                treePiece.Image = Resources.document_rejeter;

                                            }
                                            else
                                            {
                                                treePiece.Image = Resources.document;
                                            }
                                            nombre_document_charge++;
                                            treeSousDossier.Nodes.Add(treePiece);
                                        }
                                        else if (phase.Equals(2))
                                        {
                                            if (drvues[0][3].Equals(5))
                                            {
                                                nombre_document_corriger++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_corriger;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                            if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_rejet++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_rejeter;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }

                                        }
                                        else if (phase.Equals(3))
                                        {
                                            if (drvues[0][3].Equals(5))
                                            {
                                                nombre_document_corriger++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_corriger;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                            else if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_charge++;
                                                nombre_document_rejet++;
                                                treePiece.Image = Resources.document_rejeter;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                        }
                                    }
                                    i++;
                                }
                            }

                            /*Document du sous dossier*/
                            foreach (Node document in _SD_doc_Tri)
                            {
                                if (!document.Name.Contains(ConfigurationManager.AppSettings["PGSD"]))
                                {
                                    treePiece = new RadTreeNode();
                                    Regex regex = new Regex(@".*_[0-9]$");
                                    Match match = regex.Match(document.Name);
                                    if (match.Success)
                                    {
                                        treePiece.Text = document.Name.Substring(0, document.Name.Length - 2) + "_0" + document.Name.Substring(document.Name.IndexOf("_") + 1);
                                        treePiece.Name = document.Name.Substring(0, document.Name.Length - 2) + "_0" + document.Name.Substring(document.Name.IndexOf("_") + 1);
                                    }
                                    else
                                    {
                                        treePiece.Text = document.Name;
                                        treePiece.Name = document.Name;
                                    }
                                    treePiece.Value = document.ID;
                                    drvues = dt_vues.Select("id_vues=" + document.ID);
                                    if (drvues.Count().Equals(0))
                                    {
                                        //maria 18 03 2016
                                        //if (phase.Equals(1))
                                        //{
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treeSousDossier.Nodes.Add(treePiece);
                                        //}
                                    }
                                    else
                                    {
                                        if (phase.Equals(1))
                                        {
                                            if (drvues[0][3].Equals(3))
                                            {
                                                nombre_document_valider++;
                                                treePiece.Image = Resources.document_valider;

                                            }
                                            else if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_rejet++;
                                                treePiece.Image = Resources.document_rejeter;

                                            }
                                            else
                                            {
                                                treePiece.Image = Resources.document;
                                            }
                                            nombre_document_charge++;
                                            treeSousDossier.Nodes.Add(treePiece);
                                        }
                                        else if (phase.Equals(2))
                                        {
                                            if (drvues[0][3].Equals(5))
                                            {
                                                nombre_document_corriger++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_corriger;
                                                treeSousDossier.Nodes.Add(treePiece);

                                            }
                                            if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_rejet++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_rejeter;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                        }
                                        else if (phase.Equals(3))
                                        {
                                            if (drvues[0][3].Equals(5))
                                            {
                                                nombre_document_corriger++;
                                                nombre_document_charge++;
                                                treePiece.Image = Resources.document_corriger;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                            else if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_charge++;
                                                nombre_document_rejet++;
                                                treePiece.Image = Resources.document_rejeter;
                                                treeSousDossier.Nodes.Add(treePiece);
                                            }
                                        }
                                    }
                                    treeSousDossier.Nodes.OrderBy(x => x.Text);
                                }
                            }
                        }
                        if (!treeSousDossier.Nodes.Count.Equals(0))
                        {
                            treeSousDossiers.Add(treeSousDossier);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //BW_Fermer dossier
        private void bWFermerdossier_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                /*Mise a jour*/
                da_dossier.Update(dt_dossier);
                da_vues.Update(dt_vues);
                da_vues_historique.Update(dt_vues_historique);

                /*librer la mémoire*/
                if (da_vues != null)
                    dt_vues.Dispose();
                if (dtformalite != null)
                    dtformalite.Dispose();
                if (dtpiece != null)
                    dtpiece.Dispose();
                nombre_document_charge = 0;
                nombre_document_valider = 0;
                nombre_document_rejet = 0;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Charger l'image
        private void bWImage_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                vues = service_ged.get_Node(authToken, idDocument);
                image_stream = service_ged.dowlande(authToken, "", vues);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Charger les index
        private void bWIndex_DoWork(object sender, DoWorkEventArgs e)
        {
            vues = service_ged.get_Node(authToken, idDocument);
            if (vues.Metadata != null)
            {
                element = new Elements();
                Metadata data = vues.Metadata;
                if (vues.VersionInfo != null)
                {
                    type_image = vues.VersionInfo.MimeType;
                    taille_image = vues.VersionInfo.FileDataSize;
                }
                element = service_ged.getElement(element, data);
                dr_vues_index = dt_vues.Select("id_vues="+vues.ID);
                if (type_dossier.Equals("DF"))
                {
                    dtformalite = new DataTable();
                    da_sd_manquent = new SqlDataAdapter("SELECT NSDM as [Numéro SD],[libelle_SD] as [Libellé de sous dossier],[id_DF] as [ID Dossier],etat as Status FROM [GED].[GED].[TB_SD_Manquent] where id_DF=" + vues.ParentID, service_data.getConnextion());
                    cmdBuilder_sd_manquent = new SqlCommandBuilder(da_sd_manquent);
                    da_sd_manquent.Fill(dtformalite);
                }
                if (type_dossier.Equals("SDF"))
                {
                    dtpiece = new DataTable();
                    da_piece_manquent = new SqlDataAdapter("SELECT N_view_m as [Numéro Piéce],[libelle] as [Libellé du piéce],[id_DF] as [ID Dossier],[id_SDF] as  [ID Sous Dossier] ,etat as Status FROM [GED].[GED].[TB_View_Manquent] where id_SDF=" + vues.ParentID, service_data.getConnextion());
                    cmdBuilder_piece_manquent = new SqlCommandBuilder(da_piece_manquent);
                    da_piece_manquent.Fill(dtpiece);
                }
            }
        }


        private void bWChargerdossier_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        } 

        //Affichage des index
        private void bWIndex_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
                
                /* Formulair Dossier */
                txt_nature_orgine.Text = element.nature_Orgine;
                txt_numero_orgine.Text = element.numero_Orgine.ToString();
                txt_indice_orgine.Text = element.indice_orgine;
                txt_indice_speciale_orgine.Text = element.indice_special_orgine.ToString();
                txt_numero_titre.Text = element.numero_Titre.ToString();
                txt_indice_titre.Text = element.indice_titre;
                txt_indice_spciale_titre.Text = element.indice_special_titre;

            /* Formulaire Sous Dossier */
            if (tree_unite_dossier.SelectedNode.Level.Equals(4))
            {
                txt_numero_sd.Text = element.numero_sousDossier.ToString();
                txt_formalite.Text = element.formaliteFR;
                txt_volume_depot.Text = element.volume_depot;
                txt_numero_depot.Text = element.numero_depot;
                txt_date_depot.Text = element.date_depot.ToString("dd/MM/yyyy");
            }

            /* Formulaire Piece */
            txtNompiece.Text = element.nom_piece;
            txtnbrpage.Text = element.nombre_page.ToString();
            txtnumpage.Text = element.numero_page.ToString();

            /*Information sur l'image*/
            lbl_taille_image.Text = string.Format("Taille d'image (KB) : {0} ",taille_image);
            if (taille_image >= 0x400) {
                lbl_taille_image.Text = string.Format("Taille d'image : {0} KB ", (taille_image) / 1024);
                 
            }
            lbl_type_image.Text = string.Format("Type d'image : {0}", type_image);
            
            /*Recuperation des données */
            if (ConfigurationManager.AppSettings["role"].Equals("1"))
            {
                if (!dr_vues_index.Count().Equals(0))
                {
                    if (tree_unite_dossier.SelectedNode.Level.Equals(3))
                    {
                        //pour la phase 1
                        ck_nature_orgine_rjt.Checked = ((int)dr_vues_index[0]["nature_orgine"] == 0) ? false : true;
                        ck_numero_orgine_rjt.Checked = ((int)dr_vues_index[0]["numero_orgine"] == 0) ? false : true;
                        ck_indice_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_orgine"] == 0) ? false : true;
                        ck_indice_special_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_special_orgine"] == 0) ? false : true;
                        ck_numero_titre_rjt.Checked = ((int)dr_vues_index[0]["numero_titre"] == 0) ? false : true;
                        ck_indice_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_titre"] == 0) ? false : true;
                        ck_indice_speciale_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_speciale_titre"] == 0) ? false : true;

                        // pour la phase 2
                        ck_nature_orgine_cr.Checked = ((int)dr_vues_index[0]["nature_orgine_cr"] == 0) ? false : true;
                        ck_numero_orgine_cr.Checked = ((int)dr_vues_index[0]["numero_orgine_cr"] == 0) ? false : true;
                        ck_indice_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_orgine_cr"] == 0) ? false : true;
                        ck_indice_special_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_special_orgine_cr"] == 0) ? false : true;
                        ck_numero_titre_cr.Checked = ((int)dr_vues_index[0]["numero_titre_cr"] == 0) ? false : true;
                        ck_indice_titre_cr.Checked = ((int)dr_vues_index[0]["indice_titre_cr"] == 0) ? false : true;
                        ck_indice_speciale_titre_cr.Checked = ((int)dr_vues_index[0]["indice_speciale_titre_cr"] == 0) ? false : true;
                        ck_sd_manquent_cr.Checked = ((int)dr_vues_index[0]["sd_manquent_cr"] == 0) ? false : true;
                    }

                    if (tree_unite_dossier.SelectedNode.Level.Equals(4) && type_dossier.Equals("SDF"))
                    {

                        //pour la phase 1
                        ck__numero_sd_rjt.Checked = ((int)dr_vues_index[0]["numero_sd"] == 0) ? false : true;
                        ck_formalite_rjt.Checked = ((int)dr_vues_index[0]["formalite"] == 0) ? false : true;
                        ck_volume_depot_rjt.Checked = ((int)dr_vues_index[0]["volume_depot"] == 0) ? false : true;
                        ck_numero_depot_rjt.Checked = ((int)dr_vues_index[0]["numero_depot"] == 0) ? false : true;
                        ck_date_depot_rjt.Checked = ((int)dr_vues_index[0]["date_depot"] == 0) ? false : true;

                        //pour la phase 2
                        ck__numero_sd_cr.Checked = ((int)dr_vues_index[0]["numero_sd_cr"] == 0) ? false : true;
                        ck_formalite_cr.Checked = ((int)dr_vues_index[0]["formalite_cr"] == 0) ? false : true;
                        ck_volume_depot_cr.Checked = ((int)dr_vues_index[0]["volume_depot_cr"] == 0) ? false : true;
                        ck_numero_depot_cr.Checked = ((int)dr_vues_index[0]["numero_depot_cr"] == 0) ? false : true;
                        ck_date_depot_cr.Checked = ((int)dr_vues_index[0]["date_depot_cr"] == 0) ? false : true;
                        ck_piece_manquent_cr.Checked = ((int)dr_vues_index[0]["piece_manquent_cr"] == 0) ? false : true;
                    }

                    //phase 1 
                    ck_nom_piece_rjt.Checked = ((int)dr_vues_index[0]["nom_piece"] == 0) ? false : true; ;
                    ck_nombre_page_rjt.Checked = ((int)dr_vues_index[0]["nombre_page"] == 0) ? false : true;
                    ck_num_page_rjt.Checked = ((int)dr_vues_index[0]["num_page"] == 0) ? false : true;

                    //phase 2
                    ck_nom_piece_cr.Checked = ((int)dr_vues_index[0]["nom_piece_cr"] == 0) ? false : true;
                    ck_nombre_page_cr.Checked = ((int)dr_vues_index[0]["nombre_page_cr"] == 0) ? false : true;
                    ck_num_page_cr.Checked = ((int)dr_vues_index[0]["num_page_cr"] == 0) ? false : true;

                    // phase 1
                    ck_qlt_image_rjt.Checked = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;
                    ck_taille_image_rjt.Checked = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;
                    ck_mal_classe_rjt.Checked = ((int)dr_vues_index[0]["mal_classe"] == 0) ? false : true;
                    ck_a_supprimer_rjt.Checked = ((int)dr_vues_index[0]["A_SUPPRIMER"] == 0) ? false : true;
                    

                    // phase 2
                    ck_qlt_image_cr.Checked = ((int)dr_vues_index[0]["qualite_image_cr"] == 0) ? false : true;
                    ck_taille_image_cr.Checked = ((int)dr_vues_index[0]["taille_image_cr"] == 0) ? false : true;
                    ck_mal_classe_cr.Checked = ((int)dr_vues_index[0]["mal_classe_cr"] == 0) ? false : true;
                    ck_qlt_img_org_illisible_cr.Checked = ((int)dr_vues_index[0]["img_orgine_illisible_cr"] == 0) ? false : true;

                    if (phase.Equals(1))
                    {
                        if (dr_vues_index[0][3].Equals(3))
                            tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                        else
                            tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                    }
                    if (type_dossier.Equals("DF"))
                    {
                        GridViewFormalite.DataSource = dtformalite;
                        GridViewFormalite.Refresh();
                    }
                    else if (type_dossier.Equals("SDF"))
                    {
                        GridViewPiece.DataSource = dtpiece;
                        GridViewPiece.Refresh();
                    }
                    else
                    {
                        if (dtpiece != null)
                        {
                            dtpiece.Rows.Clear();
                            GridViewPiece.DataSource = dtpiece;
                            GridViewPiece.Refresh();
                        }
                    }
                }
            }
            else if(ConfigurationManager.AppSettings["role"].Equals("2"))
            {
                if (!dr_vues_index.Count().Equals(0))
                {
                    //phase 1 Dossier
                    ck_nature_orgine_rjt.Checked = ((int)dr_vues_index[0]["nature_orgine"] == 0) ? false : true;
                    ck_nature_orgine_cr.Enabled = ((int)dr_vues_index[0]["nature_orgine"] == 0) ? false : true;

                    ck_numero_orgine_rjt.Checked = ((int)dr_vues_index[0]["numero_orgine"] == 0) ? false : true;
                    ck_numero_orgine_cr.Enabled = ((int)dr_vues_index[0]["numero_orgine"] == 0) ? false : true;

                    ck_indice_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_orgine"] == 0) ? false : true;
                    ck_indice_orgine_cr.Enabled = ((int)dr_vues_index[0]["indice_orgine"] == 0) ? false : true;

                    ck_indice_special_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_special_orgine"] == 0) ? false : true;
                    ck_indice_special_orgine_cr.Enabled = ((int)dr_vues_index[0]["indice_special_orgine"] == 0) ? false : true;

                    ck_numero_titre_rjt.Checked = ((int)dr_vues_index[0]["numero_titre"] == 0) ? false : true;
                    ck_numero_titre_cr.Enabled = ((int)dr_vues_index[0]["numero_titre"] == 0) ? false : true;

                    ck_indice_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_titre"] == 0) ? false : true;
                    ck_indice_titre_cr.Enabled = ((int)dr_vues_index[0]["indice_titre"] == 0) ? false : true;

                    ck_indice_speciale_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_speciale_titre"] == 0) ? false :true;
                    ck_indice_speciale_titre_cr.Enabled = ((int)dr_vues_index[0]["indice_speciale_titre"] == 0) ? false : true;


                    //phase 2 Dossier
                    
                    ck_nature_orgine_cr.Checked = ((int)dr_vues_index[0]["nature_orgine_cr"] == 0) ? false : true;
                    ck_numero_orgine_cr.Checked = ((int)dr_vues_index[0]["numero_orgine_cr"] == 0) ? false : true;
                    ck_indice_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_orgine_cr"] == 0) ? false : true;
                    ck_indice_special_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_special_orgine_cr"] == 0) ? false : true;
                    ck_numero_titre_cr.Checked = ((int)dr_vues_index[0]["numero_titre_cr"] == 0) ? false : true;
                    ck_indice_titre_cr.Checked = ((int)dr_vues_index[0]["indice_titre_cr"] == 0) ? false : true;
                    ck_indice_speciale_titre_cr.Checked = ((int)dr_vues_index[0]["indice_speciale_titre_cr"] == 0) ? false : true;
                    ck_sd_manquent_cr.Checked = ((int)dr_vues_index[0]["sd_manquent_cr"] == 0) ? false : true;

                    //phase 1 sous dossier
                    ck__numero_sd_rjt.Checked = ((int)dr_vues_index[0]["numero_sd"] == 0) ? false : true;
                    ck__numero_sd_cr.Enabled = ((int)dr_vues_index[0]["numero_sd"] == 0) ? false : true;

                    ck_formalite_rjt.Checked = ((int)dr_vues_index[0]["formalite"] == 0) ? false : true;
                    ck_formalite_cr.Enabled = ((int)dr_vues_index[0]["formalite"] == 0) ? false : true;

                    ck_volume_depot_rjt.Checked = ((int)dr_vues_index[0]["volume_depot"] == 0) ? false : true;
                    ck_volume_depot_cr.Enabled = ((int)dr_vues_index[0]["volume_depot"] == 0) ? false : true;

                    ck_numero_depot_rjt.Checked = ((int)dr_vues_index[0]["numero_depot"] == 0) ? false : true;
                    ck_numero_depot_cr.Enabled = ((int)dr_vues_index[0]["numero_depot"] == 0) ? false : true;
                    
                    ck_date_depot_rjt.Checked = ((int)dr_vues_index[0]["date_depot"] == 0) ? false : true;
                    ck_date_depot_cr.Enabled = ((int)dr_vues_index[0]["date_depot"] == 0) ? false : true;

                    //phase 2 sous dossier
                    ck__numero_sd_cr.Checked = ((int)dr_vues_index[0]["numero_sd_cr"] == 0) ? false : true;
                    ck_formalite_cr.Checked = ((int)dr_vues_index[0]["formalite_cr"] == 0) ? false : true;
                    ck_volume_depot_cr.Checked = ((int)dr_vues_index[0]["volume_depot_cr"] == 0) ? false : true;
                    ck_numero_depot_cr.Checked = ((int)dr_vues_index[0]["numero_depot_cr"] == 0) ? false : true;
                    ck_date_depot_cr.Checked = ((int)dr_vues_index[0]["date_depot_cr"] == 0) ? false : true;
                    ck_piece_manquent_cr.Checked = ((int)dr_vues_index[0]["piece_manquent_cr"] == 0) ? false : true;

                    //phase 1 pièce
                    ck_nom_piece_rjt.Checked = ((int)dr_vues_index[0]["nom_piece"] == 0) ? false : true;
                    ck_nom_piece_cr.Enabled = ((int)dr_vues_index[0]["nom_piece"] == 0) ? false : true;

                    ck_nombre_page_rjt.Checked = ((int)dr_vues_index[0]["nombre_page"] == 0) ? false : true;
                    ck_nombre_page_cr.Enabled = ((int)dr_vues_index[0]["nombre_page"] == 0) ? false : true;

                    ck_num_page_rjt.Checked = ((int)dr_vues_index[0]["num_page"] == 0) ? false : true;
                    ck_num_page_cr.Enabled = ((int)dr_vues_index[0]["num_page"] == 0) ? false : true;

                    //phase 2 pièce
                    ck_nom_piece_cr.Checked = ((int)dr_vues_index[0]["nom_piece_cr"] == 0) ? false : true;
                    ck_nombre_page_cr.Checked = ((int)dr_vues_index[0]["nombre_page_cr"] == 0) ? false : true;
                    ck_num_page_cr.Checked = ((int)dr_vues_index[0]["num_page_cr"] == 0) ? false : true;

                    // phase 1 pièce
                    ck_qlt_image_rjt.Checked = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;
                    ck_qlt_image_cr.Enabled = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;
                    ck_qlt_img_org_illisible_cr.Enabled = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;

                    ck_a_supprimer_rjt.Checked = ((int)dr_vues_index[0]["A_SUPPRIMER"] == 0) ? false : true;

                    ck_taille_image_rjt.Checked = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;
                    ck_taille_image_cr.Enabled = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;
                    ck_taille_image_cr.Enabled = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;


                    ck_mal_classe_rjt.Checked = ((int)dr_vues_index[0]["mal_classe"] == 0) ? false : true;
                    ck_mal_classe_cr.Enabled = ((int)dr_vues_index[0]["mal_classe"] == 0) ? false : true;

                    // phase 2 pièce
                    ck_qlt_image_cr.Checked = ((int)dr_vues_index[0]["qualite_image_cr"] == 0) ? false : true;
                    ck_taille_image_cr.Checked = ((int)dr_vues_index[0]["taille_image_cr"] == 0) ? false : true;
                    ck_mal_classe_cr.Checked = ((int)dr_vues_index[0]["mal_classe_cr"] == 0) ? false : true;
                    
                    ck_qlt_img_org_illisible_cr.Checked = ((int)dr_vues_index[0]["img_orgine_illisible_cr"] == 0) ? false : true;

                    //supprimer
                    cb_suppri_cr.Enabled = ((int)dr_vues_index[0]["A_SUPPRIMER"] == 0) ? false : true;
                    //

                    if (type_dossier.Equals("DF"))
                    {
                        GridViewFormalite.DataSource = dtformalite;
                        GridViewFormalite.Refresh();
                        ck_sd_manquent_cr.Enabled = true;
                        ck_piece_manquent_cr.Enabled = false;
                    }
                    else if (type_dossier.Equals("SDF"))
                    {
                        GridViewPiece.DataSource = dtpiece;
                        GridViewPiece.Refresh();
                        ck_piece_manquent_cr.Enabled = true;
                        ck_sd_manquent_cr.Enabled = false;
                    }
                    else 
                    {
                        if (dtformalite != null)
                        {
                            dtformalite.Rows.Clear();
                            GridViewFormalite.DataSource = dtformalite;
                            GridViewPiece.Refresh();
                        }
                        if (dtpiece != null)
                        {
                            dtpiece.Rows.Clear();
                            GridViewPiece.DataSource = dtpiece;
                            GridViewPiece.Refresh();
                        }
                        ck_sd_manquent_cr.Enabled = false;
                        ck_piece_manquent_cr.Enabled = false;
                    }
                }
            }

            /*List Formalité*/
            if (type_dossier.Equals("DF"))
            {
                GridViewFormalite.DataSource = dtformalite;
                if (dtformalite.Rows.Count.Equals(0))
                {
                    ck_sd_manquent_cr.Enabled = false;
                }
                /*Formalite*/
                btnajouterFormalite.Enabled = true;
                cbolistFormalite.Enabled = true;
                GridViewFormalite.Enabled = true;
                txtnSD.Enabled = true; 

                /*Piece*/
                btnAjoutePiece.Enabled = false;
                cbolistPiece.Enabled = false;
                GridViewPiece.Enabled = false;
                txtNpiece.Enabled = false;
            }
            else if (type_dossier.Equals("SDF"))
            {
                GridViewPiece.DataSource = dtpiece;
                if (dtpiece.Rows.Count.Equals(0))
                {
                    ck_piece_manquent_cr.Enabled = false;
                }
                /*Formalite*/
                btnajouterFormalite.Enabled = false;
                cbolistFormalite.Enabled = false;
                GridViewFormalite.Enabled = false;
                txtnSD.Enabled = false;

                /*Piece*/
                btnAjoutePiece.Enabled = true;
                cbolistPiece.Enabled = true;
                GridViewPiece.Enabled = true;
                txtNpiece.Enabled = true;
            }
            else 
            {
                btnAjoutePiece.Enabled = false;
                btnajouterFormalite.Enabled = false;
                cbolistFormalite.Enabled = false;
                cbolistPiece.Enabled = false;
                GridViewFormalite.Enabled = false;
                GridViewPiece.Enabled = false;
                txtnSD.Enabled = false;
                txtNpiece.Enabled = false;
            }
            ////forc_correct

            if (cb_forc.Checked)
            {
                ck_nature_orgine_cr.Checked = ((int)dr_vues_index[0]["nature_orgine"] == 0) ? false : true;
                ck_numero_orgine_cr.Checked = ((int)dr_vues_index[0]["numero_orgine"] == 0) ? false : true;
                ck_indice_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_orgine"] == 0) ? false : true;
                ck_indice_special_orgine_cr.Checked = ((int)dr_vues_index[0]["indice_special_orgine"] == 0) ? false : true;
                ck_numero_titre_cr.Checked = ((int)dr_vues_index[0]["numero_titre"] == 0) ? false : true;
                ck_indice_titre_cr.Checked = ((int)dr_vues_index[0]["indice_titre"] == 0) ? false : true;
                ck_indice_speciale_titre_cr.Checked = ((int)dr_vues_index[0]["indice_speciale_titre"] == 0) ? false : true;
                if (dtformalite != null)
                    ck_sd_manquent_cr.Checked =true;
                ck__numero_sd_cr.Checked = ((int)dr_vues_index[0]["numero_sd"] == 0) ? false : true;
                ck_formalite_cr.Checked = ((int)dr_vues_index[0]["formalite"] == 0) ? false : true;
                ck_volume_depot_cr.Checked = ((int)dr_vues_index[0]["volume_depot"] == 0) ? false : true;
                ck_numero_depot_cr.Checked = ((int)dr_vues_index[0]["numero_depot"] == 0) ? false : true;
                ck_date_depot_cr.Checked = ((int)dr_vues_index[0]["date_depot"] == 0) ? false : true;
                if (dtpiece != null)
                    ck_piece_manquent_cr.Checked =true;
                ck_nom_piece_cr.Checked = ((int)dr_vues_index[0]["nom_piece"] == 0) ? false : true;
                ck_nombre_page_cr.Checked = ((int)dr_vues_index[0]["nombre_page"] == 0) ? false : true;
                ck_num_page_cr.Checked = ((int)dr_vues_index[0]["num_page"] == 0) ? false : true;
                ck_qlt_image_cr.Checked = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;
                ck_taille_image_cr.Checked = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;
                ck_mal_classe_cr.Checked = ((int)dr_vues_index[0]["mal_classe"] == 0) ? false : true;
            }
            
            ////
            
        }

        //Affichage de l'image
        private void bWImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.pb_vues.Width = 430;
                this.pb_vues.Height = 650;
                this.pb_vues.SizeMode = PictureBoxSizeMode.Zoom;
                image = (Bitmap)Image.FromStream(image_stream, true, false);//new Bitmap(image_stream);
                this.pb_vues.Image = image;
                defImgHeight = image.Height;
                defImgWidth = image.Width;
                WaitingBarTraitement.StopWaiting();
                WaitingBarTraitement.Visible = false;
                toolImage.Enabled = true;
                btnPageSuivant.Enabled = true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally 
            {
                
            }
        }


        private void bWChargerdossier_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (tree_unite_dossier.SelectedNode.Nodes.Count > 0)
            {
                try
                {
                    bool trouver = false;
                    IEnumerable<RadTreeNode> _nodes = tree_unite_dossier.SelectedNode.Nodes;
                    foreach (RadTreeNode node in _nodes)
                    {
                        foreach (RadTreeNode _node in treePieces)
                        {
                            if (node.Name == _node.Name)
                            {
                                treePieces.Remove(_node);
                                trouver = true;
                                break;
                            }
                        }
                        if (!trouver)
                        {
                            foreach (RadTreeNode _node in treeSousDossiers)
                            {
                                if (node.Name == _node.Name)
                                {
                                    treeSousDossiers.Remove(_node);
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (treePieces != null)
            {
                if (treePieces.Count != 0)
                {
                    this.tree_unite_dossier.SelectedNode.Nodes.AddRange(treePieces);
                }
            }

            if (treeSousDossiers != null)
            {
                if (treeSousDossiers.Count != 0)
                    this.tree_unite_dossier.SelectedNode.Nodes.AddRange(treeSousDossiers);
            }

            if (tree_unite_dossier.SelectedNode.Nodes.Count > 0)
            {
                this.tree_unite_dossier.SelectedNode.Expand();
                menuAction.Items[0].Enabled = false;
                menuAction.Items[1].Enabled = true;
                tree_unite_dossier.SelectedNode.Image = Resources.folder_edit;
            }
            else
            {
                menuAction.Items[0].Enabled = true;
                menuAction.Items[1].Enabled = false;
            }

            if (phase.Equals(1))
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
            else if (phase.Equals(2))
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues corrigées : {0}", nombre_document_corriger);
            }
            else if (phase.Equals(3))
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
            this.tree_unite_dossier.Enabled = true;
            WaitingBarTraitement.StopWaiting();
            WaitingBarTraitement.Visible = false;
        }

        //BW_Fermer dossier
        private void bWFermerdossier_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tree_unite_dossier.SelectedNode.Nodes.Clear();
            menuAction.Items[0].Enabled = true;
            menuAction.Items[1].Enabled = false;
            WaitingBarTraitement.StopWaiting();
            WaitingBarTraitement.Visible = false;
        }
        
        //Ajouter Formalite SD
        private void btnajouterFormalite_Click(object sender, EventArgs e)
        {
            DataRow n_formalite = dtformalite.NewRow();
            n_formalite[0] = txtnSD.Text;
            n_formalite[1] = cbolistFormalite.SelectedValue;
            n_formalite[2] = idDossier;
            n_formalite[3] = 0;
            dtformalite.Rows.Add(n_formalite);
            GridViewFormalite.Refresh();
        }

        //Ajouter Piece
        private void btnAjoutePiece_Click(object sender, EventArgs e)
        {
            DataRow n_piece = dtpiece.NewRow();
            n_piece[0] = txtNpiece.Text;
            n_piece[1] = cbolistPiece.SelectedValue;
            n_piece[2] = idDossier;
            n_piece[3] = idSDDossier;
            n_piece[4] = 0;
            dtpiece.Rows.Add(n_piece);
            GridViewPiece.Refresh();
        }

        //page suivante
        private void btnPageSuivant_Click(object sender, EventArgs e)
        {
            btnPageSuivant.Enabled = false;
            RadTreeNode nodeV = new RadTreeNode();
            
            if (radGroupBoxDossier.Enabled == false && radGroupBoxSousDossier.Enabled == false && radGroupBoxPiece_rjt.Enabled == false)
            {
                if (tree_unite_dossier.SelectedNode.NextNode != null)
                {
                    nodeV = tree_unite_dossier.SelectedNode.NextNode;
                    if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Folder")
                    {
                        nodeV.Expand();
                        IEnumerable<RadTreeNode> _nodes = nodeV.Nodes;
                        foreach (RadTreeNode _node in _nodes)
                        {
                            nodeV = _node;
                            if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Document")
                            {
                                break;
                            }
                        }
                    }
                    tree_unite_dossier.SelectedNode = nodeV;
                }
                else
                {
                    tree_unite_dossier.SelectedNode.Parent.Expanded = false;
                    if (tree_unite_dossier.SelectedNode.Parent.NextNode != null)
                    {
                        nodeV = tree_unite_dossier.SelectedNode.Parent.NextNode;
                        if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Folder")
                        {
                            nodeV.Expand();
                            IEnumerable<RadTreeNode> _nodes = nodeV.Nodes;
                            foreach (RadTreeNode _node in _nodes)
                            {
                                nodeV = _node;
                                if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Document")
                                {
                                    break;
                                }
                            }
                        }
                        tree_unite_dossier.SelectedNode = nodeV;
                    }
                }
            }

            else
            {
                if (tree_unite_dossier.SelectedNode.NextNode != null)
                {
                if (phase.Equals(1))
                {
                    save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                    if (document_isvalide())
                    {
                        if (type_dossier.Equals("DF") && !dtformalite.Rows.Count.Equals(0))
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                        }
                        else if (type_dossier.Equals("SDF") && !dtpiece.Rows.Count.Equals(0))
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                        }
                        else
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                        }
                    }
                    else
                        tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                    
                }
                else if (phase.Equals(3))
                {
                    save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                    DataRow[] dr_formalite = null ;
                    DataRow[] dr_piece =null;
                    if(dtformalite!=null)
                        dr_formalite = dtformalite.Select("[ID Dossier]=" + tree_unite_dossier.SelectedNode.Parent.Value + " and Status=0");
                    if(dtpiece!=null)
                        dr_piece = dtpiece.Select("[ID Sous Dossier]=" + tree_unite_dossier.SelectedNode.Parent.Value + " and Status=0");
                    if (document_isvalide())
                    {
                        if (type_dossier.Equals("DF") && dr_formalite != null)
                        {
                            if (!dr_formalite.Count().Equals(0))
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                            }
                            else
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                            }
                        }
                        else if (type_dossier.Equals("SDF") && dr_piece!=null)
                        {
                            if (!dr_piece.Count().Equals(0))
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                            }
                            else
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                            }
                        }
                        else
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                        }
                    }
                    else
                        tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                }
                else if (phase.Equals(2))
                {
                    if (isToutesCorriger())
                    {
                        save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                        tree_unite_dossier.SelectedNode.Image = Resources.document_corriger;
                    }
                    else
                    {
                        MessageBox.Show("Merci de corriger tous les index", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnPageSuivant.Enabled = true;
                        return;
                    }
                    //supprim
                    if (cb_suppri_cr.Checked)
                    {
                        DialogResult result = MessageBox.Show("Voulez-vous vraiment supprimer cet élément? ", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                        supprimer = true;
                        service_ged.delete(authToken, (long)tree_unite_dossier.SelectedNode.Value);
                        
                    }
                    //
                }
                if (phase.Equals(2))
                {
                    label18.Visible = true;
                    tree_unite_dossier.Enabled = false;
                    Cursor.Current = Cursors.WaitCursor;
                    corriger();
                }
                nodeV = tree_unite_dossier.SelectedNode.NextNode;
            }
            else
            {
                if (phase.Equals(2))
                {
                    label18.Visible = true;
                    tree_unite_dossier.Enabled = false;
                    Cursor.Current = Cursors.WaitCursor;
                    corriger();
                }

                if (phase.Equals(1))
                {
                    save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                    if (document_isvalide())
                    {
                        if (type_dossier.Equals("DF") && !dtformalite.Rows.Count.Equals(0))
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                        }
                        else if (type_dossier.Equals("SDF") && !dtpiece.Rows.Count.Equals(0))
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                        }
                        else
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                        }
                    }
                    else
                        tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;

                }
                else if (phase.Equals(3))
                {
                    save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                    DataRow[] dr_formalite = null;
                    DataRow[] dr_piece = null;
                    if (dtformalite != null)
                        dr_formalite = dtformalite.Select("[ID Dossier]=" + tree_unite_dossier.SelectedNode.Parent.Value + " and Status=0");
                    if (dtpiece != null)
                        dr_piece = dtpiece.Select("[ID Sous Dossier]=" + tree_unite_dossier.SelectedNode.Parent.Value + " and Status=0");
                    if (document_isvalide())
                    {
                        if (type_dossier.Equals("DF") && dr_formalite != null)
                        {
                            if (!dr_formalite.Count().Equals(0))
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                            }
                            else
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                            }
                        }
                        else if (type_dossier.Equals("SDF") && dr_piece != null)
                        {
                            if (!dr_piece.Count().Equals(0))
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                            }
                            else
                            {
                                tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                            }
                        }
                        else
                        {
                            tree_unite_dossier.SelectedNode.Image = Resources.document_valider;
                        }
                    }
                    else
                        tree_unite_dossier.SelectedNode.Image = Resources.document_rejeter;
                }
                else if (phase.Equals(2))
                {
                    if (isToutesCorriger())
                    {
                        save_donnee_index((long)tree_unite_dossier.SelectedNode.Value);
                        tree_unite_dossier.SelectedNode.Image = Resources.document_corriger;
                    }
                    else
                    {
                        MessageBox.Show("Merci de corriger tous les index", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //supprim
                    if (cb_suppri_cr.Checked)
                    {
                        DialogResult result = MessageBox.Show("Voulez-vous vraiment supprimer cet élément? ", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                        supprimer = true;
                        service_ged.delete(authToken, (long)tree_unite_dossier.SelectedNode.Value);
                    }
                    //
                }
               
                tree_unite_dossier.SelectedNode.Parent.Expanded = false;
                nodeV = tree_unite_dossier.SelectedNode.Parent.NextNode;
                if (tree_unite_dossier.SelectedNode.Parent.NextNode == null)
                {
                    if (phase.Equals(1))
                    {
                        lblvue_rejeter.Text = string.Format("Nombre de vue rejetées : {0}", nombre_document_rejet);
                        lblnbr_valider.Text = string.Format("Nombre de vue validées : {0}", nombre_document_valider);
                    }
                    else if (phase.Equals(2))
                    {
                        //supprimer
                        cb_suppri_cr.Checked = false;
                        //
                        lblvue_rejeter.Text = string.Format("Nombre de vue rejetées : {0}", nombre_document_rejet);
                        lblnbr_valider.Text = string.Format("Nombre de vue corrigées : {0}", nombre_document_corriger);
                    }
                    else if (phase.Equals(3))
                    {
                        lblvue_rejeter.Text = string.Format("Nombre de vue rejetées : {0}", nombre_document_rejet);
                        lblnbr_valider.Text = string.Format("Nombre de vue validées : {0}", nombre_document_valider);
                    }
                    if (phase.Equals(3))
                        ck_valider_vue.Checked = false;
                    MessageBox.Show("Merci de Clôturer le dossier pour passer vers un autre dossier","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }
            }
            if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Folder")
            {
                nodeV.Expand();
                IEnumerable<RadTreeNode> _nodes = nodeV.Nodes;
                foreach (RadTreeNode _node in _nodes)
                {
                    nodeV = _node;
                    if ((node = service_ged.get_Node(authToken, (long)nodeV.Value)).Type == "Document")
                    {
                        break; 
                    }
                }
            }
            tree_unite_dossier.SelectedNode = nodeV;
            tree_unite_dossier.Refresh();
            if (phase.Equals(1))
            {
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
            else if (phase.Equals(2))
            {
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues corrigées : {0}", nombre_document_corriger);
            }
            else if (phase.Equals(3))
            {
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
            if (phase.Equals(3))
                ck_valider_vue.Checked = false;
                }
        }

        //vider les champ
        public void videchamp()
        {
            foreach (Control C in this.toolWindow2.Controls)
            {
                if (C.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    C.Text = "";
                }
                else if (C.GetType() == typeof(Telerik.WinControls.UI.RadGroupBox))
                {
                    foreach (Control C1 in C.Controls)
                    {
                        if (C1.GetType() == typeof(System.Windows.Forms.TextBox))
                        {
                            C1.Text = "";
                        }
                    }
                }
            }
        }

        //verifier si la vue est valide (aucun rejet)
        public bool document_isvalide()
        {
            foreach (Control c in this.toolWindow2.Controls)
            {
                if (c.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    if (((CheckBox)c).Name.Contains("_rjt"))
                    {
                        if (((CheckBox)c).Checked)
                        {
                            return false;
                        }
                    }
                }
                else if (c.GetType() == typeof(Telerik.WinControls.UI.RadGroupBox))
                {
                    foreach (Control c1 in c.Controls)
                    {
                        if (c1.GetType() == typeof(System.Windows.Forms.CheckBox))
                        {
                            if (((CheckBox)c1).Name.Contains("_rjt"))
                            {
                                if (((CheckBox)c1).Checked)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        //vider les checkBox
        public void intialiserControle(Control radBoxControle,string phase)
        {
            foreach (Control c in radBoxControle.Controls)
            {
                if (c.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    if (phase.Equals("1"))
                    {
                        if (((CheckBox)c).Name.Contains("_rjt"))
                            ((CheckBox)c).Checked = false;
                    }
                    else if (phase.Equals("2")) 
                    {
                        if (((CheckBox)c).Name.Contains("_cr"))
                            ((CheckBox)c).Checked = false;
                    }
                }
                if (c.GetType() == typeof(System.Windows.Forms.DataGridView))
                {
                    if (phase.Equals("1"))
                    {
                        ((DataGridView)c).DataSource = null;
                    }
                }
            }
        }

        public void activeCheckbox(string phase,bool active)
        {
            foreach (Control c in this.toolWindow2.Controls)
            {
                if (c.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    ((CheckBox)c).Enabled = active;
                }
                else if (c.GetType() == typeof(Telerik.WinControls.UI.RadGroupBox))
                {
                    foreach (Control c1 in c.Controls)
                    {
                        if (c1.GetType() == typeof(System.Windows.Forms.CheckBox))
                        {
                            if (phase.Equals("1"))
                            {
                                if (((CheckBox)c1).Name.Contains("_rjt"))
                                    ((CheckBox)c1).Enabled = active;
                            }
                            else if (phase.Equals("2"))
                            {
                                if (((CheckBox)c1).Name.Contains("_cr"))
                                    ((CheckBox)c1).Enabled = active;
                            }
                        }
                    }
                }
            }
        }

        public void save_donnee_index(long _idVue){
            //int nbr_vues=0;
            //maria
            SqlCommand cmd = new SqlCommand("select GETDATE()", con);
            con.Open();
            string date = cmd.ExecuteScalar().ToString();
            con.Close();

            if (type_dossier.Equals("DF"))
            {
                da_sd_manquent.Update(dtformalite);
            }
            else if (type_dossier.Equals("SDF"))
            {
                da_piece_manquent.Update(dtpiece);
            }

            DataRow[] dr_vues = dt_vues.Select("id_vues =" + _idVue);
            bool isValid = document_isvalide();
            if (dr_vues.Count().Equals(0))
            {
                
                DataRow dr_vue = dt_vues.NewRow();
                dr_vue["id_vues"] = _idVue;
                dr_vue["id_DF"] = idDossier;

                if (!idSDDossier.Equals(0))
                {
                    dr_vue["id_SDF"] = idSDDossier;
                }

                if (isValid)
                {
                    if (type_dossier.Equals("DF"))
                    {
                        if (!dtformalite.Rows.Count.Equals(0))
                        {
                            nombre_document_rejet++;
                            dr_vue["status"] = 4;
                        }
                        else
                        {
                            nombre_document_valider++;
                            dr_vue["status"] = 3;
                        }
                    }
                    else if (type_dossier.Equals("SDF"))
                    {
                        if (!dtpiece.Rows.Count.Equals(0))
                        {
                            nombre_document_rejet++;
                            dr_vue["status"] = 4;
                        }
                        else
                        {
                            nombre_document_valider++;
                            dr_vue["status"] = 3;
                        }
                    }
                    else if (!dr_vue["status"].Equals(3))
                    {
                        nombre_document_valider++;
                        dr_vue["status"] = 3;
                    }

                    if (phase == 2)
                    {
                        nombre_document_corriger++;
                        dr_vue["status"] = 5;
                    }

                }
                else
                {
                    if (!dr_vue["status"].Equals(4))
                    {
                        nombre_document_rejet++;
                        dr_vue["status"] = 4;
                    }

                    //supprimer
                    if (phase == 2)
                    {
                        if (cb_suppri_cr.Checked)
                        {
                            //nombre_document_corriger++;
                            dr_vue["status"] = 6;
                        }
                    }
                    //
                }
                
                // Dossier
                dr_vue["nature_orgine"] = ck_nature_orgine_rjt.Checked;
                dr_vue["numero_orgine"] = ck_numero_orgine_rjt.Checked;
                dr_vue["indice_orgine"] = ck_indice_orgine_rjt.Checked;
                dr_vue["indice_special_orgine"] = ck_indice_special_orgine_rjt.Checked;
                dr_vue["numero_titre"] = ck_numero_titre_rjt.Checked;
                dr_vue["indice_titre"] = ck_indice_titre_rjt.Checked;
                dr_vue["indice_speciale_titre"] = ck_indice_speciale_titre_cr.Checked;

                //sous dossier
                dr_vue["numero_sd"] = ck__numero_sd_rjt.Checked;
                dr_vue["formalite"] = ck_formalite_rjt.Checked;
                dr_vue["volume_depot"] = ck_volume_depot_rjt.Checked;
                dr_vue["numero_depot"] = ck_numero_depot_rjt.Checked;
                dr_vue["date_depot"] = ck_date_depot_rjt.Checked;

                // Pièce
                dr_vue["nom_piece"] = ck_nom_piece_rjt.Checked;
                dr_vue["nombre_page"] = ck_nombre_page_rjt.Checked;
                dr_vue["num_page"] = ck_num_page_rjt.Checked;

                dr_vue["qualite_image"] = ck_qlt_image_rjt.Checked;
                dr_vue["taille_image"] = ck_taille_image_rjt.Checked;

                dr_vue["mal_classe"] = ck_mal_classe_rjt.Checked;

                dr_vue["A_SUPPRIMER"] = ck_a_supprimer_rjt.Checked;

                //Dossier corriger
                dr_vue["nature_orgine_cr"] = ck_nature_orgine_cr.Checked;
                dr_vue["numero_orgine_cr"] = ck_numero_orgine_cr.Checked;
                dr_vue["indice_orgine_cr"] = ck_indice_orgine_cr.Checked;
                dr_vue["indice_special_orgine_cr"] = ck_indice_special_orgine_cr.Checked;
                dr_vue["numero_titre_cr"] = ck_numero_titre_cr.Checked;
                dr_vue["indice_titre_cr"] = ck_indice_titre_cr.Checked;
                dr_vue["indice_speciale_titre_cr"] = ck_indice_speciale_titre_cr.Checked;

                // Sous Dossier Corriger
                dr_vue["numero_sd_cr"] = ck__numero_sd_cr.Checked;
                dr_vue["formalite_cr"] = ck_formalite_cr.Checked;
                dr_vue["volume_depot_cr"] = ck_volume_depot_cr.Checked;
                dr_vue["numero_depot_cr"] = ck_numero_depot_cr.Checked;
                dr_vue["date_depot_cr"] = ck_date_depot_cr.Checked;

                //pièce
                dr_vue["nom_piece_cr"] = ck_nom_piece_cr.Checked;
                dr_vue["nombre_page_cr"] = ck_nombre_page_cr.Checked;
                dr_vue["num_page_cr"] = ck_num_page_cr.Checked;

                dr_vue["qualite_image_cr"] = ck_qlt_image_cr.Checked;
                dr_vue["taille_image_cr"] = ck_taille_image_cr.Checked;
                dr_vue["mal_classe_cr"] = ck_mal_classe_cr.Checked;
                dr_vue["img_orgine_illisible_cr"] = ck_qlt_img_org_illisible_cr.Checked;
                dr_vue["sd_manquent_cr"] = ck_sd_manquent_cr.Checked;
                dr_vue["piece_manquent_cr"] = ck_piece_manquent_cr.Checked;
                dr_vue["A_SUPPRIMER"] = ck_a_supprimer_rjt.Checked;

                if (phase.Equals(1))
                {
                    dr_vue["user_controle"] = id_user;
                }
                else if (phase.Equals(2))
                {
                    dr_vue["user_corriger"] = id_user;
                    
                }
                else if (phase.Equals(3))
                {
                    dr_vue["user_controle_corriger"] = id_user;
                }
                //dt_vues.Rows.Add(dr_vue);
                //maria 01 04 2016
                //nbr_vues++;
                //if (nbr_vues > 30)
                //{
                    //da_vues.Update(dt_vues);
                   // nbr_vues = 0;
               //}
                    if (phase.Equals(1))
                    {
                        dr_vue["date_controle"] = DateTime.Now;//Convert.ToDateTime(date); 
                    }
                    else if (phase.Equals(2))
                    {
                        dr_vue["date_corriger"] = DateTime.Now;//Convert.ToDateTime(date);
                    }
                    else if (phase.Equals(3))
                    {
                        dr_vue["date_controle_corriger"] = DateTime.Now;//Convert.ToDateTime(date);
                    }
                    dt_vues.Rows.Add(dr_vue);
                    da_vues.Update(dt_vues);
            }
            else
            {
                if (isValid)
                {
                    if (phase.Equals(3))
                    {
                        if (!dr_vues[0]["status"].Equals(3))
                        {
                            nombre_document_valider++;
                            if (dr_vues[0]["status"].Equals(4))
                                if (nombre_document_rejet > 0)
                                    nombre_document_rejet--;
                            dr_vues[0]["status"] = 3;
                        }
                    }
                    else if (phase.Equals(1))
                    {
                        if (!dr_vues[0]["status"].Equals(3))
                        {
                            nombre_document_valider++;
                            if (dr_vues[0]["status"].Equals(4))
                                if (nombre_document_rejet > 0)
                                    nombre_document_rejet--;
                            dr_vues[0][3] = 3;
                        }
                        if (dr_vues[0]["status"].Equals(3))
                        {
                            if (dtformalite != null)
                            {
                                if (!dtformalite.Rows.Count.Equals(0) && type_dossier.Equals("DF"))
                                {
                                    nombre_document_rejet++;
                                    if (dr_vues[0]["status"].Equals(3))
                                        if (nombre_document_valider > 0)
                                            nombre_document_valider--;
                                    dr_vues[0]["status"] = 4;
                                }
                            }
                            if (dtpiece != null)
                            {
                                if (!dtpiece.Rows.Count.Equals(0) && type_dossier.Equals("SDF"))
                                {
                                    nombre_document_rejet++;
                                    if (dr_vues[0]["status"].Equals(3))
                                        if (nombre_document_valider > 0)
                                            nombre_document_valider--;
                                    dr_vues[0]["status"] = 4;
                                }
                            }
                        }

                    }
                    else if (phase.Equals(2))
                    {
                        if (!dr_vues[0]["status"].Equals(5))
                        {
                            nombre_document_corriger++;
                            if (dr_vues[0]["status"].Equals(4))
                                if (nombre_document_rejet > 0)
                                    nombre_document_rejet--;
                            dr_vues[0]["status"] = 5;
                        }
                    }   
                }
                else
                {
                    if (phase.Equals(2))
                    {
                        if (!dr_vues[0]["status"].Equals(5))
                        {
                            nombre_document_corriger++;
                            if (dr_vues[0]["status"].Equals(4))
                                if (nombre_document_rejet > 0)
                                    nombre_document_rejet--;
                            dr_vues[0]["status"] = 5;
                        }
                        //supprimer
                        
                            if (cb_suppri_cr.Checked)
                            {
                               // nombre_document_corriger++;
                                //if (nombre_document_rejet > 0)
                                    //nombre_document_rejet--;
                                dr_vues[0]["status"] = 6;
                            }
                       
                        //

                    }
                    else if (phase.Equals(3))
                    {
                        if (!dr_vues[0]["status"].Equals(4))
                        {
                            nombre_document_rejet++;
                            if(dr_vues[0]["status"].Equals(5))
                                if (nombre_document_corriger > 0)
                                    nombre_document_corriger--;
                            if (dr_vues[0]["status"].Equals(3))
                                if (nombre_document_valider > 0)
                                    nombre_document_valider--;
                            dr_vues[0]["status"] = 4;
                        }
                    }
                    else
                    {
                        if (!dr_vues[0]["status"].Equals(4))
                        {
                            nombre_document_rejet++;
                            if (dr_vues[0]["status"].Equals(3))
                                if (nombre_document_valider > 0)
                                    nombre_document_valider--;
                            dr_vues[0]["status"] = 4;
                        }
                    }
                }

                // Dossier
                dr_vues[0]["nature_orgine"] = ck_nature_orgine_rjt.Checked;
                dr_vues[0]["numero_orgine"] = ck_numero_orgine_rjt.Checked;
                dr_vues[0]["indice_orgine"] = ck_indice_orgine_rjt.Checked;
                dr_vues[0]["indice_special_orgine"] = ck_indice_special_orgine_rjt.Checked;
                dr_vues[0]["numero_titre"] = ck_numero_titre_rjt.Checked;
                dr_vues[0]["indice_titre"] = ck_indice_titre_rjt.Checked;
                dr_vues[0]["indice_speciale_titre"] = ck_indice_speciale_titre_cr.Checked;

                //sous dossier
                dr_vues[0]["numero_sd"] = ck__numero_sd_rjt.Checked;
                dr_vues[0]["formalite"] = ck_formalite_rjt.Checked;
                dr_vues[0]["volume_depot"] = ck_volume_depot_rjt.Checked;
                dr_vues[0]["numero_depot"] = ck_numero_depot_rjt.Checked;
                dr_vues[0]["date_depot"] = ck_date_depot_rjt.Checked;

                // Pièce
                dr_vues[0]["nom_piece"] = ck_nom_piece_rjt.Checked;
                dr_vues[0]["nombre_page"] = ck_nombre_page_rjt.Checked;
                dr_vues[0]["num_page"] = ck_num_page_rjt.Checked;

                dr_vues[0]["qualite_image"] = ck_qlt_image_rjt.Checked;
                dr_vues[0]["taille_image"] = ck_taille_image_rjt.Checked;

                dr_vues[0]["mal_classe"] = ck_mal_classe_rjt.Checked;
                dr_vues[0]["A_SUPPRIMER"] = ck_a_supprimer_rjt.Checked;

                //Dossier corriger
                dr_vues[0]["nature_orgine_cr"] = ck_nature_orgine_cr.Checked;
                dr_vues[0]["numero_orgine_cr"] = ck_numero_orgine_cr.Checked;
                dr_vues[0]["indice_orgine_cr"] = ck_indice_orgine_cr.Checked;
                dr_vues[0]["indice_special_orgine_cr"] = ck_indice_special_orgine_cr.Checked;
                dr_vues[0]["numero_titre_cr"] = ck_numero_titre_cr.Checked;
                dr_vues[0]["indice_titre_cr"] = ck_indice_titre_cr.Checked;
                dr_vues[0]["indice_speciale_titre_cr"] = ck_indice_speciale_titre_cr.Checked;

                // Sous Dossier Corriger
                dr_vues[0]["numero_sd_cr"] = ck__numero_sd_cr.Checked;
                dr_vues[0]["formalite_cr"] = ck_formalite_cr.Checked;
                dr_vues[0]["volume_depot_cr"] = ck_volume_depot_cr.Checked;
                dr_vues[0]["numero_depot_cr"] = ck_numero_depot_cr.Checked;
                dr_vues[0]["date_depot_cr"] = ck_date_depot_cr.Checked;

                //pièce
                dr_vues[0]["nom_piece_cr"] = ck_nom_piece_cr.Checked;
                dr_vues[0]["nombre_page_cr"] = ck_nombre_page_cr.Checked;
                dr_vues[0]["num_page_cr"] = ck_num_page_cr.Checked;

                dr_vues[0]["qualite_image_cr"] = ck_qlt_image_cr.Checked;
                dr_vues[0]["taille_image_cr"] = ck_taille_image_cr.Checked;
                dr_vues[0]["mal_classe_cr"] = ck_mal_classe_cr.Checked;
                dr_vues[0]["img_orgine_illisible_cr"] = ck_qlt_img_org_illisible_cr.Checked;
                dr_vues[0]["sd_manquent_cr"] = ck_sd_manquent_cr.Checked;
                dr_vues[0]["piece_manquent_cr"] = ck_piece_manquent_cr.Checked;
                dr_vues[0]["A_SUPPRIMER"] = ck_a_supprimer_rjt.Checked;

                if (phase.Equals(1))
                {
                    dr_vues[0]["user_controle"] = id_user;
                }
                else if (phase.Equals(2))
                {
                    dr_vues[0]["user_corriger"] = id_user;
                }
                else if (phase.Equals(3))
                {
                    dr_vues[0]["user_controle_corriger"] = id_user;
                }

                

                if (phase.Equals(1))
                {
                    dr_vues[0]["date_controle"] = DateTime.Now;//Convert.ToDateTime(date);
                }
                else if (phase.Equals(2))
                {
                    dr_vues[0]["date_corriger"] = DateTime.Now;//Convert.ToDateTime(date);
                }
                else if (phase.Equals(3))
                {
                    dr_vues[0]["date_controle_corriger"] = DateTime.Now;//Convert.ToDateTime(date);
                }
                da_vues.Update(dt_vues);
                
            }
        }

        private void ck_valider_vue_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_valider_vue.Checked)
            {
                intialiserControle(radGroupBoxPiece_rjt, "1");
                intialiserControle(radGroupBoxDossier, "1");
                intialiserControle(radGroupBoxSousDossier, "1");
                if (type_dossier.Equals("DF"))
                {
                    if (dtformalite != null)
                        for (int i = 0; i < dtformalite.Rows.Count; i++)
                        {
                            dtformalite.Rows[i][3] = 1;
                        }
                }
                   
                if (type_dossier.Equals("SDF"))
                {
                    if (dtpiece != null)
                        for (int i = 0; i < dtpiece.Rows.Count; i++)
                        {
                            dtpiece.Rows[i][4] = 1;
                        }
                }
                    
            }
            else
            {
                //Dossier

                ck_nature_orgine_rjt.Checked = ((int)dr_vues_index[0]["nature_orgine"] == 0) ? false : true;
                ck_numero_orgine_rjt.Checked = ((int)dr_vues_index[0]["numero_orgine"] == 0) ? false : true;
                ck_indice_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_orgine"] == 0) ? false : true;
                ck_indice_special_orgine_rjt.Checked = ((int)dr_vues_index[0]["indice_special_orgine"] == 0) ? false : true;
                ck_numero_titre_rjt.Checked = ((int)dr_vues_index[0]["numero_titre"] == 0) ? false : true;
                ck_indice_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_titre"] == 0) ? false : true;
                ck_indice_speciale_titre_rjt.Checked = ((int)dr_vues_index[0]["indice_speciale_titre"] == 0) ? false : true;


                //Sous dossier
                ck__numero_sd_rjt.Checked = ((int)dr_vues_index[0]["numero_sd"] == 0) ? false : true;
                ck_formalite_rjt.Checked = ((int)dr_vues_index[0]["formalite"] == 0) ? false : true;
                ck_volume_depot_rjt.Checked = ((int)dr_vues_index[0]["volume_depot"] == 0) ? false : true;
                ck_numero_depot_rjt.Checked = ((int)dr_vues_index[0]["numero_depot"] == 0) ? false : true;
                ck_date_depot_rjt.Checked = ((int)dr_vues_index[0]["date_depot"] == 0) ? false : true;

                
                //pièce
                ck_nom_piece_rjt.Checked = ((int)dr_vues_index[0]["nom_piece"] == 0) ? false : true; ;
                ck_nombre_page_rjt.Checked = ((int)dr_vues_index[0]["nombre_page"] == 0) ? false : true;
                ck_num_page_rjt.Checked = ((int)dr_vues_index[0]["num_page"] == 0) ? false : true;

                //pièce
                ck_qlt_image_rjt.Checked = ((int)dr_vues_index[0]["qualite_image"] == 0) ? false : true;
                ck_taille_image_rjt.Checked = ((int)dr_vues_index[0]["taille_image"] == 0) ? false : true;

                ck_mal_classe_rjt.Checked = ((int)dr_vues_index[0]["mal_classe"] == 0) ? false : true;
                ck_a_supprimer_rjt.Checked = ((int)dr_vues_index[0]["A_SUPPRIMER"] == 0) ? false : true;

                if (dtformalite != null)
                    for (int i = 0; i < dtformalite.Rows.Count; i++)
                    {
                        dtformalite.Rows[i][3] = 0;
                    }
               
                if (dtpiece != null)
                    for (int i = 0; i < dtpiece.Rows.Count; i++)
                    {
                        dtpiece.Rows[i][4] = 0;
                    }
            }
        }

        private void radMenuItem9_Click(object sender, EventArgs e)
        {
            deconnexion = true;
            if (dt_dossier != null)
                da_dossier.Update(dt_dossier);
            if (dt_vues != null)
                da_vues.Update(dt_vues);
            Authentification authentifcation = new Authentification();
            authentifcation.Show();
            this.Close();
        }

        public bool isReserver(int id_user,long id_dossier)
        {
            int nbr_dossier = service_data.getValue("select count(*) from TB_DossierF where id_DF in (" + id_dossier + ") and idUser not in ("+id_user+")");
            if (nbr_dossier.Equals(0))
            {
                return false;
            }
            else
                return true;
        }

        public bool isToutesCorriger()
        {
            int nomberCKRejet=0, nombreCKCorriger=0;

            if (type_dossier.Equals("DF"))
            {
                if (dtformalite != null)
                {
                    if (!dtformalite.Rows.Count.Equals(0))
                        nomberCKRejet++;
                }
            }
            if (type_dossier.Equals("SDF"))
            {
                if (dtpiece != null)
                {
                    if (!dtpiece.Rows.Count.Equals(0))
                        nomberCKRejet++;
                }
            }


            foreach (Control c in this.toolWindow2.Controls)
            {
                if (c.GetType() == typeof(Telerik.WinControls.UI.RadGroupBox))
                {
                    foreach (Control c1 in c.Controls)
                    {
                        if (c1.GetType() == typeof(System.Windows.Forms.CheckBox))
                        {
                                if (((CheckBox)c1).Name.Contains("_rjt"))
                                {
                                    if(((CheckBox)c1).Checked)
                                    {
                                        nomberCKRejet++;
                                    }
                                }
                                if (((CheckBox)c1).Name.Contains("_cr"))
                                {
                                    if(((CheckBox)c1).Checked)
                                    {
                                        nombreCKCorriger++;
                                    }
                                }
                           }
                        }
                    }
                }

            if(nombreCKCorriger >= nomberCKRejet)
            {
                return true;
            }
            else
                return false;
        }

        public void historiserDossier(int phase_dossier,int user_action,int status_dossier)
        {
            int id_historique = 1;
            DataRow[] old_dr_dossier_historique = dt_dossier_historique.Select("id_DF =" + idDossier);
            //maria
            SqlCommand cmd = new SqlCommand("select GETDATE()", con);
            con.Open();
            string date = cmd.ExecuteScalar().ToString();
            con.Close();
            if (old_dr_dossier_historique.Count().Equals(0) )
            {
                DataRow new_dr_dossier_historique = dt_dossier_historique.NewRow();
                new_dr_dossier_historique["id_DF"] = idDossier;
                new_dr_dossier_historique["id_unite"] = id_unite;
                new_dr_dossier_historique["status"] = status_dossier;
                new_dr_dossier_historique["phase"] = phase_dossier;
                new_dr_dossier_historique["date_action"] = DateTime.Now;
                //new_dr_dossier_historique["date_action"] = Convert.ToDateTime(date);
                new_dr_dossier_historique["user_action"] = user_action;
                new_dr_dossier_historique["id_historique"] = id_historique;
                dt_dossier_historique.Rows.Add(new_dr_dossier_historique);
                da_dossier_historique.Update(dt_dossier_historique);
                historiserVue(phase_dossier, user_action, id_historique);
            }
            else
            {
                id_historique = (int)old_dr_dossier_historique[0]["id_historique"];
                DataRow new_dr_dossier_historique = dt_dossier_historique.NewRow();
                new_dr_dossier_historique["id_DF"] = idDossier;
                new_dr_dossier_historique["id_unite"] = id_unite;
                new_dr_dossier_historique["status"] = status_dossier;
                new_dr_dossier_historique["phase"] = phase_dossier;
                new_dr_dossier_historique["date_action"] = DateTime.Now;
                //new_dr_dossier_historique["date_action"] = Convert.ToDateTime(date);
                new_dr_dossier_historique["user_action"] = user_action;
                new_dr_dossier_historique["id_historique"] = id_historique + 1;
                dt_dossier_historique.Rows.Add(new_dr_dossier_historique);
                da_dossier_historique.Update(dt_dossier_historique);
                historiserVue(phase_dossier, user_action, id_historique+1);
            }
        }

        public void historiserVue(int phase_vue, int user_action,int id_historique)
        {
            DataTable dt_vues_a_hist = new DataTable();
            SqlDataAdapter da_vues_a_hist;
            SqlCommandBuilder cmdBuilder_vues_a_hist;
            dt_vues_historique = new DataTable();
            da_vues_historique = new SqlDataAdapter("select * from TB_historique_Vues where id_DF=" + idDossier, service_data.getConnextion());
            cmdBuilder_historique_vues = new SqlCommandBuilder(da_vues_historique);
            da_vues_historique.Fill(dt_vues_historique);
            if (phase_vue.Equals(1))
            {
                da_vues_a_hist = new SqlDataAdapter("select * from TB_Vues where id_DF=" + idDossier, service_data.getConnextion());
                cmdBuilder_vues_a_hist = new SqlCommandBuilder(da_vues_a_hist);
                da_vues_a_hist.Fill(dt_vues_a_hist);
            }
            else if (phase_vue.Equals(2))
            {
                da_vues_a_hist = new SqlDataAdapter("select * from TB_Vues where id_DF=" + idDossier + " and status=5", service_data.getConnextion());
                cmdBuilder_vues_a_hist = new SqlCommandBuilder(da_vues_a_hist);
                da_vues_a_hist.Fill(dt_vues_a_hist);
            }
            else if (phase_vue.Equals(3))
            {
                da_vues_a_hist = new SqlDataAdapter("select * from TB_Vues  where id_vues in (select id_vues from TB_historique_Vues where id_DF=" + idDossier + " and phase=2) and status in (3,4)", service_data.getConnextion());
                cmdBuilder_vues_a_hist = new SqlCommandBuilder(da_vues_a_hist);
                da_vues_a_hist.Fill(dt_vues_a_hist);
            }

            foreach (DataRow dr in dt_vues_a_hist.Rows)
            {
                DataRow dr_vue_historique = dt_vues_historique.NewRow();
                dr_vue_historique["id_vues"] = int.Parse(dr["id_vues"].ToString());
                dr_vue_historique["id_DF"] = int.Parse(dr["id_DF"].ToString());
                if (dr["id_SDF"] != null)
                {
                    dr_vue_historique["id_SDF"] = dr["id_SDF"];
                }
                dr_vue_historique["status"] = int.Parse(dr["status"].ToString()); 
                // Dossier
                dr_vue_historique["nature_orgine"] = int.Parse(dr["nature_orgine"].ToString());
                dr_vue_historique["numero_orgine"] = int.Parse(dr["numero_orgine"].ToString());
                dr_vue_historique["indice_orgine"] = int.Parse(dr["indice_orgine"].ToString());
                dr_vue_historique["indice_special_orgine"] = int.Parse(dr["indice_special_orgine"].ToString());
                dr_vue_historique["numero_titre"] = int.Parse(dr["numero_titre"].ToString());
                dr_vue_historique["indice_titre"] = int.Parse(dr["indice_titre"].ToString());
                dr_vue_historique["indice_speciale_titre"] = int.Parse(dr["indice_speciale_titre"].ToString());
                // sous dossier
                dr_vue_historique["numero_sd"] = int.Parse(dr["numero_sd"].ToString());
                dr_vue_historique["formalite"] = int.Parse(dr["formalite"].ToString());
                dr_vue_historique["volume_depot"] = int.Parse(dr["volume_depot"].ToString());
                dr_vue_historique["numero_depot"] = int.Parse(dr["numero_depot"].ToString());
                dr_vue_historique["date_depot"] = int.Parse(dr["date_depot"].ToString());
                // Pièce
                dr_vue_historique["nom_piece"] = int.Parse(dr["nom_piece"].ToString());
                dr_vue_historique["nombre_page"] = int.Parse(dr["nombre_page"].ToString());
                dr_vue_historique["num_page"] = int.Parse(dr["num_page"].ToString());
                dr_vue_historique["qualite_image"] = int.Parse(dr["qualite_image"].ToString());
                dr_vue_historique["taille_image"] = int.Parse(dr["taille_image"].ToString());
                dr_vue_historique["mal_classe"] = int.Parse(dr["mal_classe"].ToString());
                dr_vue_historique["A_SUPPRIMER"] = int.Parse(dr["A_SUPPRIMER"].ToString());
                // Dossier corriger
                dr_vue_historique["nature_orgine_cr"] = int.Parse(dr["nature_orgine_cr"].ToString());
                dr_vue_historique["numero_orgine_cr"] = int.Parse(dr["numero_orgine_cr"].ToString());
                dr_vue_historique["indice_orgine_cr"] = int.Parse(dr["indice_orgine_cr"].ToString());
                dr_vue_historique["indice_special_orgine_cr"] = int.Parse(dr["indice_special_orgine_cr"].ToString());
                dr_vue_historique["numero_titre_cr"] = int.Parse(dr["numero_titre_cr"].ToString());
                dr_vue_historique["indice_titre_cr"] = int.Parse(dr["indice_titre_cr"].ToString());
                dr_vue_historique["indice_speciale_titre_cr"] = int.Parse(dr["indice_speciale_titre_cr"].ToString());
                // Sous Dossier Corriger
                dr_vue_historique["numero_sd_cr"] = int.Parse(dr["numero_sd_cr"].ToString());
                dr_vue_historique["formalite_cr"] = int.Parse(dr["formalite_cr"].ToString());
                dr_vue_historique["volume_depot_cr"] = int.Parse(dr["volume_depot_cr"].ToString());
                dr_vue_historique["numero_depot_cr"] = int.Parse(dr["numero_depot_cr"].ToString());
                dr_vue_historique["date_depot_cr"] = int.Parse(dr["date_depot_cr"].ToString());
                // pièce
                dr_vue_historique["nom_piece_cr"] = int.Parse(dr["nom_piece_cr"].ToString());
                dr_vue_historique["nombre_page_cr"] = int.Parse(dr["nombre_page_cr"].ToString());
                dr_vue_historique["num_page_cr"] = int.Parse(dr["num_page_cr"].ToString());
                dr_vue_historique["qualite_image_cr"] = int.Parse(dr["qualite_image_cr"].ToString());
                dr_vue_historique["taille_image_cr"] = int.Parse(dr["taille_image_cr"].ToString());
                dr_vue_historique["mal_classe_cr"] = int.Parse(dr["mal_classe_cr"].ToString());
                dr_vue_historique["img_orgine_illisible_cr"] = int.Parse(dr["img_orgine_illisible_cr"].ToString());
                dr_vue_historique["sd_manquent_cr"] = int.Parse(dr["sd_manquent_cr"].ToString());
                dr_vue_historique["piece_manquent_cr"] = int.Parse(dr["piece_manquent_cr"].ToString());
                dr_vue_historique["A_SUPPRIMER"] = int.Parse(dr["A_SUPPRIMER"].ToString());
                dr_vue_historique["dateaction"] = DateTime.Now;
                dr_vue_historique["phase"] = phase_vue;
                if (phase_vue.Equals(1))
                {
                    dr_vue_historique["user_controle"] = int.Parse(dr["user_controle"].ToString());
                    //dr_vue_historique["dateaction"] = DateTime.Parse(dr["date_controle"].ToString());
                   
                }
                else if (phase_vue.Equals(2))
                {
                    dr_vue_historique["user_corriger"] = int.Parse(dr["user_corriger"].ToString());
                    //dr_vue_historique["dateaction"] = DateTime.Parse(dr["date_corriger"].ToString());
                }
                else if (phase_vue.Equals(3))
                {
                    dr_vue_historique["user_controle_corriger"] = int.Parse(dr["user_controle_corriger"].ToString());
                    //dr_vue_historique["dateaction"] = DateTime.Parse(dr["date_controle_corriger"].ToString());
                }
                dr_vue_historique["id_historique"] = id_historique;
                dt_vues_historique.Rows.Add(dr_vue_historique);
            }
            da_vues_historique.Update(dt_vues_historique);   
        }

        private void btnPageSuivant_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Traitement_Controle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnPageSuivant_Click(sender, e);
            }
        }

        private void radMenuItem5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Changer l'unité
        private void radMenuItem10_Click(object sender, EventArgs e)
        {
            choix_unite ch_u = new choix_unite();
            ch_u.authToken = this.authToken;
            ch_u.user = this.user;
            ch_u.id_user = this.id_user;
            ch_u.phase = this.phase;
            ch_u.id_tranche = this.id_tranche;
            ch_u.Show();
            this.Hide();
        }

        //Etat
        private void menu_etat_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringGED"].ConnectionString);
            conn.Open();
            String date = DateTime.Now.ToString("yyyy-MM-dd");
            SqlCommand sc = new SqlCommand("SELECT COUNT(distinct id_vues) as nbr_vues FROM dbo.TB_historique_Vues WHERE dateaction='" + date + "' and  (user_controle=" + id_user + " or user_corriger=" + id_user + " or user_controle_corriger=" + id_user + ")", conn);
            sc.CommandType = CommandType.Text;
            sc.Connection = conn;
            using (SqlDataReader rdr = sc.ExecuteReader())
            {
                while (rdr.Read())
                {
                    nbrVue_trait = (int)rdr["nbr_vues"];
                }
            }
            MessageBox.Show("Le nombre de vues traitées : " + nbrVue_trait, date, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //recherche
        private void recherche_Click(object sender, EventArgs e)
        {
            recherche r = new recherche();
            r.control = this;
            r.Show();
        }

        //Chargement de l'arborescence en cas de recherche
        public void loadTree_recherche(int id_Dos) 
        {
            tree_unite_dossier.Nodes.Clear();
            menuAction.Items[0].Enabled = true;

            Node dos = service_ged.get_Node(this.authToken, id_Dos);
            Node unite = service_ged.get_Node(this.authToken, dos.ParentID);
            Node tranche = service_ged.get_Node(this.authToken, unite.ParentID);
            //tranche
            RadTreeNode tranche_tree = new RadTreeNode();
            tranche_tree.Value = tranche.ID;
            tranche_tree.Name = tranche.Name;
            tranche_tree.Text = tranche.Name;
            tranche_tree.Image = Resources.tranche;
            this.tree_unite_dossier.Nodes.Add(tranche_tree);
            //unite
            RadTreeNode unite_tree = new RadTreeNode();
            unite_tree.Value = unite.ID;
            unite_tree.Name = unite.Name;
            unite_tree.Text = String.Format("{0}", unite.Name);
            unite_tree.Image = Resources.unite;
            unite_tree.ContextMenu = contextMenuUnite;
            this.tree_unite_dossier.Nodes[0].Nodes.Add(unite_tree);
            this.tree_unite_dossier.Nodes[0].Expand();
            //dossier
            RadTreeNode Dossier_tree = new RadTreeNode();
            Dossier_tree.Value = dos.ID;
            Dossier_tree.Name = dos.Name;
            Dossier_tree.Text = String.Format("{0}", dos.Name);
            Dossier_tree.Image = Resources.folder;
            this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes.Add(Dossier_tree);
            this.tree_unite_dossier.Nodes[0].Nodes[0].Expand();
            
            ph = service_data.getValue("select phase from dbo.TB_DossierF where id_DF=" + dos.ID);
            if (ph != phase)
            {
                if (phase == 1 && ph == 0)
                {
                    btnPageSuivant.Enabled = true;
                    this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[0].ContextMenu = menuAction;
                }
                else 
                {
                    isPhase = false;
                radGroupBoxPiece_rjt.Enabled = false;
                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[0].ContextMenu = menuVisual;
                menuVisual.Items[0].Enabled = true;
                }
            }
            else 
            {
                btnPageSuivant.Enabled = true;
                this.tree_unite_dossier.Nodes[0].Nodes[0].Nodes[0].ContextMenu = menuAction;
            }
        }

        //visualiser
        private void radMenuItem11_Click(object sender, EventArgs e) 
        {
            
            if (this.tree_unite_dossier.SelectedNode != null)
            {
                try
                {
                        tree_unite_dossier.SelectedNode.Expanded = false;
                        tree_unite_dossier.Enabled = false;
                        treeDossier = tree_unite_dossier.SelectedNode;
                        idDossier = (long)treeDossier.Value;
                        menuVisual.Items[0].Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                try
                {
                    treePieces = new List<RadTreeNode>();
                    treeSousDossiers = new List<RadTreeNode>();
                    DataRow[] drvues;
                    RadTreeNode treeDossier = new RadTreeNode();
                    dt_vues = new DataTable();
                    da_vues = new SqlDataAdapter("select * from TB_Vues where id_DF=" + idDossier, service_data.getConnextion());
                    cmdBuilder_vues = new SqlCommandBuilder(da_vues);
                    nombre_document_charge = 0;
                    nombre_document_corriger = 0;
                    nombre_document_rejet = 0;
                    nombre_document_valider = 0;

                    /*Partie API CLM*/
                    int i = 0;
                    nodes = service_ged.get_List_Nodes(this.authToken, idDossier);
                    da_vues.Fill(dt_vues);
                    if (nodes != null)
                    {
                        var documents = nodes.Where(item => item.Type.Equals("Document"));
                        var _DF_PG = documents.Where(x => x.Name.Contains(ConfigurationManager.AppSettings["PGD"]));
                        var _docTri = documents.OrderBy(x => x.Name);
                        var folders = nodes.Where(item => item.Type.Equals("Folder"));
                        var _nodesTri = folders.OrderBy(x => x.Name);
                        if (_DF_PG != null)
                        {
                            if (_DF_PG.Count() > 0)
                            {
                                /*Page de gard du dossier*/
                                foreach (Node document in _DF_PG)
                                {
                                    treePiece = new RadTreeNode();
                                    treePiece.Name = document.Name;
                                    treePiece.Text = document.Name;
                                    treePiece.Value = document.ID;
                                    drvues = dt_vues.Select("id_vues=" + document.ID);
                                    if (drvues.Count().Equals(0))
                                    {
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treePieces.Add(treePiece);
                                    }
                                    else
                                    {
                                            if (drvues[0][3].Equals(3))
                                            {
                                                nombre_document_valider++;
                                                treePiece.Image = Resources.document_valider;
                                            }
                                            else if (drvues[0][3].Equals(4))
                                            {
                                                nombre_document_rejet++;
                                                treePiece.Image = Resources.document_rejeter;
                                            }
                                            else if (drvues[0][3].Equals(5))
                                            {
                                                nombre_document_corriger++;
                                                treePiece.Image = Resources.document_corriger;
                                            }
                                            else
                                            {
                                                treePiece.Image = Resources.document;
                                            }
                                             nombre_document_charge++;
                                            treePieces.Add(treePiece);
                                    }
                                    i++;
                                }
                            }
                        }

                        /*Fiche de contrôle */
                        if (_docTri != null)
                        {
                            foreach (Node document in _docTri)
                            {
                                if (!document.Name.Contains(ConfigurationManager.AppSettings["PGD"]))
                                {
                                    treePiece = new RadTreeNode();
                                    treePiece.Name = document.Name;
                                    treePiece.Text = document.Name;
                                    treePiece.Value = document.ID;
                                    drvues = dt_vues.Select("id_vues=" + document.ID);
                                    if (drvues.Count().Equals(0))
                                    {
                                          nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treePieces.Add(treePiece);
                                    }
                                    else
                                    {
                                        if (drvues[0][3].Equals(3))
                                        {
                                            nombre_document_valider++;
                                            treePiece.Image = Resources.document_valider;
                                        }
                                        else if (drvues[0][3].Equals(4))
                                        {
                                            nombre_document_rejet++;
                                            treePiece.Image = Resources.document_rejeter;
                                        }
                                        else if (drvues[0][3].Equals(5))
                                        {
                                            nombre_document_corriger++;
                                            treePiece.Image = Resources.document_corriger;
                                        }
                                        else
                                        {
                                            treePiece.Image = Resources.document;
                                        }
                                        nombre_document_charge++;
                                        treePieces.Add(treePiece);
                                    }
                                    i++;
                                }
                                }
                            }
                        /* Sous Dossier */
                        i = 0;
                        foreach (Node sub_folder in _nodesTri)
                        {
                            treeSousDossier = new RadTreeNode();
                            treeSousDossier.Name = sub_folder.Name;
                            treeSousDossier.Text = sub_folder.Name;
                            treeSousDossier.Value = sub_folder.ID;
                            treeSousDossier.Image = Resources.folder_open;
                            nodes = service_ged.get_List_Nodes(this.authToken, sub_folder.ID);
                            if (nodes != null)
                            {
                                var _SD_doc_Tri = nodes.OrderBy(x => x.Name);
                                var _SD_PG = _SD_doc_Tri.Where(x => x.Name.Contains(ConfigurationManager.AppSettings["PGSD"]));
                                /*Page de garde du sous dossier*/
                                if (_SD_PG != null)
                                {
                                    foreach (Node document in _SD_PG)
                                    {
                                        treePiece = new RadTreeNode();
                                        treePiece.Name = document.Name;
                                        treePiece.Text = document.Name;
                                        treePiece.Value = document.ID;
                                        drvues = dt_vues.Select("id_vues=" + document.ID);
                                        if (drvues.Count().Equals(0))
                                        {
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treeSousDossier.Nodes.Add(treePiece);   
                                        }
                                        else
                                        {
                                                if (drvues[0][3].Equals(3))
                                                {
                                                    nombre_document_valider++;
                                                    treePiece.Image = Resources.document_valider;
                                                }
                                                else if (drvues[0][3].Equals(4))
                                                {
                                                    nombre_document_rejet++;
                                                    treePiece.Image = Resources.document_rejeter;

                                                }
                                                else if (drvues[0][3].Equals(5))
                                                {
                                                    nombre_document_corriger++;
                                                    nombre_document_charge++;
                                                    treePiece.Image = Resources.document_corriger;
                                                    treeSousDossier.Nodes.Add(treePiece);
                                                }
                                                else
                                                {
                                                    treePiece.Image = Resources.document;
                                                }
                                                nombre_document_charge++;
                                                treeSousDossier.Nodes.Add(treePiece);
                                        }
                                        i++;
                                    }
                                }

                                /*Document du sous dossier*/
                                foreach (Node document in _SD_doc_Tri)
                                {
                                    if (!document.Name.Contains(ConfigurationManager.AppSettings["PGSD"]))
                                    {
                                        treePiece = new RadTreeNode();
                                        Regex regex = new Regex(@".*_[0-9]$");
                                        Match match = regex.Match(document.Name);
                                        if (match.Success)
                                        {
                                            treePiece.Text = document.Name.Substring(0, document.Name.Length - 2) + "_0" + document.Name.Substring(document.Name.IndexOf("_") + 1);
                                            treePiece.Name = document.Name.Substring(0, document.Name.Length - 2) + "_0" + document.Name.Substring(document.Name.IndexOf("_") + 1);
                                        }
                                        else
                                        {
                                            treePiece.Text = document.Name;
                                            treePiece.Name = document.Name;
                                        }
                                        treePiece.Value = document.ID;
                                        drvues = dt_vues.Select("id_vues=" + document.ID);
                                        if (drvues.Count().Equals(0))
                                        {
                                            nombre_document_charge++;
                                            treePiece.Image = Resources.document;
                                            treeSousDossier.Nodes.Add(treePiece);  
                                        }
                                        else
                                        {
                                                if (drvues[0][3].Equals(3))
                                                {
                                                    nombre_document_valider++;
                                                    treePiece.Image = Resources.document_valider;

                                                }
                                                else if (drvues[0][3].Equals(4))
                                                {
                                                    nombre_document_rejet++;
                                                    treePiece.Image = Resources.document_rejeter;

                                                }
                                                else if (drvues[0][3].Equals(5))
                                                {
                                                    nombre_document_corriger++;
                                                    treePiece.Image = Resources.document_corriger;
                                                    treeSousDossier.Nodes.Add(treePiece);
                                                }
                                                else
                                                {
                                                    treePiece.Image = Resources.document;
                                                }
                                                nombre_document_charge++;
                                                treeSousDossier.Nodes.Add(treePiece);

                                            }
                                            
                                        }
                                        treeSousDossier.Nodes.OrderBy(x => x.Text);
                                    }
                            }
                            if (!treeSousDossier.Nodes.Count.Equals(0))
                            {
                                treeSousDossiers.Add(treeSousDossier);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (tree_unite_dossier.SelectedNode.Nodes.Count > 0)
            {
                try
                {
                    bool trouver = false;
                    IEnumerable<RadTreeNode> _nodes = tree_unite_dossier.SelectedNode.Nodes;
                    foreach (RadTreeNode node in _nodes)
                    {
                        foreach (RadTreeNode _node in treePieces)
                        {
                            if (node.Name == _node.Name)
                            {
                                treePieces.Remove(_node);
                                trouver = true;
                                break;
                            }
                        }
                        if (!trouver)
                        {
                            foreach (RadTreeNode _node in treeSousDossiers)
                            {
                                if (node.Name == _node.Name)
                                {
                                    treeSousDossiers.Remove(_node);
                                    break;
                                }
                            }
                        }
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (treePieces != null)
            {
                if (treePieces.Count != 0)
                {
                    this.tree_unite_dossier.SelectedNode.Nodes.AddRange(treePieces);
                }
            }
            if (treeSousDossiers != null)
            {
                if (treeSousDossiers.Count != 0)
                    this.tree_unite_dossier.SelectedNode.Nodes.AddRange(treeSousDossiers);
            }
            if (tree_unite_dossier.SelectedNode.Nodes.Count > 0)
            {
                this.tree_unite_dossier.SelectedNode.Expand();
                
                tree_unite_dossier.SelectedNode.Image = Resources.folder_edit;
            }
            
            if (ph.Equals(2))
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues corrigées : {0}", nombre_document_corriger);
            }
            else if (ph.Equals(3))
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }
            else 
            {
                lblnbr_vue.Text = string.Format("Nombre de vues chargées : {0}", nombre_document_charge);
                lblvue_rejeter.Text = string.Format("Nombre de vues rejetées : {0}", nombre_document_rejet);
                lblnbr_valider.Text = string.Format("Nombre de vues validées : {0}", nombre_document_valider);
            }

            this.tree_unite_dossier.Enabled = true;
            
        }

        private void cb_suppri_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        //05-04-2017
        private Elements getElement()
        {
            Elements elem = new Elements();
            if (txt_nature_orgine.Text != "")
            {
                elem.nature_Orgine = txt_nature_orgine.Text;
            }
            if (txt_numero_orgine.Text != "")
            {
                elem.numero_Orgine = Int32.Parse(txt_numero_orgine.Text);
            }
            if (txt_indice_orgine.Text != "")
            {
                elem.indice_orgine = txt_indice_orgine.Text;
            }
            if (txt_indice_speciale_orgine.Text != "")
            {
                elem.indice_special_orgine = Int32.Parse(txt_indice_speciale_orgine.Text);
            }
            if (txt_numero_titre.Text != "")
            {
                elem.numero_Titre = Int32.Parse(txt_numero_titre.Text);
            }
            if (txt_indice_titre.Text != "")
            {
                elem.indice_titre = txt_indice_titre.Text;
            }
            if (txt_indice_spciale_titre.Text != "")
            {
                elem.indice_special_titre = txt_indice_spciale_titre.Text;
            }
            if (txt_numero_sd.Text != "")
            {
                elem.numero_sousDossier = Int32.Parse(txt_numero_sd.Text);
            }
            if (txt_formalite.Text != "")
            {
                elem.formaliteFR = txt_formalite.Text;

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["GED"].ConnectionString);
                conn.Open();
                SqlCommand sc = new SqlCommand("select [LIBELLE_FORMALITE_ARAB],[CATEGORIE] FROM [GED].[dbo].[LISTE_FORMALITES] where LIBELLE_FORMALITE='" + txt_formalite.Text + "'", conn);

                using (SqlDataReader rdr = sc.ExecuteReader())
                {
                    while (rdr.Read())
                    {

                        elem.categorie = rdr["CATEGORIE"].ToString();
                        elem.formaliteAR = rdr["LIBELLE_FORMALITE_ARAB"].ToString();
                    }
                }

                conn.Close();
            }
            if (txt_volume_depot.Text != "")
            {
                elem.volume_depot = txt_volume_depot.Text;
            }
            if (txt_volume_depot.Text != "")
            {
                elem.numero_depot = txt_numero_depot.Text;
            }
            if (txt_date_depot.Text != "")
            {
                elem.date_depot = DateTime.Parse(txt_date_depot.Text);
            }
            if (txtNompiece.Text != "")
            {
                elem.nom_piece = txtNompiece.Text;
            }
            if (txtnbrpage.Text != "")
            {
                elem.nombre_page = Int32.Parse(txtnbrpage.Text);
            }
            if (txtnumpage.Text != "")
            {
                elem.numero_page = Int32.Parse(txtnumpage.Text);
            }
            return elem;


        }

        //05-04-2017
        public void corriger()
        {
            //set element 
            try
            {
                //Fiche Piece
                if (radGroupBoxPiece_rjt.Enabled == true)
                {
                    if ((txtNompiece.Text != element.nom_piece) || (txtnbrpage.Text != element.nombre_page.ToString()) || (txtnumpage.Text != element.numero_page.ToString()))
                    {
                        Elements elm = getElement();
                        Node nn = service_ged.get_Node(authToken, (long)tree_unite_dossier.SelectedNode.Value);
                        Metadata meta = service_ged.setElementPiece(elm, nn.Metadata);
                        service_ged.set_Node(authToken, nn.ID, meta);
                        intitule_Piece = nn.Name;
                        intitule_Piece = intitule_Piece.Replace(element.nom_piece.ToString(), txtNompiece.Text);
                        intitule_Piece = intitule_Piece.Replace(element.nombre_page.ToString(), txtnbrpage.Text);
                        intitule_Piece = intitule_Piece.Replace(element.numero_page.ToString(), txtnumpage.Text);
                        service_ged.rename(authToken, nn, intitule_Piece);
                        tree_unite_dossier.SelectedNode.Text = intitule_Piece;
                    }
                }
                //Fiche sousDossier
                //if (tree_unite_dossier.SelectedNode.Name.Contains(ConfigurationManager.AppSettings["PGSD"]))
                if (radGroupBoxSousDossier.Enabled == true)
                {

                    if ((!element.formaliteFR.ToString().Equals("") && element.formaliteFR.ToString() != null) && ((txt_numero_sd.Text != element.numero_sousDossier.ToString()) || (txt_formalite.Text != element.formaliteFR) ||
                        (txt_volume_depot.Text != element.volume_depot) || (txt_numero_depot.Text != element.numero_depot) ||
                        (txt_date_depot.Text != element.date_depot.ToString("dd/MM/yyyy"))))
                    {
                        form1.Show();
                        Elements elm = getElement();
                        Node nn = service_ged.get_Node(authToken, (long)tree_unite_dossier.SelectedNode.Value);
                        Node Parentnn = service_ged.get_Node(authToken, nn.ParentID);
                        Metadata meta = service_ged.setElementSD(elm, nn.Metadata);
                        service_ged.set_Node(authToken, nn.ID, meta);
                        Node[] vueSD = service_ged.get_List_Nodes(authToken, (long)tree_unite_dossier.SelectedNode.Parent.Value);
                        foreach (Node v in vueSD)
                        {
                            Node vu = service_ged.get_Node(authToken, v.ID);
                            Metadata m = service_ged.setElementSD(elm, vu.Metadata);
                            service_ged.set_Node(authToken, vu.ID, m);
                        }

                        if (int.Parse(txt_numero_sd.Text) <= 9)
                            intitule_Sous_Dossier = "SD_0" + txt_numero_sd.Text + "_" + txt_formalite.Text;
                        else
                        {
                            intitule_Sous_Dossier = "SD_" + txt_numero_sd.Text + "_" + txt_formalite.Text;
                        }
                        if (txt_volume_depot.Text != "" && txt_volume_depot.Text != "00")
                        {
                            intitule_Sous_Dossier = intitule_Sous_Dossier + "_" + txt_volume_depot.Text;
                        }
                        if (txt_numero_depot.Text != "" && txt_numero_depot.Text != "00")
                        {
                            intitule_Sous_Dossier = intitule_Sous_Dossier + "_" + txt_numero_depot.Text;
                        }

                        service_ged.rename(authToken, Parentnn, intitule_Sous_Dossier);
                        tree_unite_dossier.SelectedNode.Parent.Text = intitule_Sous_Dossier;
                    }
                }
                //Fiche Dossier
                if (tree_unite_dossier.SelectedNode.Name.Contains(ConfigurationManager.AppSettings["PGD"]))
                {


                    if ((txt_nature_orgine.Text != element.nature_Orgine) || (txt_numero_orgine.Text != element.numero_Orgine.ToString()) ||
                        (txt_indice_orgine.Text != element.indice_orgine) || (txt_indice_speciale_orgine.Text != element.indice_special_orgine.ToString()) ||
                        (txt_numero_titre.Text != element.numero_Titre.ToString()) || (txt_indice_titre.Text != element.indice_titre) ||
                        (txt_indice_spciale_titre.Text != element.indice_special_titre))
                    {
                        form1.Show();
                        Elements elm = getElement();
                        Node nn = service_ged.get_Node(authToken, (long)tree_unite_dossier.SelectedNode.Value);
                        Node Parentnn = service_ged.get_Node(authToken, nn.ParentID);
                        Metadata meta = service_ged.setElementDoss(elm, nn.Metadata);
                        service_ged.set_Node(authToken, nn.ID, meta);
                        Node[] elemtD = service_ged.get_List_Nodes(authToken, (long)tree_unite_dossier.SelectedNode.Parent.Value);
                        var sdD = elemtD.Where(item => item.Type.Equals("Folder"));
                        var vueD = elemtD.Where(item => item.Type.Equals("Document"));
                        foreach (Node v in vueD)
                        {
                            Node vu = service_ged.get_Node(authToken, v.ID);
                            Metadata m = service_ged.setElementDoss(elm, vu.Metadata);
                            service_ged.set_Node(authToken, vu.ID, m);
                        }
                        foreach (Node sd in sdD)
                        {
                            Node[] vueSD = service_ged.get_List_Nodes(authToken, sd.ID);
                            foreach (Node v in vueSD)
                            {
                                Node vu = service_ged.get_Node(authToken, v.ID);
                                Metadata m = service_ged.setElementDoss(elm, vu.Metadata);
                                service_ged.set_Node(authToken, vu.ID, m);
                            }
                        }
                        if (txt_numero_titre.Text != "" && txt_indice_titre.Text != "")
                        {
                            intitule_Dossier = "TF_" + txt_numero_titre.Text + "_" + txt_indice_titre.Text;
                            if (txt_indice_spciale_titre.Text != "")
                            {
                                intitule_Dossier = intitule_Dossier + "_" + txt_indice_spciale_titre.Text;
                            }

                        }

                        service_ged.rename(authToken, Parentnn, intitule_Dossier);
                        tree_unite_dossier.SelectedNode.Parent.Text = intitule_Dossier;
                        tree_unite_dossier.SelectedNode.Parent.Name = intitule_Dossier;
                    }
                }
                //
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                label18.Visible = false;
                tree_unite_dossier.Enabled = true;
                Cursor.Current = Cursors.Default;
                form1.Hide();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_formalite.Text = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNompiece.Text = comboBox2.Text;
        }
    }
}
