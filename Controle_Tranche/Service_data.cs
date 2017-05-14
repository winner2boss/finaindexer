using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace Controle_Tranche
{
    class Service_data
    {
        public SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringGED"].ConnectionString);
        DataTable dt;
        SqlDataAdapter da;
        SqlCommand cmd;

        public DataTable getData(string requet)
        {
            try
            {
                dt = new DataTable();
                da = new SqlDataAdapter(requet, cn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public void setData(string requet)
        {
            try 
            {
                cmd = new SqlCommand(requet, cn);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        public int getValue(string requet)
        {
            int valeur=0;
            try
            {
                cmd = new SqlCommand(requet, cn);
                cn.Open();
                valeur=int.Parse(cmd.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                //MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return valeur;
        }

        public SqlConnection getConnextion()
        {
            return this.cn;
        }

    }
}
