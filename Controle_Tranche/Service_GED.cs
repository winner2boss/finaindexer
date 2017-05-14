using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controle_Tranche.CWS.Authentication;
using System.ServiceModel;
using Controle_Tranche.CWS.DocumentManagement;
using System.Configuration;
using Controle_Tranche.CWS.ContentService;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Controle_Tranche
{
    class Service_GED
    {
        const int BUFFER_SIZE = 4096;
        public String Connexion(string login, string passe)
        {
            AuthenticationClient authClient = new AuthenticationClient();
            // Store the authentication token
            string authToken = null;
            // Call the AuthenticateUser() method to get an authentication token
            try
            {
                authToken = authClient.AuthenticateUser(login, passe);
                //authClient.ValidateUser("");
            }
            catch (FaultException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Always close the client
                authClient.Close();
            }
            return authToken;
        }

        public long get_ID_Node(string authTocken, long parent_id, string nom_dossier)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuthDocManger = new CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = authTocken;
            Node folder = null;
            try
            {
                folder = docManClient.GetNodeByName(ref otAuthDocManger, parent_id, nom_dossier);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Always close the client
                docManClient.Close();
            }
            if (folder != null)
                return folder.ID;
            else
                return 0;
        }

        public Node get_Node(string authTocken, long node_id) 
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuthDocManger = new CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = authTocken;
            Node node = null;
            try
            {
                node = docManClient.GetNode(ref otAuthDocManger, node_id);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                docManClient.Close();
            }
            return node;
        }

        public Node[] get_List_Nodes(string authTocken, long parent_id)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuthDocManger = new CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = authTocken;
            Node[] nodes = null;
            try
            {
                nodes = docManClient.ListNodes(ref otAuthDocManger, parent_id,true);
            }
            catch (FaultException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                docManClient.Close();
            }
            return nodes;
        }

        public Elements getElement(Elements elements, Metadata metadata)
        {
            if (metadata != null)
            {
                if (metadata.AttributeGroups != null)
                {
                    List<AttributeGroup> listAttributes = metadata.AttributeGroups.ToList<AttributeGroup>();
                    foreach (AttributeGroup attributeGroup in listAttributes)
                    {
                        string nom_cat = attributeGroup.DisplayName;
                        if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_DOSSIER"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Nature de l'origine / طبيعة الاصل")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.nature_Orgine = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }

                                        if (displayName == "Numéro de l'origine / عدد الأصل")
                                        {
                                            if (!((IntegerValue)value).Values[0].Equals(null))
                                                elements.numero_Orgine = ((IntegerValue)value).Values[0].Value;
                                            continue;
                                        }

                                        if (displayName == "Indice de l'origine / مؤشر الأصل")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.indice_orgine = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }

                                        if (displayName == "Numéro titre رقم الرسم العقاري")
                                        {
                                            if (!((IntegerValue)value).Values[0].Equals(null))
                                                elements.numero_Titre = ((IntegerValue)value).Values[0].Value;
                                            continue;
                                        }

                                        if (displayName == "Indice Titre مؤشر الرسم")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.indice_titre = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }

                                        if (displayName == "Indice spéciale de titre / مؤشر خاص بالرسم")
                                        {
                                            if (((StringValue)value).Values != null)
                                                elements.indice_special_titre = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }

                                        if (displayName == "Indice spéciale de l'origine / مؤشر خاص بالأصل")
                                        {
                                            string a = value.GetType().ToString();
                                            if (a.Split('.').Last() == "StringValue")
                                            {
                                                if (((StringValue)value).Values != null)
                                                    elements.indice_special_orgine = Convert.ToInt64(((StringValue)value).Values[0]);
                                            }
                                            else if (a.Split('.').Last() == "IntegerValue")
                                            {
                                                if (!((IntegerValue)value).Values[0].Equals(null))
                                                    elements.indice_special_orgine = ((IntegerValue)value).Values[0].Value;
                                            }
                                            continue;
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                        else if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_SOUS_DOSSIER"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Numéro Sous Dossier / رقم الاجراء")
                                        {
                                            if (!((IntegerValue)value).Values[0].Equals(null))
                                                elements.numero_sousDossier = ((IntegerValue)value).Values[0].Value;
                                            continue;
                                        }
                                        if (displayName == "Catégorie / نوع")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.categorie = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "Formalité (fr)")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.formaliteFR = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "طبيعة الاجراء")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.formaliteAR = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "Volume dépôt / رقم مجلد الايداع")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.volume_depot = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "Numéro Dépôt / رقم الايداع")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.numero_depot = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "Date Dépot / تاريخ الايداع")
                                        {
                                            if (!((DateValue)value).Values[0].Equals(null))
                                                elements.date_depot = ((DateValue)value).Values[0].Value;
                                            continue;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                        else if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_PIECE"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Nom de la pièce / اسم الوثيقة")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.nom_piece = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                        if (displayName == "Nombre de la page / عدد الصفحات")
                                        {
                                            if (!((IntegerValue)value).Values[0].Equals(null))
                                                elements.nombre_page = ((IntegerValue)value).Values[0].Value;
                                            continue;
                                        }
                                        if (displayName == "Numéro de la page / رقم الصفحة")
                                        {
                                            if (!((IntegerValue)value).Values[0].Equals(null))
                                                elements.numero_page = ((IntegerValue)value).Values[0].Value;
                                            continue;
                                        }
                                        if (displayName == "Statut")
                                        {
                                            if (!((StringValue)value).Values.Equals(null))
                                                elements.statut = ((StringValue)value).Values[0].ToString();
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return elements;
        }

        public Stream dowlande(string authToken, string pathe, Node node)
        {
            //Create the DocumentManagement service client
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuth = new CWS.DocumentManagement.OTAuthentication();
            otAuth.AuthenticationToken = authToken;

            ContentServiceClient contentServiceClient = new ContentServiceClient();
            CWS.ContentService.OTAuthentication otAuthConServices = new CWS.ContentService.OTAuthentication();
            otAuthConServices.AuthenticationToken = authToken;
            //string fileName=null;
            string contextID = null;
            try
            {
                contextID = docManClient.GetVersionContentsContext(ref otAuth, node.ID, 0);
            }
            catch (FaultException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                docManClient.Close();
            }

            Stream downloadStream = null;
            try
            {
                downloadStream = contentServiceClient.DownloadContent(ref otAuthConServices, contextID);
            }
            catch (FaultException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                contentServiceClient.Close();
            }
            /*FileStream fileStream = null;
            try
            {
                string path = Path.GetDirectoryName(Application.ExecutablePath).Replace(@"bin\Debug", string.Empty);
                fileName = Path.Combine(path.Replace(@"bin\Debug", string.Empty), @"GED_image\" + node.Name + ".tif");
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                fileStream = new FileStream(fileName, FileMode.Create);
                byte[] buffer = new byte[BUFFER_SIZE];
                long fileSize = 0;
                for (int read = downloadStream.Read(buffer, 0, buffer.Length); read > 0; read = downloadStream.Read(buffer, 0, buffer.Length))
                {
                    fileStream.Write(buffer, 0, read);
                    fileSize += read;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                downloadStream.Close();
            }*/
            return downloadStream;
        }

        public void delete(string authTocken, long node_id) 
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuthDocManger = new CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = authTocken;
            try
            {
                docManClient.DeleteNode(ref otAuthDocManger, node_id);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                docManClient.Close();
            }
        }
        
        //05-04-2017
        public Metadata setElementDoss(Elements elements, Metadata metadata)
        {
            if (metadata != null)
            {
                if (metadata.AttributeGroups != null)
                {
                    List<AttributeGroup> listAttributes = metadata.AttributeGroups.ToList<AttributeGroup>();
                    foreach (AttributeGroup attributeGroup in listAttributes)
                    {
                        string nom_cat = attributeGroup.DisplayName;
                        if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_DOSSIER"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Nature de l'origine / طبيعة الاصل")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.nature_Orgine };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.nature_Orgine;
                                            }
                                                
                                            continue;
                                        }

                                        if (displayName == "Numéro de l'origine / عدد الأصل")
                                        {
                                            if (((IntegerValue)value).Values == null)
                                            {
                                                ((IntegerValue)value).Values = new long?[] { elements.numero_Orgine };
                                            }
                                            else
                                            {
                                                ((IntegerValue)value).Values[0] = elements.numero_Orgine;
                                            }
                                                 
                                            continue;
                                        }

                                        if (displayName == "Indice de l'origine / مؤشر الأصل")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.indice_orgine };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.indice_orgine;
                                            }
                                                
                                            continue;
                                        }

                                        //if (displayName == "Indice spéciale de l'origine / مؤشر خاص بالأصل")
                                        //{
                                        //    if (((StringValue)value).Values == null)
                                        //    {
                                        //        ((IntegerValue)value).Values = new long?[] { elements.indice_special_orgine };
                                        //    }
                                        //    else
                                        //    {
                                        //        ((IntegerValue)value).Values[0] = elements.indice_special_orgine;
                                        //    }
                                              
                                        //    continue;
                                        //}

                                        if (displayName == "Indice spéciale de l'origine / مؤشر خاص بالأصل")
                                        {
                                            string a = value.GetType().ToString();
                                            if (a.Split('.').Last() == "StringValue")
                                            {
                                                
                                                if (((StringValue)value).Values == null)
                                                {
                                                    ((StringValue)value).Values = new string[] { elements.indice_special_orgine.ToString() };
                                                }
                                                else
                                                {
                                                    ((StringValue)value).Values[0] = elements.indice_special_orgine.ToString();
                                                }

                                            }
                                            else if (a.Split('.').Last() == "IntegerValue")
                                            {
                                                if (((StringValue)value).Values == null)
                                                {
                                                    ((IntegerValue)value).Values = new long?[] { elements.indice_special_orgine };
                                                }
                                                else
                                                {
                                                    ((IntegerValue)value).Values[0] = elements.indice_special_orgine;
                                                }
                                            }
                                            continue;
                                        }

                                        if (displayName == "Numéro titre رقم الرسم العقاري")
                                        {
                                            if (((IntegerValue)value).Values == null)
                                            {
                                                ((IntegerValue)value).Values = new long?[] { elements.numero_Titre };
                                            }
                                            else
                                            {
                                                ((IntegerValue)value).Values[0] = elements.numero_Titre;
                                            }
                                            
                                            continue;
                                        }

                                        if (displayName == "Indice Titre مؤشر الرسم")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.indice_titre };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.indice_titre;
                                            }
                                                
                                            continue;
                                        }

                                        if (displayName == "Indice spéciale de titre / مؤشر خاص بالرسم")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.indice_special_titre };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.indice_special_titre;
                                            }
                                               
                                            continue;
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                        
                    }
                }
            }
            return metadata;
        }

        public Metadata setElementSD(Elements elements, Metadata metadata)
        {
            if (metadata != null)
            {
                if (metadata.AttributeGroups != null)
                {
                    List<AttributeGroup> listAttributes = metadata.AttributeGroups.ToList<AttributeGroup>();
                    foreach (AttributeGroup attributeGroup in listAttributes)
                    {
                        string nom_cat = attributeGroup.DisplayName;
                        
                        if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_SOUS_DOSSIER"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Numéro Sous Dossier / رقم الاجراء")
                                        {
                                            if (((IntegerValue)value).Values == null)
                                            {
                                                ((IntegerValue)value).Values = new long?[] { elements.numero_sousDossier };
                                            }
                                            else
                                            {
                                                ((IntegerValue)value).Values[0] = elements.numero_sousDossier;
                                            }
                                            
                                            continue;
                                        }
                                        if (displayName == "Catégorie / نوع")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.categorie };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.categorie;
                                            }
                                               
                                            continue;
                                        }
                                        if (displayName == "Formalité (fr)")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.formaliteFR };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.formaliteFR;
                                            }
                                                 
                                            continue;
                                        }
                                        if (displayName == "طبيعة الاجراء")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.formaliteAR };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.formaliteAR;
                                            }
                                                
                                            continue;
                                        }
                                        if (displayName == "Volume dépôt / رقم مجلد الايداع")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.volume_depot };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.volume_depot;
                                            }
                                            continue;
                                        }
                                        if (displayName == "Numéro Dépôt / رقم الايداع")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.numero_depot };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.numero_depot;
                                            }
                                                 
                                            continue;
                                        }
                                        if (displayName == "Date Dépot / تاريخ الايداع")
                                        {
                                            if (((DateValue)value).Values == null)
                                            {
                                                ((DateValue)value).Values = new DateTime?[] { elements.date_depot };
                                            }
                                            else
                                            {
                                                ((DateValue)value).Values[0] = elements.date_depot;
                                            }
                                                
                                            continue;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                        
                    }
                }
            }
            return metadata;
        }

        public Metadata setElementPiece(Elements elements, Metadata metadata)
        {
            if (metadata != null)
            {
                if (metadata.AttributeGroups != null)
                {
                    List<AttributeGroup> listAttributes = metadata.AttributeGroups.ToList<AttributeGroup>();
                    foreach (AttributeGroup attributeGroup in listAttributes)
                    {
                        string nom_cat = attributeGroup.DisplayName;
                       
                        if (nom_cat.Equals(ConfigurationManager.AppSettings["CAT_PIECE"]))
                        {
                            if (attributeGroup.Values.Length > 0)
                            {
                                DataValue[] dataValues = attributeGroup.Values;
                                try
                                {
                                    foreach (DataValue value in dataValues)
                                    {
                                        string displayName = value.Description;

                                        if (displayName == "Nom de la pièce / اسم الوثيقة")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.nom_piece };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.nom_piece;
                                            }
                                            
                                            continue;
                                        }
                                        if (displayName == "Nombre de la page / عدد الصفحات")
                                        {
                                            if (((IntegerValue)value).Values == null)
                                            {
                                                ((IntegerValue)value).Values = new long?[] { elements.nombre_page };
                                            }
                                            else
                                            {
                                                ((IntegerValue)value).Values[0] = elements.nombre_page;
                                            }
                                            
                                                
                                            continue;
                                        }
                                        if (displayName == "Numéro de la page / رقم الصفحة")
                                        {
                                            if (((IntegerValue)value).Values == null)
                                            {
                                                ((IntegerValue)value).Values = new long?[] { elements.numero_page };
                                            }
                                            else
                                            {
                                                ((IntegerValue)value).Values[0] = elements.numero_page;
                                            }
                                                
                                            continue;
                                        }
                                        if (displayName == "Statut")
                                        {
                                            if (((StringValue)value).Values == null)
                                            {
                                                ((StringValue)value).Values = new string[] { elements.statut };
                                            }
                                            else
                                            {
                                                ((StringValue)value).Values[0] = elements.statut;
                                            }
                                                
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nError Get Value Attribute Courrier: " + ex.Message.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return metadata;
        }

        public  void set_Node(string authTocken, long node_id, Metadata metadata)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            CWS.DocumentManagement.OTAuthentication otAuthDocManger = new CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = authTocken;
            try
            {
                docManClient.SetNodeMetadata(ref otAuthDocManger, node_id, metadata);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                docManClient.Close();
            }

        }

        public void rename(String otAuth, Node node, string name)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();
            Controle_Tranche.CWS.DocumentManagement.OTAuthentication otAuthDocManger = new Controle_Tranche.CWS.DocumentManagement.OTAuthentication();
            otAuthDocManger.AuthenticationToken = otAuth;
            try
            {
                docManClient.RenameNode(ref otAuthDocManger, node.ID, name);
                

            }
            catch (FaultException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);

            }
            finally
            {
                docManClient.Close();
            }
        }

    }
        

    }

