using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    public enum AZTypeDeChamp { UNDEFINED, ClePrimairePrincipale, ClePrimaire, Texte, Entier, Combobox, Date, Double, Booleen, Guid, Blob };
    public class AZChamp : View
    {
        protected AZTypeDeChamp _type;
        protected string _header;
        protected string _nom_champ;
        protected int _lg_champ;
        protected int _lg_champ_ecran;
        protected string _nom_tab_ref_pour_cbo;
        protected string _base_req;
        protected string _base_filtre_id;
        protected string _base_filtre_lib;
        protected bool _maj;
        protected bool _visible;
        //        public Label lbl;
        public View champ_saisie;

        public AZChamp(AZTypeDeChamp type, string header, string nom_champ, int lg_champ, int lg_champ_ecran, string nom_tab_ref_pour_cbo, string base_req, string base_filtre_id, string base_filtre_lib, bool maj, bool visible)
        {
            _type = type;
            _header = header;
            _nom_champ = nom_champ;
            _lg_champ = lg_champ;
            _lg_champ_ecran = lg_champ_ecran;
            _nom_tab_ref_pour_cbo = nom_tab_ref_pour_cbo;
            _base_req = base_req;
            _base_filtre_lib = base_filtre_lib;
            _base_filtre_id = base_filtre_id;
            _maj = maj;
            _visible = visible;
        }
        public AZTypeDeChamp type { get { return _type; } set { _type = value; } }
        public string header { get { return _header; } set { _header = value; } }
        public string nom_champ { get { return _nom_champ; } set { _nom_champ = value; } }
        public int lg_champ { get { return _lg_champ; } set { _lg_champ = value; } }
        public int lg_champ_ecran { get { return _lg_champ_ecran; } set { _lg_champ_ecran = value; } }
        public string nom_tab_ref_pour_cbo { get { return _nom_tab_ref_pour_cbo; } set { _nom_tab_ref_pour_cbo = value; } }
        public bool maj { get { return _maj; } set { _maj = value; } }
        public bool visible { get { return _visible; } set { _visible = value; } }
        public string base_req { get { return _base_req; } set { _base_req = value; } }
        public string base_filtre_id { get { return _base_filtre_id; } set { _base_filtre_id = value; } }
        public string base_filtre_lib { get { return _base_filtre_lib; } set { _base_filtre_lib = value; } }
        public virtual string NomLbl()
        {
            return "lbl" + nom_champ;
        }
        public virtual string NomChampIHM()
        {
            string nom = nom_champ;
            switch (type)
            {
                case AZTypeDeChamp.Booleen:
                    nom = "chk" + nom_champ;
                    break;
                case AZTypeDeChamp.ClePrimaire:
                case AZTypeDeChamp.ClePrimairePrincipale:
                    nom = nom_champ;
                    break;
                case AZTypeDeChamp.Combobox:
                    nom = "cbo" + nom_champ;
                    break;
                case AZTypeDeChamp.Date:
                    nom = "dtt" + nom_champ;
                    break;
                case AZTypeDeChamp.Double:
                    nom = "dbl" + nom_champ;
                    break;
                case AZTypeDeChamp.Entier:
                    nom = "int" + nom_champ;
                    break;
                case AZTypeDeChamp.Texte:
                    nom = "txt" + nom_champ;
                    break;
                default:
                    throw new Exception("type de champ non traite");
                    //                    break;
            }
            return nom;
        }

        public virtual string NomChampBD()
        {
            return nom_champ;
        }

        public Type TypeCsharp()
        {
            Type t = null;
            switch (type)
            {
                case AZTypeDeChamp.Booleen:
                    t = typeof(bool);
                    break;
                case AZTypeDeChamp.ClePrimaire:
                case AZTypeDeChamp.ClePrimairePrincipale:
                    t = typeof(int);
                    break;
                case AZTypeDeChamp.Combobox:
                    t = typeof(int);
                    break;
                case AZTypeDeChamp.Date:
                    t = typeof(DateTime);
                    break;
                case AZTypeDeChamp.Double:
                    t = typeof(double);
                    break;
                case AZTypeDeChamp.Entier:
                    t = typeof(int);
                    break;
                case AZTypeDeChamp.Texte:
                    t = typeof(string);
                    break;
                default:
                    throw new Exception("type de champ non traite");
                    //                    break;
            }
            return t;
        }

        public virtual string NomFctChangement()
        {
            string t = null;
            switch (type)
            {
                case AZTypeDeChamp.Booleen:
                    t = NomChampBD() + "_Toggled";
                    break;
                case AZTypeDeChamp.ClePrimaire:
                case AZTypeDeChamp.ClePrimairePrincipale:
                    break;
                case AZTypeDeChamp.Combobox:
                    t = NomChampBD() + "_CboIdchanged";
                    break;
                case AZTypeDeChamp.Date:
                    t = NomChampBD() + "_DateChanged";
                    break;
                case AZTypeDeChamp.Double:
                case AZTypeDeChamp.Entier:
                case AZTypeDeChamp.Texte:
                    t = NomChampBD() + "_TextChanged";
                    break;
                default:
                    throw new Exception("type de champ non traite");
                    //                    break;
            }
            return t;
        }

        public string NomFctChangementBis()
        {
            string t = null;
            switch (type)
            {
                case AZTypeDeChamp.Combobox:
                    t = NomChampBD() + "_CboLibchanged";
                    break;
            }
            return t;
        }
    }

    public class AZChampCritere : AZChamp
    {
        private string _clause_sql;

        public AZChampCritere(AZTypeDeChamp type, string header, string nom_champ, int lg_champ, int largeur_champ_ecran, string nom_tab_ref_pour_cbo, string base_req, string base_filtre_id, string base_filtre_lib, bool maj, bool visible, string clause_sql) : base(type, header, nom_champ, lg_champ, largeur_champ_ecran, nom_tab_ref_pour_cbo, base_req, base_filtre_id, base_filtre_lib, maj, visible)
        {
            /*
            _type = type;
            _header = header;
            _nom_champ = nom_champ;
            _lg_champ = lg_champ;
            _lg_champ_ecran = largeur_champ_ecran;
            _nom_tab_ref_pour_cbo = nom_tab_ref_pour_cbo;
            _base_req = base_req;
            _base_filtre_lib = base_filtre_lib;
            _base_filtre_id = base_filtre_id;
            _maj = maj;
            _visible = visible;
            */
            _clause_sql = clause_sql;
        }

        public string clause_sql { get { return _clause_sql; } set { _clause_sql = value; } }
    }
}

