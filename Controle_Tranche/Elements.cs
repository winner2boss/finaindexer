using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controle_Tranche
{
    class Elements
    {
        public string nature_Orgine { set; get; }
        public long numero_Orgine { set; get; }
        public string indice_orgine { set; get; }
        public long indice_special_orgine { set; get; }
        public long numero_Titre { set; get; }
        public string indice_titre { set; get; }
        public string indice_special_titre { set; get; }
        public string nom_piece { set; get; }
        public long nombre_page { set; get; }
        public long numero_page { set; get; }
        public string statut { set; get; }
        public long numero_sousDossier { set; get; }
        public string categorie { set; get; }
        public string formaliteFR { set; get; }
        public string formaliteAR { set; get; }
        public string volume_depot { set; get; }
        public string numero_depot { set; get; }
        public DateTime date_depot { set; get; }
        public string type_img { set; get; }
        public long size_img { set; get; }
        public string file { set; get; }

        public Elements()
        {
            nature_Orgine = "";
            indice_orgine = "";
            indice_special_titre = "";
            indice_titre = "";
            nom_piece = "";
            formaliteFR = "";
            volume_depot = "";
            numero_depot = "";

        }
    }
}
