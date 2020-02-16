using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    public enum AZTypeDeChamp { UNDEFINED, ClePrimairePrincipale, ClePrimaire, Texte, Entier, Combobox, Date, Double, Booleen, Guid, Blob };
    public class AZChamp    // : View
    {
        protected AZTypeDeChamp m_type;
        protected string m_header;
        protected string m_nom_champ;
        protected int m_lg_champ;
        protected int m_lg_champ_ecran;
        protected string m_nom_tab_ref_pour_cbo;
        protected string m_base_req;
        protected string m_base_filtre_id;
        protected string m_base_filtre_lib;
        protected bool m_maj;
        protected bool m_visible;
        protected bool m_oblig;
        private AZBlocDonnees m_bloc_donnees;
        //        public Label lbl;
        public View champ_saisie;

        public AZChamp(AZBlocDonnees bloc_donnees, AZTypeDeChamp type, string header, string nom_champ, int lg_champ, int lg_champ_ecran, string nom_tab_ref_pour_cbo, string base_req, string base_filtre_id, string base_filtre_lib, bool maj, bool oblig, bool visible)
        {
            m_bloc_donnees = bloc_donnees;
            m_type = type;
            m_header = header;
            m_nom_champ = nom_champ;
            m_lg_champ = lg_champ;
            m_lg_champ_ecran = lg_champ_ecran;
            m_nom_tab_ref_pour_cbo = nom_tab_ref_pour_cbo;
            m_base_req = base_req;
            m_base_filtre_lib = base_filtre_lib;
            m_base_filtre_id = base_filtre_id;
            m_maj = maj;
            m_oblig = oblig;
            m_visible = visible;
        }
        public AZBlocDonnees bloc_donnees { get { return m_bloc_donnees; } set { m_bloc_donnees = value; } }
        public AZTypeDeChamp type { get { return m_type; } set { m_type = value; } }
        public string header { get { return m_header; } set { m_header = value; } }
        public string nom_champ { get { return m_nom_champ; } set { m_nom_champ = value; } }
        public int lg_champ { get { return m_lg_champ; } set { m_lg_champ = value; } }
        public int lg_champ_ecran { get { return m_lg_champ_ecran; } set { m_lg_champ_ecran = value; } }
        public string nom_tab_ref_pour_cbo { get { return m_nom_tab_ref_pour_cbo; } set { m_nom_tab_ref_pour_cbo = value; } }
        public bool maj { get { return m_maj; } set { m_maj = value; } }
        public bool oblig { get { return m_oblig; } set { m_oblig = value; } }
        public bool visible { get { return m_visible; } set { m_visible = value; } }
        public string base_req { get { return m_base_req; } set { m_base_req = value; } }
        public string base_filtre_id { get { return m_base_filtre_id; } set { m_base_filtre_id = value; } }
        public string base_filtre_lib { get { return m_base_filtre_lib; } set { m_base_filtre_lib = value; } }
        public virtual string NomLbl()
        {
            return "lbl" + nom_champ;
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
    }

    public class AZChampCritere : AZChamp
    {
        private string _clause_sql;

        public AZChampCritere(AZBlocDonnees bloc_donnees, AZTypeDeChamp type, string header, string nom_champ, int lg_champ, int largeur_champ_ecran, string nom_tab_ref_pour_cbo, string base_req, string base_filtre_id, string base_filtre_lib, bool maj, bool visible, string clause_sql) : base(bloc_donnees, type, header, nom_champ, lg_champ, largeur_champ_ecran, nom_tab_ref_pour_cbo, base_req, base_filtre_id, base_filtre_lib, maj, false, visible)
        {
            _clause_sql = clause_sql;
        }
        public string clause_sql { get { return _clause_sql; } set { _clause_sql = value; } }
    }
}
