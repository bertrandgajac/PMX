using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controles
{
    public enum AZTypeBlocDonnees { UNDEFINED, Formulaire, Grille, CriteresRecherche };
    public enum AZSeparationColonnes { Splitter, BoutonsAuDessus, BoutonsALaFinDesChamps };
    public class AZBlocDonnees
    {
        private const AZSeparationColonnes m_separation_colonnes = AZSeparationColonnes.BoutonsALaFinDesChamps;
        private const int m_largeur_separation_colonnes = 20;
        private AZEcran m_p;
        private AZOnglet m_o;
        private AZTypeBlocDonnees m_type_bloc_donnees;
        private List<AZChamp> m_lc;
        //        private List<AZChampCritere> _lc_criteres;
        private string m_nom_table_bloc;
        private string m_header;
        private DataTable m_dt;
        private DataTable m_dt_suppr;
        private string m_req_lire;
        private string m_req_maj;
        private string m_proc_maj;
        private AZScrollView m_sv;
        private AZListView m_dg;
        //ABCD        private ListView _dgr;
        private bool m_avec_box_view;

        public AZBlocDonnees(AZEcran p, AZOnglet o, AZTypeBlocDonnees type_bloc_donnees, string nom_tab_bloc)
        {
            m_p = p;
            m_o = o;
            m_type_bloc_donnees = type_bloc_donnees;
            m_lc = new List<AZChamp>();
            m_nom_table_bloc = nom_tab_bloc;
        }
        public AZBlocDonnees(AZEcran p, AZOnglet o, AZTypeBlocDonnees type_bloc_donnees, List<AZChamp> lc, string nom_tab_bloc, string header, string req_lire, string req_maj, string proc_maj)
        {
            m_p = p;
            m_o = o;
            m_type_bloc_donnees = type_bloc_donnees;
            m_lc = lc;
            m_nom_table_bloc = nom_tab_bloc;
            m_header = header;
            m_req_lire = req_lire;
            m_req_maj = req_maj;
            m_proc_maj = proc_maj;
        }
        public AZEcran p { get { return m_p; } set { m_p = value; } }
        public AZOnglet o { get { return m_o; } set { m_o = value; } }
        public AZTypeBlocDonnees type_bloc_donnees { get { return m_type_bloc_donnees; } set { m_type_bloc_donnees = value; } }
        public List<AZChamp> lc { get { return m_lc; } set { m_lc = value; } }
        public string nom_table_bloc { get { return m_nom_table_bloc; } set { m_nom_table_bloc = value; } }
        public string header { get { return m_header; } set { m_header = value; } }
        public DataTable dt { get { return m_dt; } set { m_dt = value; } }
        public DataTable dt_suppr { get { return m_dt_suppr; } set { m_dt_suppr = value; } }
        public string req_lire { get { return m_req_lire; } set { m_req_lire = value; } }
        public string req_maj { get { return m_req_maj; } set { m_req_maj = value; } }
        public string proc_maj { get { return m_proc_maj; } set { m_proc_maj = value; } }
        public AZScrollView sv { get { return m_sv; } set { m_sv = value; } }
        public AZListView dg { get { return m_dg; } set { m_dg = value; } }
        //ABCD        public ListView dgr { get { return _dgr; } set { _dgr = value; } }
        public string NomDataGridPourGrille { get { return "dg" + m_nom_table_bloc; } }
        public bool avec_box_view { get { return m_avec_box_view; } set { m_avec_box_view = value; } }

        protected async void AfficherMessage(string msg)
        {
            bool trouve = false;
            bool fini = false;
            Element ma_page = m_sv;
            while (!fini)
            {
                if (ma_page is Page)
                {
                    trouve = true;
                    fini = true;
                }
                else if (ma_page == null)
                {
                    fini = true;
                }
                else
                {
                    ma_page = ma_page.Parent;
                }
            }
            if (trouve)
            {
                await ((Page)ma_page).DisplayAlert("Erreur", msg, "Cancel");
            }
            /*
            dtattente.Rows[0]["erreur"] = msg;
            dgattente.ItemsSource = null;
            dgattente.ItemsSource = dtattente.Rows;
            */
        }
        protected void AfficherException(Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg += " (" + ex.InnerException.Message + ")";
            }
            msg += ex.StackTrace;
            AfficherMessage(msg);
        }
        public async Task<bool> GenererChamps(string sql)
        {
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            m_dt = await ab.LireTable(sql);
            return await GenererChamps();
        }
        public async Task<bool> GenererChamps()
        {
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            lc = new List<AZChamp>();
            int max_cols = 999;
            int nb_cols = 0;
            int lg_min_champ_ecran = 80;
            int lg_max_champ_ecran = 300;
            string nb_ligs = "nb_lig";
            string nom_table_principale = "";
            string sql = "";
            nom_table_principale = p.nom_table_principale;
            foreach (DataColumn col in dt.Columns)
            {
                if (nb_cols < max_cols)
                {
                    string nom_col = col.ColumnName;
                    if (nom_col != "DateCreation" && nom_col != "UserCreation" && nom_col != "DateMaj" && nom_col != "UserMaj" && nom_col != "DateExtraction" && nom_col != "UserExtraction" && nom_col != "DateArchive" && nom_col != "UserArchive" && nom_col != "IdTech")
                    {
                        string type_col = col.DataType.Name.ToLower();
                        AZTypeDeChamp tc = AZTypeDeChamp.UNDEFINED;
                        int lg_col = 0;
                        string desc_col = nom_col;
                        if (!desc_col.EndsWith(" périmé"))
                        {
                            bool faire = true;
                            if (nom_col == "lib_cab_lg")
                            {
                                int a = 0;
                            }
                            if (nom_col == "@voir_doc")
                            {
                                lg_col = 4;
                                tc = AZTypeDeChamp.Blob;
                                desc_col = "Voir";
                            }
                            else if (nom_col == "@definir_doc")
                            {
                                lg_col = 4;
                                tc = AZTypeDeChamp.Blob;
                                desc_col = "Définir";
                            }
                            else
                            {
                                string sql_tmp = "select count(*) from information_schema.columns c inner join sys.columns as cc on cc.object_id = object_id(c.table_schema + '.' + c.table_name) and cc.name = c.column_name inner join sys.extended_properties p on p.major_id = cc.object_id and p.minor_id = cc.column_id and p.name = 'MS_description' where c.table_name not like 'v_' and c.column_name='" + nom_col + "' and isnull(p.value,'')!=''";
                                int? nb_desc = await ab.LireEntier(sql_tmp);
                                if (nb_desc > 0)
                                {
                                    sql_tmp = "select distinct p.value as column_description from information_schema.columns c inner join sys.columns as cc on cc.object_id = object_id(c.table_schema + '.' + c.table_name) and cc.name = c.column_name inner join sys.extended_properties p on p.major_id = cc.object_id and p.minor_id = cc.column_id and p.name = 'MS_description' where c.table_name not like 'v_' and c.column_name='" + nom_col + "' and isnull(p.value,'')!=''";
                                    DataTable dt_desc = await ab.LireTable(sql_tmp);
                                    if (dt_desc.Rows.Count == 1)
                                    {
                                        desc_col = dt_desc.Rows[0][0].ToString();
                                    }
                                }
                                switch (type_col)
                                {
                                    case "int32":
                                        lg_col = 4;
                                        if (nom_col.StartsWith("id_"))
                                        {
                                            if (nom_col == "id_" + nom_table_principale)
                                            {
                                                tc = AZTypeDeChamp.ClePrimairePrincipale;
                                            }
                                            else if (nom_col == "id_" + nom_table_bloc)
                                            {
                                                tc = AZTypeDeChamp.ClePrimaire;
                                            }
                                            else
                                            {
                                                tc = AZTypeDeChamp.Combobox;
                                                lg_col = 5;
                                            }
                                        }
                                        else
                                        {
                                            tc = AZTypeDeChamp.Entier;
                                        }
                                        break;
                                    case "varchar":
                                    case "nvarchar":
                                    case "string":
                                        tc = AZTypeDeChamp.Texte;
                                        lg_col = col.MaxLength;
                                        if (nom_col.EndsWith("WITH"))
                                        {
                                            string nom_col_base = nom_col.Substring(0, nom_col.Length - "WITH".Length);
                                            // la colonne nom_col_base est une cle etrangere: trouvons la longueur de la colonne vers laquelle elle pointe
                                            string nom_tab_fk = "";
                                            if (nom_table_bloc == "recherche")
                                            {
                                                if (nom_col_base.StartsWith("id_"))
                                                    nom_tab_fk = nom_col_base.Substring("id_".Length);
                                                else if (nom_col_base.StartsWith("_id_"))
                                                    nom_tab_fk = nom_col_base.Substring("_id_".Length);
                                            }
                                            else
                                            {
                                                sql = "select CU.TABLE_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 ON FK.CONSTRAINT_NAME = CU2.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.UNIQUE_CONSTRAINT_NAME = CU.CONSTRAINT_NAME where FK.CONSTRAINT_TYPE = 'FOREIGN KEY' and FK.TABLE_NAME = '" + nom_table_bloc + "' and CU2.COLUMN_NAME='" + nom_col_base + "'";
                                                nom_tab_fk = await ab.LireChaineOuVide(sql);
                                                if (nom_tab_fk.Length == 0)
                                                {
                                                    string nom_col_base_tmp = nom_col_base;
                                                    if (nom_col_base.StartsWith("_"))
                                                        nom_col_base_tmp = nom_col_base.Substring(1);
                                                    nom_tab_fk = nom_col_base_tmp.Substring(3);
                                                    string[] tab_nom_tab = nom_tab_fk.Split('_');
                                                    int nb_tabs = tab_nom_tab.Length;
                                                    bool trouve = false;
                                                    bool fini = false;
                                                    int nb_parties = nb_tabs;
                                                    while (!fini)
                                                    {
                                                        nom_tab_fk = "";
                                                        for (int i = 0; i < nb_parties; i++)
                                                        {
                                                            if (i > 0)
                                                                nom_tab_fk += "_";
                                                            nom_tab_fk += tab_nom_tab[i];
                                                        }
                                                        sql = "select count(*) from information_schema.tables where table_name='" + nom_tab_fk + "'";
                                                        int? nb2 = await ab.LireEntier(sql);
                                                        if (nb2 > 0)
                                                        {
                                                            trouve = true;
                                                            fini = true;
                                                        }
                                                        else
                                                        {
                                                            nb_parties--;
                                                            if (nb_parties == 0)
                                                                fini = true;
                                                        }
                                                    }
                                                    if (!trouve)
                                                    {
                                                        throw new Exception("Erreur: clé étrangère invalide (" + sql + ")");
                                                    }

                                                }
                                            }
                                            /*
                                            sql = "select CU.TABLE_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 ON FK.CONSTRAINT_NAME = CU2.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.UNIQUE_CONSTRAINT_NAME = CU.CONSTRAINT_NAME where FK.CONSTRAINT_TYPE = 'FOREIGN KEY' and FK.TABLE_NAME = '" + nom_table_bloc + "' and CU2.COLUMN_NAME='" + nom_col_base + "'";
                                            string nom_tab_fk = await ab.LireChaine(sql);
                                            */
                                            sql = "select count(*) from information_schema.columns where table_name='v_" + nom_tab_fk + "' and column_name='rep_" + nom_tab_fk + "'";
                                            int? nb = await ab.LireEntier(sql);
                                            if (nb.HasValue && nb.Value > 0)
                                            {
                                                sql = "select character_maximum_length from information_schema.columns where table_name='v_" + nom_tab_fk + "' and column_name='rep_" + nom_tab_fk + "'";
                                                int? lg_colx = await ab.LireEntier(sql);
                                                if (lg_colx != null && lg_colx.HasValue)
                                                    lg_col = lg_colx.Value;
                                            }
                                            else
                                                lg_col = 5;
                                            string nom_col_cbo = nom_col.Substring(0, nom_col.Length - 4);
                                            foreach (AZChamp c in lc)
                                            {
                                                if (c.NomChampBD() == nom_col_cbo)
                                                    c.lg_champ = lg_col;
                                            }
                                            /*
                                            if (dtMaj == null)
                                            {
                                                foreach (AZChamp c in lc_criteres)
                                                {
                                                    if (c.NomChampBD() == nom_col_cbo)
                                                        c.lg_champ = lg_col;
                                                }
                                                foreach (AZChamp c in lc_recherche)
                                                {
                                                    if (c.NomChampBD() == nom_col_cbo)
                                                        c.lg_champ = lg_col;
                                                }
                                            }
                                            */
                                        }
                                        else if (nom_col != "etat" && nom_col != "User")
                                        {
                                            /*
                                            string sql = "select count(*) from information_schema.columns where table_name='" + nom_tab + "' and column_name='" + nom_col + "'";
                                            int? nb = await ab.LireEntier(sql);
                                            if (nb.HasValue && nb.Value > 0)
                                            {
                                                sql = "select isnull(character_maximum_length,5) from information_schema.columns where table_name='" + nom_tab + "' and column_name='" + nom_col + "'";
                                                int? lg_colx = await ab.LireEntier(sql);
                                                if (lg_colx != null && lg_colx.HasValue)
                                                    lg_col = lg_colx.Value;
                                            }
                                            else
                                                lg_col = 5;
                                            */
                                            lg_col = col.MaxLength;
                                        }
                                        else
                                        {
                                            lg_col = 1;
                                        }
                                        break;
                                    case "datetime":
                                        tc = AZTypeDeChamp.Date;
                                        lg_col = 15;
                                        break;
                                    case "decimal":
                                    case "double":
                                        tc = AZTypeDeChamp.Double;
                                        lg_col = 8;
                                        break;
                                    case "bit":
                                    case "boolean":
                                        tc = AZTypeDeChamp.Booleen;
                                        lg_col = 5;
                                        break;
                                    case "uniqueidentifier":
                                        tc = AZTypeDeChamp.Guid;
                                        lg_col = 10;
                                        break;
                                    default:
                                        //                                    AfficherMessage("erreur: type inconnu");
                                        throw new Exception("erreur: type inconnu");
                                        //                                    break;
                                }
                            }
                            if (faire)
                            {
                                int largeur_ecran = lg_col * 10;
                                if (largeur_ecran < lg_min_champ_ecran)
                                    largeur_ecran = lg_min_champ_ecran;
                                if (largeur_ecran > lg_max_champ_ecran)
                                    largeur_ecran = lg_max_champ_ecran;
                                string nom_tab_ref_pour_cbo = "";
                                string base_req = "";
                                string base_filtre_lib = "";
                                string base_filtre_id = "";
                                if (tc == AZTypeDeChamp.Combobox)
                                {
                                    if (nom_col == "id_tenue_deb")
                                    {
                                        int a = 0;
                                    }
                                    string nom_col_tmp = nom_col;
                                    if (nom_col.StartsWith("_"))
                                        nom_col_tmp = nom_col.Substring(1);
                                    string nom_tab_tmp = nom_table_bloc;
                                    if (nom_table_bloc == "recherche")
                                        nom_tab_tmp = nom_table_principale;
                                    sql = "select CU.TABLE_NAME";
                                    sql += " FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C";
                                    sql += "	INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME";
                                    sql += "	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 ON FK.CONSTRAINT_NAME = CU2.CONSTRAINT_NAME";
                                    sql += "	INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME";
                                    sql += "	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.UNIQUE_CONSTRAINT_NAME = CU.CONSTRAINT_NAME";
                                    sql += "	where FK.CONSTRAINT_TYPE='FOREIGN KEY' and FK.TABLE_NAME = '" + nom_tab_tmp + "' and CU2.COLUMN_NAME='" + nom_col_tmp + "'";
                                    //                                    nom_tab_ref_pour_cbo = await ab.LireChaine(sql);
                                    //                                    if (nom_tab_ref_pour_cbo.Length == 0)
                                    DataTable dt_fk = await ab.LireTable(sql);
                                    if (dt_fk.Rows.Count > 0)
                                    {
                                        nom_tab_ref_pour_cbo = dt_fk.Rows[0][0].ToString();
                                    }
                                    else
                                    {
                                        string nom_tab_fk = nom_col_tmp.Substring(3);
                                        string[] tab_nom_tab = nom_tab_fk.Split('_');
                                        int nb_tabs = tab_nom_tab.Length;
                                        bool trouve = false;
                                        bool fini = false;
                                        int nb_parties = nb_tabs;
                                        while (!fini)
                                        {
                                            nom_tab_fk = "";
                                            for (int i = 0; i < nb_parties; i++)
                                            {
                                                if (i > 0)
                                                    nom_tab_fk += "_";
                                                nom_tab_fk += tab_nom_tab[i];
                                            }
                                            sql = "select count(*) from information_schema.tables where table_name='" + nom_tab_fk + "'";
                                            int? nb = await ab.LireEntier(sql);
                                            if (nb > 0)
                                            {
                                                nom_tab_ref_pour_cbo = nom_tab_fk;
                                                trouve = true;
                                                fini = true;
                                            }
                                            else
                                            {
                                                nb_parties--;
                                                if (nb_parties == 0)
                                                    fini = true;
                                            }
                                        }
                                        if (!trouve)
                                        {
                                            throw new Exception("Erreur: clé étrangère invalide (" + sql + ")");
                                        }
                                    }
                                    if (nom_tab_ref_pour_cbo.Length > 0)
                                    {
                                        base_req = "select @top id_" + nom_tab_ref_pour_cbo + " as @id,dbo.fct_rep('" + nom_tab_ref_pour_cbo + "',id_" + nom_tab_ref_pour_cbo + ") as @lib from " + nom_tab_ref_pour_cbo + " where 1=1 order by 2";
                                        base_filtre_lib = " and dbo.fct_rep('" + nom_tab_ref_pour_cbo + "')='@valeur'";
                                        base_filtre_id = " and id_" + nom_tab_ref_pour_cbo + "=@valeur";
                                    }
                                }
                                bool maj = false;
                                if (m_proc_maj != null)
                                {
                                    sql = "select count(*) from INFORMATION_SCHEMA.PARAMETERS where SPECIFIC_NAME='" + m_proc_maj + "' and PARAMETER_NAME='@" + nom_col + "'";
                                    int? nb_maj = await ab.LireEntier(sql);
                                    maj = nb_maj.Value > 0;
                                }
                                bool visible = true;
                                if (nom_col == "Mode" || nom_col == "etat" || nom_col == "User" || nom_col.EndsWith("WITH") || tc == AZTypeDeChamp.ClePrimaire || tc == AZTypeDeChamp.ClePrimairePrincipale || nom_col == nb_ligs || nom_col == "SelectId")
                                    visible = false;
                                //                                AZChamp c = new AZChamp(tc, desc_col, nom_col, lg_col, largeur_ecran, nom_tab, nom_tab_ref_pour_cbo, maj, visible, dans_grille);
                                bool oblig = col.AllowDBNull;
                                AZChamp c = new AZChamp(this, tc, desc_col, nom_col, lg_col, largeur_ecran, nom_tab_ref_pour_cbo, base_req, base_filtre_id, base_filtre_lib, maj, oblig, visible);
                                lc.Add(c);
                            }
                        }
                    }
                    nb_cols++;
                }
            }
            return true;
        }
        private ViewCell CreerItemTemplateRecherche()
        {
            return CreerItemTemplate("rech", false, m_lc);
        }
        private ViewCell CreerItemTemplateCriteres()
        {
            if (m_p is AZEcranRecherche)
            {
                List<AZChamp> lc_temp = new List<AZChamp>();
                foreach(AZChamp c in ((AZEcranRecherche)m_p).lc_criteres)
                {
                    lc_temp.Add(c);
                }
                return CreerItemTemplate("crit", false, lc_temp);
            }
            else
                return CreerItemTemplate("crit", false, m_lc);
        }
        private ViewCell CreerItemTemplateOnglet()
        {
            return CreerItemTemplate("", true, m_lc);
        }
        private ViewCell CreerItemTemplate(string prefixe, bool avec_boxview, List<AZChamp> liste_champs)
        {
            int nb_max_champs_grille = 999;
            double taille_textes = m_p.taille_textes;
            //            bool avec_grid_splitter = false;
            //            int largeur_grid_splitter = 10;
            ViewCell vc = null;
            try
            {
                var gr = new AZGrid();
                /*
                Label lbl = new Label();
                lbl.Text = "A";
                gr.Children.Add(lbl);
                */
                int num_col = 0;
                int num_champ = 0;
                if (avec_boxview)
                {
                    gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = gr.DonnerGridLength(10, AZGridUnitType.Absolute) });
                    AZBoxView bv = new AZBoxView();
                    bv.BackgroundColor = Color.Black;
                    bv.SetBinding(BoxView.ClassIdProperty, "ItemArray[1]");
                    Grid.SetColumn(bv, num_col);
                    gr.Children.Add(bv);
                    num_col++;
                }
                //                bool debut = true;
                int dernier_champ_visible = 0;
                for (int index_champ = 0; index_champ < liste_champs.Count; index_champ++)
                {
                    AZChamp c = liste_champs[index_champ];
                    if (c.visible)
                        dernier_champ_visible = index_champ;
                }
                for (int index_champ = 0; index_champ < liste_champs.Count; index_champ++)
                {
                    AZChamp c = liste_champs[index_champ];
                    if (c.visible)
                    {
                        /*
                        if (m_avec_grid_splitter)
                        {
                            if (debut == false) // && avec_redim
                            {
                                gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(m_largeur_grid_splitter, GridUnitType.Absolute) });
                                num_col++;
                            }
                        }
                        */
                        /*
                        if (avec_redim)
                        {
                            if (debut == false) // && avec_redim
                            {
                                gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Absolute) });
                                gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Absolute) });
                                if (avec_redim_tmp)
                                {
                                    Button btn_gauche = new Button();
                                    btn_gauche.Clicked += BtnGaucheClicked;
                                    btn_gauche.ClassId = num_col.ToString();
                                    btn_gauche.BackgroundColor = Color.DarkCyan;
                                    Grid.SetColumn(btn_gauche, num_col);
                                    gr.Children.Add(btn_gauche);
                                }
                                num_col++;
                                if (avec_redim_tmp)
                                {
                                    Button btn_droite = new Button();
                                    btn_droite.Clicked += BtnDroiteClicked;
                                    btn_droite.ClassId = num_col.ToString();
                                    btn_droite.BackgroundColor = Color.Chocolate;
                                    Grid.SetColumn(btn_droite, num_col);
                                    gr.Children.Add(btn_droite);
                                }
                                num_col++;
                            }
                        }
                        */
                        /*ABCD
                        int lg_col = c.lg_champ_ecran;
                        if (m_avec_grid_splitter)
                            lg_col += m_largeur_grid_splitter;
                        */
                        /*
                        if (num_col > 0 && m_avec_grid_splitter == true)
                        {
                            gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(m_largeur_grid_splitter, GridUnitType.Absolute) });
                            num_col++;
                        }
                        */
                        int lg_col = c.lg_champ_ecran;
                        if (m_separation_colonnes == AZSeparationColonnes.Splitter)
                        {
                            int delta_lg = m_largeur_separation_colonnes + (int)gr.ColumnSpacing;
                            if (index_champ == dernier_champ_visible)
                                delta_lg += m_largeur_separation_colonnes;
                            lg_col += delta_lg;
                        }
                        else if (m_separation_colonnes == AZSeparationColonnes.BoutonsALaFinDesChamps)
                        {
                            int delta_lg = 2 * m_largeur_separation_colonnes + (int)gr.ColumnSpacing;
                            lg_col += delta_lg;
                        }
                        gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = gr.DonnerGridLength(lg_col, AZGridUnitType.Absolute) });
                        /*
                        bool modif = c.maj;
                        if (prefixe == "crit")
                            modif = true;
                        else if (prefixe == "rech")
                            modif = false;
                        */
                        bool modif = prefixe == "rech" ? false : c.maj;
                        bool colorer = false;
                        Color coul_fond = Color.White;
                        if (prefixe != "rech" && prefixe != "crit")
                        {
                            //                            colorer = true;
                            if (modif == false)
                                coul_fond = Color.LightGray;
                            else if (c.oblig)
                                coul_fond = Color.LemonChiffon;
                        }
                        switch (c.type)
                        {
                            case AZTypeDeChamp.Blob:
                                AZButton btn = new AZButton();
                                Grid.SetColumn(btn, num_col);
                                btn.Clicked += BtnBlobClicked;
                                btn.ClassId = prefixe + c.NomChampBD();
//                                btn.IsEnabled = false;
                                gr.Children.Add(btn);
                                num_col++;
                                break;
                            case AZTypeDeChamp.Booleen:
                                AZSwitch sw = new AZSwitch();
                                Grid.SetColumn(sw, num_col);
                                sw.SetBinding(Switch.IsToggledProperty, "ItemArray[" + num_champ.ToString() + "]");
                                sw.Toggled += SwToggled;
                                sw.ClassId = prefixe + c.NomChampBD();
                                sw.IsEnabled = prefixe == "crit";
                                sw.IsEnabled = modif;
                                if (colorer)
                                    sw.BackgroundColor = coul_fond;
                                gr.Children.Add(sw);
                                num_col++;
                                break;
                            case AZTypeDeChamp.Combobox:
                                //                                int num_champ_lib = num_champ + 1;
                                if (modif == false)
                                {
                                    AZLabel lbl = new AZLabel();
                                    //                                    lbl.FontSize = taille_textes;
                                    Grid.SetColumn(lbl, num_col);
                                    bool binding_fait = false;
                                    for (int i = 0; i < lc.Count; i++)
                                    {
                                        if (lc[i].NomChampBD() == c.NomChampBD() + "WITH")
                                        {
                                            lbl.SetBinding(Label.TextProperty, new Binding("ItemArray[" + i.ToString() + "]"));
                                            binding_fait = true;
                                        }
                                    }
                                    if (!binding_fait)
                                    {
                                        //                                        lbl.SetBinding(Label.TextProperty, new Binding("ItemArray[" + num_champ.ToString() + "]"));
                                        lbl.Text = "Manque le champ WITH";
                                    }
                                    lbl.ClassId = prefixe + c.NomChampBD();
                                    if (colorer)
                                        lbl.BackgroundColor = coul_fond;
                                    gr.Children.Add(lbl);
                                }
                                else
                                {
                                    string titre = c.header;
                                    if (titre == "Début")
                                    {
                                        int a = 0;
                                    }
                                    string base_req = c.base_req;
                                    string base_filtre_lib = c.base_filtre_lib;
                                    string base_filtre_id = c.base_filtre_id;
                                    AZComboCS cbo = new AZComboCS(c, titre, base_req, base_filtre_lib, base_filtre_id);
//                                    cbo.IsEnabled = prefixe == "crit";
                                    Grid.SetColumn(cbo, num_col);
                                    cbo.SetBinding(AZComboCS.CboIdProperty, "ItemArray[" + num_champ.ToString() + "]");
                                    for (int i = 0; i < liste_champs.Count; i++)
                                    {
                                        if (liste_champs[i].NomChampBD() == c.NomChampBD() + "WITH")
                                        {
                                            cbo.SetBinding(AZComboCS.CboLibProperty, "ItemArray[" + i.ToString() + "]");
                                        }
                                    }
//                                    cbo.SetBinding(AZComboCS.CboIdObjetProperty, "ItemArray[i" + num_champ.ToString() + "]");
                                    cbo.CboIdChanged += CboIdChanged;
                                    cbo.CboLibChanged += CboLibChanged;
                                    cbo.ClassId = prefixe + c.NomChampBD();
                                    if (colorer)
                                        cbo.BackgroundColor = coul_fond;
                                    gr.Children.Add(cbo);
                                }
                                num_col++;
                                break;
                            case AZTypeDeChamp.Date:
                                AZDatePicker dp = new AZDatePicker();
                                dp.FontSize = taille_textes;
                                Grid.SetColumn(dp, num_col);
                                dp.SetBinding(DatePicker.DateProperty, new Binding("ItemArray[" + num_champ.ToString() + "]"));
                                dp.DateSelected += DateSelected;
                                dp.ClassId = prefixe + c.NomChampBD();
                                dp.IsEnabled = prefixe == "crit"; ;
                                dp.IsEnabled = modif;
                                if (colorer)
                                    dp.BackgroundColor = coul_fond;
                                gr.Children.Add(dp);
                                num_col++;
                                break;
                            case AZTypeDeChamp.Double:
                            case AZTypeDeChamp.Entier:
                            case AZTypeDeChamp.Texte:
                                if (modif == false)
                                {
                                    AZLabel lbl = new AZLabel();
                                    lbl.FontSize = taille_textes;
                                    Grid.SetColumn(lbl, num_col);
                                    lbl.SetBinding(Label.TextProperty, new Binding("ItemArray[" + num_champ.ToString() + "]"));
                                    lbl.ClassId = prefixe + c.NomChampBD();
                                    if (colorer)
                                        lbl.BackgroundColor = coul_fond;
                                    gr.Children.Add(lbl);
                                }
                                else
                                {
                                    AZEntry en = new AZEntry();
                                    en.FontSize = taille_textes;
                                    Grid.SetColumn(en, num_col);
                                    en.SetBinding(Entry.TextProperty, new Binding("ItemArray[" + num_champ.ToString() + "]"));
                                    en.TextChanged += TextChanged;
                                    en.ClassId = prefixe + c.NomChampBD();
                                    en.IsEnabled = prefixe == "crit";
                                    en.IsEnabled = modif;
                                    if (colorer)
                                        en.BackgroundColor = coul_fond;
                                    gr.Children.Add(en);
                                }
                                num_col++;
                                break;
                        }
                        //                        debut = false;
                    }
                    num_champ++;
                }
                vc = new ViewCell { View = gr };
            }
            catch (Exception ex)
            {
                //               AfficherException(ex);
                throw new Exception("CreerItemTemplate", ex);
            }
            return vc;
        }
        private async void BtnBlobClicked(object sender, EventArgs e)
        {
            try
            {
                AZButton btn = (AZButton)sender;
                AZGrid gr = (AZGrid)btn.Parent;
                string str_id_doc = "";
                ViewCell vc = (ViewCell)gr.Parent;
                AZListView lv = (AZListView)vc.Parent;
                AZGrid gr2 = (AZGrid)lv.Parent;
                string nom_table = gr2.ClassId.Substring(2);
                string nom_col = "id_" + nom_table;
                /*
                if (lv.SelectedItem != null)
                {
                    DataRow dr = (DataRow)lv.SelectedItem;
                    string str_id_doc = dr[nom_col].ToString();
                */
                AZBoxView bv = (AZBoxView)gr.Children[0];
                str_id_doc = bv.ClassId;
                if (str_id_doc.Length>0)
                {
                    string nom_champ = btn.ClassId;
                    if (nom_champ.EndsWith("@voir_doc"))
                    {
                        AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                        string sql = "select code_type_fic from " + nom_table + " d,type_fic t where d.id_type_fic=t.id_type_fic and d." + nom_col + "=" + str_id_doc;
                        string code_type_fic = await ab.LireChaine(sql);
                        switch (code_type_fic)
                        {
                            case ".gif":
                            case ".jpg":
                            case ".pdf":
                                p.Attente(true);
                                sql = "select doc from " + nom_table + " where " + nom_col + "=" + str_id_doc;
                                byte[] contenu = await ab.LireBlob(sql);
                                await o.p.Navigation.PushAsync(new AZVoirImagePage(contenu));
                                p.Attente(false);
                                break;
                            default:
                                AfficherMessage("Format de document non affichable");
                                break;
                        }
                    }
                    else if (nom_champ.EndsWith("@definir_doc"))
                    {
                        p.Attente(true);
                        Stream stream = await DependencyService.Get<IAccesAuxPhotos>().DonnerStreamVersPhotoAsync();
                        AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);
                        byte[] doc = ms.ToArray();
                        bool ret = await ab.EcrireBlob("update " + nom_table + " set doc=@blob where " + nom_col + "=" + str_id_doc, doc);
                        p.Attente(false);
                    }
                }
                else
                {
                    AfficherMessage("Il faut d'abord sélectionner la ligne du document à voir");
                }
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private AZGrid CreerGrilleRedimensionnement(int num_col, int num_champ)
        {
            AZGrid gr = new AZGrid();
            gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = gr.DonnerGridLength(0.5, AZGridUnitType.Star) });
            gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = gr.DonnerGridLength(0.5, AZGridUnitType.Star) });
            gr.Padding = 0.0;
            gr.ColumnSpacing = 0.0;
            AZButton btnmoins = new AZButton();
            btnmoins.Text = "-";
            btnmoins.BackgroundColor = Color.LightGreen;
            string classid = num_col.ToString() + "," + num_champ.ToString();
            btnmoins.ClassId = classid;
            //            btnmoins.HeightRequest = 30;
            //            btnmoins.WidthRequest = c.largeur_champ_ecran / 2;
            btnmoins.Clicked += BtnMoinsClicked;
            Grid.SetColumn(btnmoins, 0);
            gr.Children.Add(btnmoins);
            AZButton btnplus = new AZButton();
            btnplus.Text = "+";
            btnplus.BackgroundColor = Color.LightPink;
            btnplus.ClassId = classid;
            //            btnplus.HeightRequest = 30;
            //            btnplus.WidthRequest = c.largeur_champ_ecran / 2;
            btnplus.Clicked += BtnPlusClicked;
            Grid.SetColumn(btnplus, 1);
            gr.Children.Add(btnplus);
            return gr;
        }
        /*
        private Grid MaGridGrandMere(Element el)
        {
            bool trouve = false;
            bool fini = false;
            while (!fini)
            {
                if (el is Grid)
                {
                    trouve = true;
                    fini = true;
                }
                else if (el == null)
                    fini = true;
                else
                {
                    el = el.Parent;
                }
            }
            if (!trouve)
            {
                el = null;
            }
            else
            {
                el = el.Parent;
                trouve = false;
                fini = false;
                while (!fini)
                {
                    if (el is Grid)
                    {
                        trouve = true;
                        fini = true;
                    }
                    else if (el == null)
                        fini = true;
                    else
                    {
                        el = el.Parent;
                    }
                }
                if (!trouve)
                {
                    el = null;
                }
            }
            return (Grid)el;
        }
        private void DeplacerFrontiereVerticale(Button btn, bool vers_la_gauche)
        {
            int delta_largeur = vers_la_gauche ? -10 : 10;
            Grid gr = MaGridGrandMere(btn);
            if (gr != null)
            {
                string[] tab_num = btn.ClassId.Split(',');
                int num_col_btn = Convert.ToInt32(tab_num[0]);
                int num_champ_btn = Convert.ToInt32(tab_num[1]);
                int largeur_colonne = 0;
                if (gr.ClassId == "dgcriteres_recherche")
                {
                    if (p is AZEcranComplexe)
                    {
                        AZEcranComplexe ecr = (AZEcranComplexe)p;
                        ecr.lc_criteres[num_champ_btn].lg_champ_ecran += delta_largeur;
                    }
                }
                lc[num_champ_btn].lg_champ_ecran += delta_largeur;
                largeur_colonne = lc[num_champ_btn].lg_champ_ecran;
                gr.ColumnDefinitions[num_col_btn].Width = new GridLength((double)largeur_colonne);
                gr.IsVisible = false;
                dg.IsVisible = false;
                dg.ItemsSource = null;
                / *
                if (gr.ClassId == "dgcriteres_recherche")
                {
                    dg.ItemTemplate = new DataTemplate(CreerItemTemplateCriteres);
                }
                else
                {
                    dg.ItemTemplate = new DataTemplate(CreerItemTemplateOnglet);
                }
                * /
                if (dt != null)
                    dg.ItemsSource = dt.Rows;
                gr.IsVisible = true;
                dg.IsVisible = true;
                if (gr.ClassId == "dgcriteres_recherche" && p is AZEcranRecherche)
                {
                    AZEcranRecherche ecr = (AZEcranRecherche)p;
                    ecr.dg_criteres.IsVisible = false;
                    ecr.dg_criteres.ItemsSource = null;
                    //                    dgr.ItemTemplate = new DataTemplate(CreerItemTemplateRecherche);
                    if (ecr.dt_criteres != null)
                        ecr.dg_criteres.ItemsSource = ecr.dt_criteres.Rows;
                    ecr.dg_criteres.IsVisible = true;
                }
            }
        }
        */
        public void DeplacerFrontiereVerticale(string classid, int delta_largeur, bool maj)
        {
            AZGrid gr = (AZGrid)sv.Content;
            if (gr != null)
            {
                string[] tab_num = classid.Split(',');
                int num_col_btn = Convert.ToInt32(tab_num[0]);
                int num_champ_btn = Convert.ToInt32(tab_num[1]);
                int largeur_colonne = 0;
                if (gr.ClassId == "dgcriteres_recherche")
                {
                    if (p is AZEcranComplexe)
                    {
                        AZEcranComplexe ecr = (AZEcranComplexe)p;
                        ecr.lc_criteres[num_champ_btn].lg_champ_ecran += delta_largeur;
                    }
                }
                lc[num_champ_btn].lg_champ_ecran += delta_largeur;
                largeur_colonne = lc[num_champ_btn].lg_champ_ecran;
                gr.ColumnDefinitions[num_col_btn].Width = new GridLength((double)largeur_colonne);
                if (maj)
                {
                    gr.IsVisible = false;
                    dg.IsVisible = false;
                    dg.ItemsSource = null;
                    /*
                    if (gr.ClassId == "dgcriteres_recherche")
                    {
                        dg.ItemTemplate = new DataTemplate(CreerItemTemplateCriteres);
                    }
                    else
                    {
                        dg.ItemTemplate = new DataTemplate(CreerItemTemplateOnglet);
                    }
                    */
                    if (dt != null)
                        dg.ItemsSource = dt.Rows;
                    gr.IsVisible = true;
                    dg.IsVisible = true;
                    if (gr.ClassId == "dgcriteres_recherche" && p is AZEcranRecherche)
                    {
                        AZEcranRecherche ecr = (AZEcranRecherche)p;
                        ecr.dg_criteres.IsVisible = false;
                        ecr.dg_criteres.ItemsSource = null;
                        //                    dgr.ItemTemplate = new DataTemplate(CreerItemTemplateRecherche);
                        if (ecr.dt_criteres != null)
                            ecr.dg_criteres.ItemsSource = ecr.dt_criteres.Rows;
                        ecr.dg_criteres.IsVisible = true;
                    }
                }
            }
        }
        private void BtnMoinsClicked(object sender, EventArgs e)
        {
            try
            {
                AZButton btn = (AZButton)sender;
                int delta_vers_la_gauche = -10;
                DeplacerFrontiereVerticale(btn.ClassId, delta_vers_la_gauche, true);
            }
            catch (Exception ex)
            {
                //                AfficherException(ex);
            }
        }
        private void BtnPlusClicked(object sender, EventArgs e)
        {
            try
            {
                AZButton btn = (AZButton)sender;
                int delta_vers_la_droite = 10;
                DeplacerFrontiereVerticale(btn.ClassId, delta_vers_la_droite, true);
            }
            catch (Exception ex)
            {
                //                AfficherException(ex);
            }
        }
        /* creation d'une structure
         * - ScrollView
         *  - Grid
         *   - pour formulaire: les controles
         *   - pour grille de criteres: une ListView avec une ligne pour les criteres et une ListView pour la recherche
         *   - pour grille d'onglet: une Listview de données
         */
        public void InitIHM()
        {
            var activation_tri = new TapGestureRecognizer();
            activation_tri.Tapped += Activation_tri_Tapped;
            //int largeur_grid_splitter = 10;
            int nb_max_champs_formulaire = 999;
            int hauteur_grille_redimensionnement = m_separation_colonnes == AZSeparationColonnes.BoutonsAuDessus ? 30 : 1;
            m_sv = new AZScrollView();
            m_sv.IsVisible = false;
            switch (m_type_bloc_donnees)
            {
                case AZTypeBlocDonnees.Formulaire:
                    int num_lig = 0;
                    m_sv.Orientation = ScrollOrientation.Both;
                    AZGrid grf = new AZGrid();
                    grf.ColumnDefinitions.Add(new ColumnDefinition() { Width = grf.DonnerGridLength(150, AZGridUnitType.Absolute) });
                    grf.ColumnDefinitions.Add(new ColumnDefinition() { Width = grf.DonnerGridLength(1, AZGridUnitType.Star) });
                    //                        o.sv = (ScrollView)FindByName("tbprincipal");
                    //                        Grid gr = (Grid)FindByName("grprincipal");
                    for (int num_champ = 0; num_champ < m_lc.Count; num_champ++)
                    {
                        AZChamp c = m_lc[num_champ];
                        if (c.visible && num_lig < nb_max_champs_formulaire)
                        {
                            grf.RowDefinitions.Add(new RowDefinition() { Height = grf.DonnerGridLength(40, AZGridUnitType.Absolute) });
                            AZLabel lbl = new AZLabel();
                            Grid.SetColumn(lbl, 0);
                            Grid.SetRow(lbl, num_lig);
                            lbl.Text = c.header;
                            grf.Children.Add(lbl);
                            switch (c.type)
                            {
                                case AZTypeDeChamp.Booleen:
                                    AZSwitch sw = new AZSwitch();
                                    Grid.SetColumn(sw, 1);
                                    Grid.SetRow(sw, num_lig);
                                    sw.ClassId = c.NomChampBD();
                                    sw.Toggled += SwToggled;
                                    grf.Children.Add(sw);
                                    lc[num_champ].champ_saisie = sw;
                                    break;
                                case AZTypeDeChamp.Combobox:
                                    string titre = c.header;
                                    string base_req = c.base_req;
                                    string base_filtre_lib = c.base_filtre_lib;
                                    string base_filtre_id = c.base_filtre_id;
                                    AZComboCS cbo = new AZComboCS(c, titre, base_req, base_filtre_lib, base_filtre_id);
                                    Grid.SetColumn(cbo, 1);
                                    Grid.SetRow(cbo, num_lig);
                                    cbo.ClassId = c.NomChampBD();
                                    cbo.CboIdChanged += CboIdChanged;
                                    cbo.CboLibChanged += CboLibChanged;
                                    grf.Children.Add(cbo);
                                    lc[num_champ].champ_saisie = cbo;
                                    break;
                                case AZTypeDeChamp.Double:
                                case AZTypeDeChamp.Entier:
                                case AZTypeDeChamp.Texte:
                                    AZEntry en = new AZEntry();
                                    Grid.SetColumn(en, 1);
                                    Grid.SetRow(en, num_lig);
                                    en.ClassId = c.NomChampBD();
                                    en.TextChanged += TextChanged;
                                    grf.Children.Add(en);
                                    lc[num_champ].champ_saisie = en;
                                    break;
                                case AZTypeDeChamp.Date:
                                    AZDatePicker dp = new AZDatePicker();
                                    Grid.SetColumn(dp, 1);
                                    Grid.SetRow(dp, num_lig);
                                    dp.ClassId = c.NomChampBD();
                                    dp.DateSelected += DateSelected;
                                    grf.Children.Add(dp);
                                    lc[num_champ].champ_saisie = dp;
                                    break;
                            }
                            num_lig++;
                        }
                    }
                    m_sv.Content = grf;
                    break;
                case AZTypeBlocDonnees.CriteresRecherche:
                case AZTypeBlocDonnees.Grille:
                    m_sv.Orientation = ScrollOrientation.Horizontal;
                    m_sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
                    if (m_sv.Content != null)
                    {
                        AZGrid gr_tmp = (AZGrid)m_sv.Content;
                        gr_tmp.Children.Clear();
                        m_sv.Content = null;
                    }
                    AZGrid grg = new AZGrid();
                    grg.ClassId = NomDataGridPourGrille;
                    grg.RowDefinitions.Add(new RowDefinition() { Height = grg.DonnerGridLength(hauteur_grille_redimensionnement, AZGridUnitType.Absolute) });
                    //entetes de colonnes
                    grg.RowDefinitions.Add(new RowDefinition() { Height = grg.DonnerGridLength(20, AZGridUnitType.Absolute) });
                    if (m_type_bloc_donnees == AZTypeBlocDonnees.CriteresRecherche)
                    {
                        // grille de criteres
                        grg.RowDefinitions.Add(new RowDefinition() { Height = grg.DonnerGridLength(35, AZGridUnitType.Absolute) });
                    }
                    // donnees
                    grg.RowDefinitions.Add(new RowDefinition() { Height = grg.DonnerGridLength(1, AZGridUnitType.Star) });
                    int num_col = 0;
                    if (m_type_bloc_donnees != AZTypeBlocDonnees.CriteresRecherche)
                    {
                        // BoxView
                        grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = grg.DonnerGridLength(10, AZGridUnitType.Absolute) });
                        num_col = 1;
                    }
                    //                    bool debut = true;
                    //                    AZChamp champ_precedent = null;
                    int dernier_champ_visible = 0;
                    for (int i = 0; i < m_lc.Count; i++)
                    {
                        AZChamp c = m_lc[i];
                        if (c.visible)
                        {
                            dernier_champ_visible = i;
                        }
                    }
                    for (int i = 0; i < m_lc.Count; i++)
                    {
                        AZChamp c = m_lc[i];
                        if (c.visible)
                        {
                            //                            string classid_des_btn = num_col.ToString();
                            /*
                            if (0 == 1)    // GridSplitter
                            {
                                grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_grid_splitter, GridUnitType.Absolute) });
                                AZGridSplitter gs = new AZGridSplitter();
                                gs.HorizontalOptions = LayoutOptions.Center;
                                gs.ClassId = num_col.ToString();
                                gs.BackgroundColor = Color.Azure;
                                Grid.SetColumn(gs, num_col);
                                Grid.SetRow(gs, 1);
                                grg.Children.Add(gs);
                                num_col++;
                            }
                            */
                            grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = grg.DonnerGridLength(c.lg_champ_ecran, AZGridUnitType.Absolute) });
                            AZLabel lbl = new AZLabel();
                            Grid.SetColumn(lbl, num_col);
                            Grid.SetRow(lbl, 1);
                            lbl.Text = c.header;
                            lbl.FontAttributes = FontAttributes.Bold;
                            lbl.HorizontalTextAlignment = TextAlignment.Center;
                            lbl.ClassId = i.ToString();
                            lbl.GestureRecognizers.Add(activation_tri);
                            grg.Children.Add(lbl);
                            if (m_separation_colonnes == AZSeparationColonnes.BoutonsAuDessus)
                            {
                                AZGrid grbtn = CreerGrilleRedimensionnement(num_col, i);
                                Grid.SetColumn(grbtn, num_col);
                                Grid.SetRow(grbtn, 0);
                                grg.Children.Add(grbtn);
                            }
                            else if (m_separation_colonnes == AZSeparationColonnes.Splitter && i < dernier_champ_visible)
                            {
                                grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = grg.DonnerGridLength(m_largeur_separation_colonnes, AZGridUnitType.Absolute) });
                                AZGridSplitter grspl = new AZGridSplitter() { BackgroundColor = Color.Orange, HorizontalOptions = LayoutOptions.Center };
                                grspl.m_p = m_p;
                                grspl.m_bloc = this;
                                grspl.ClassId = (num_col).ToString() + "," + i.ToString();
                                Grid.SetColumn(grspl, ++num_col);
                                Grid.SetRow(grspl, 1);
                                grg.Children.Add(grspl);
                            }
                            else
                            {
                                /*
                                grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(m_largeur_grid_splitter, GridUnitType.Absolute) });
                                Button btnmoins = new Button() { BackgroundColor = Color.Olive };
                                btnmoins.ClassId = (num_col).ToString() + "," + i.ToString();
                                Grid.SetColumn(btnmoins, ++num_col);
                                Grid.SetRow(btnmoins, 1);
                                grg.Children.Add(btnmoins);
                                grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(m_largeur_grid_splitter, GridUnitType.Absolute) });
                                Button btnplus = new Button() { BackgroundColor = Color.Red };
                                btnplus.ClassId = (num_col).ToString() + "," + i.ToString();
                                Grid.SetColumn(btnplus, ++num_col);
                                Grid.SetRow(btnplus, 1);
                                grg.Children.Add(btnplus);
                                */
                                grg.ColumnDefinitions.Add(new ColumnDefinition() { Width = grg.DonnerGridLength(m_largeur_separation_colonnes * 2, AZGridUnitType.Absolute) });
                                AZGrid grbtn = CreerGrilleRedimensionnement(num_col++, i);
                                Grid.SetColumn(grbtn, num_col);
                                Grid.SetRow(grbtn, 1);
                                grg.Children.Add(grbtn);
                            }
                            num_col++;
                            //                            debut = false;
                            //                            champ_precedent = c;
                        }
                    }
                    //                        sl.Children.Add(gr);
                    m_sv.Content = grg;
                    m_dg = new AZListView();
                    m_dg.HasUnevenRows = true;
                    Grid.SetColumn(m_dg, 0);
                    Grid.SetColumnSpan(m_dg, num_col);
                    int num_row = m_type_bloc_donnees == AZTypeBlocDonnees.CriteresRecherche ? 3 : 2;
                    Grid.SetRow(m_dg, num_row);
                    m_dg.Margin = 0.0;
                    //                    dg.RowHeight = 40;
                    //20190515                    onglet_actif = o.NomOnglet
                    if (m_type_bloc_donnees == AZTypeBlocDonnees.CriteresRecherche)
                        m_dg.ItemTemplate = new DataTemplate(CreerItemTemplateRecherche);
                    else
                        m_dg.ItemTemplate = new DataTemplate(CreerItemTemplateOnglet);
                    grg.Children.Add(m_dg);
                    m_dg.ItemsSource = null;
                    if (dt != null)
                        m_dg.ItemsSource = dt.Rows;
//                    m_dg.ItemSelected += m_dg_ItemSelected;
//                    m_dg.ItemTapped += m_dg_ItemTapped;
                    if (m_type_bloc_donnees == AZTypeBlocDonnees.CriteresRecherche && p is AZEcranRecherche)
                    {
                        AZEcranRecherche ecr = (AZEcranRecherche)p;
                        // grille de criteres
                        ecr.dg_criteres = new AZListView();
                        ecr.dg_criteres.ItemsSource = ecr.dt_criteres.Rows;
                        ecr.dg_criteres.ItemTemplate = new DataTemplate(CreerItemTemplateCriteres);
                        Grid.SetColumn(ecr.dg_criteres, 0);
                        Grid.SetColumnSpan(ecr.dg_criteres, num_col);
                        Grid.SetRow(ecr.dg_criteres, 2);
                        ecr.dg_criteres.Margin = 0.0;
                        //                        ecr.dg_criteres.RowHeight = 30;
//                        ecr.dg_criteres.ItemTemplate = new DataTemplate(CreerItemTemplateCriteres);
                        grg.Children.Add(ecr.dg_criteres);
                    }
                    break;
            }
        }
        /*
        private void m_dg_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //            throw new NotImplementedException();
            DataRow dr = e.Item as DataRow;
            string nom_tab = dr.Table.TableName;
            if (nom_tab != "recherche" && nom_tab != "criteres")
                ((ListView)sender).SelectedItem = null; // de-select the row
        }
        private void m_dg_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DataRow dr = e.SelectedItem as DataRow;
            string nom_tab = dr.Table.TableName;
            if (nom_tab != "recherche" && nom_tab != "criteres")
            {
                int idx = e.SelectedItemIndex;
            }
            //            throw new NotImplementedException();
        }
        */
        private void Activation_tri_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (dt != null)
                {
                    dg.ItemsSource = null;
                    Label lbl = (Label)sender;
                    int num_champ = Convert.ToInt32(lbl.ClassId);
                    DataView dv = dt.DefaultView;
                    dv.Sort = lc[num_champ].NomChampBD();
                    dt = dv.ToTable();
                    dg.ItemsSource = dt.Rows;
                }
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private DataRow DonnerDataRow(object obj)
        {
            DataRow r = null;
            try
            {
                if (obj is Element)
                {
                    bool fini = false;
                    Element o = (Element)obj;
                    while (!fini)
                    {
                        if (o.BindingContext is DataRow)
                        {
                            r = (DataRow)o.BindingContext;
                            fini = true;
                        }
                        else
                        {
                            o = o.Parent;
                            if (o == null)
                                fini = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
            return r;
        }
        private void ToucherOngletCourant(bool toucher)
        {
            if (m_o != null)
                m_o.Toucher(toucher);
        }
        private void ModifCommun(string nom_champ, object sender, object val)
        {
            DataRow r = DonnerDataRow(sender);
            if (r != null && nom_champ.StartsWith("rech") == false)
            {
                string nom_champ_tmp = nom_champ;
                if (nom_champ.StartsWith("crit"))
                    nom_champ_tmp = nom_champ.Substring(4);
                if (r[nom_champ_tmp].ToString().Length > 0 || val.ToString().Length > 0)
                {
                    if (r[nom_champ_tmp] != val)
                    {
                        r[nom_champ_tmp] = val;
                        if (!nom_champ.StartsWith("crit"))
                            ToucherOngletCourant(true);
                    }
                }
            }
        }
        private void ModifEntry(string nom_champ, object sender)
        {
            string val = "";
            if (sender is AZEntry)
            {
                AZEntry c = (AZEntry)sender;
                val = c.Text;
            }
            else if (sender is AZComboCS)
            {
                AZComboCS c = (AZComboCS)sender;
                val = c.CboLib;
            }
            /*
            DataRow r = DonnerDataRow(sender);
            if (r != null)
            {
                string nom_champ_tmp = nom_champ;
                if (nom_champ.StartsWith("crit"))
                    nom_champ_tmp = nom_champ.Substring(4);
                if (r[nom_champ_tmp].ToString() != val)
                {
                    r[nom_champ_tmp] = val;
                    if (!nom_champ.StartsWith("crit"))
                        ToucherOngletCourant(true);
                }
            }
            */
            ModifCommun(nom_champ, sender, val);
        }
        private void ModifSwitch(string nom_champ, object sender)
        {
            bool val = false;
            if (sender is AZSwitch)
            {
                AZSwitch c = (AZSwitch)sender;
                val = c.IsToggled;
            }
            /*
            DataRow r = DonnerDataRow(sender);
            if (r != null && nom_champ.StartsWith("rech") == false)
            {
                string nom_champ_tmp = nom_champ;
                if (nom_champ.StartsWith("crit"))
                    nom_champ_tmp = nom_champ.Substring(4);
                if (r[nom_champ_tmp].ToString() != val.ToString())
                {
                    r[nom_champ_tmp] = val;
                    if (!nom_champ.StartsWith("crit"))
                        ToucherOngletCourant(true);
                }
            }
            */
            ModifCommun(nom_champ, sender, val);
        }
        private void ModifDate(string nom_champ, object sender)
        {
            DateTime val = DateTime.MinValue;
            if (sender is AZDatePicker)
            {
                AZDatePicker c = (AZDatePicker)sender;
                val = c.Date;
            }
            /*
            DataRow r = DonnerDataRow(sender);
            if (r != null)
            {
                string nom_champ_tmp = nom_champ;
                if (nom_champ.StartsWith("crit"))
                    nom_champ_tmp = nom_champ.Substring(4);
                if (r[nom_champ_tmp].ToString() != val.ToString())
                {
                    r[nom_champ_tmp] = val;
                    if (!nom_champ.StartsWith("crit"))
                        ToucherOngletCourant(true);
                }
            }
            */
            ModifCommun(nom_champ, sender, val);
        }
        private void ModifCboId(string nom_champ, object sender)
        {
            AZComboCS c = (AZComboCS)sender;
            DataRow r = (DataRow)c.Parent.Parent.BindingContext;
            if (r != null)
            {
                string nom_champ_tmp = nom_champ;
                if (nom_champ.StartsWith("crit"))
                    nom_champ_tmp = nom_champ.Substring(4);
                if (c.CboId == null || c.CboId.HasValue == false)
                {
                    if (r[nom_champ_tmp] != DBNull.Value)
                    {
                        r[nom_champ_tmp] = DBNull.Value;
                        if (!nom_champ.StartsWith("crit"))
                            ToucherOngletCourant(true);
                    }
                }
                else
                {
                    if (r[nom_champ_tmp].ToString() != c.CboId.Value.ToString())
                    {
                        r[nom_champ_tmp] = c.CboId.Value;
                        if (!nom_champ.StartsWith("crit"))
                            ToucherOngletCourant(true);
                    }
                }
            }
        }
        private void SwToggled(object sender, ToggledEventArgs e)
        {
            try
            {
                AZSwitch sw = (AZSwitch)sender;
                string nom_champ = sw.ClassId;
                ModifSwitch(nom_champ, sender);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                AZEntry en = (AZEntry)sender;
                string nom_champ = en.ClassId;
                ModifEntry(nom_champ, sender);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private void DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                AZDatePicker dp = (AZDatePicker)sender;
                string nom_champ = dp.ClassId;
                ModifDate(nom_champ, sender);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private void CboIdChanged(object sender, CboIdChangedEventArgs e)
        {
            try
            {
                AZComboCS cbo = (AZComboCS)sender;
                string nom_champ = cbo.ClassId;
                ModifCboId(nom_champ, sender);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        private void CboLibChanged(object sender, CboLibChangedEventArgs e)
        {
            try
            {
                AZComboCS cbo = (AZComboCS)sender;
                if (!cbo.ClassId.StartsWith("rech") && !cbo.ClassId.StartsWith("crit"))
                {
                    string nom_champ = cbo.ClassId + "WITH";
                    ModifEntry(nom_champ, sender);
                }
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        /*
        public virtual string PreparerSqlPourComboboxDetail(DataRow dr, string nom_grille, string nom_champ)
        {
            return "";
        }
        */
        public virtual string PreparerSqlPourComboboxDetail(AZComboCS cbo, AZGrid gr_ligne)
        {
            string sql = "";
            if(m_p is AZEcranComplexe)
            {
                sql = ((AZEcranComplexe)m_p).PreparerSqlPourComboboxDetail(cbo, gr_ligne);
            }
            return sql;
        }
    }
    /*
    public class ScrollEnabledChangedEventArgs : EventArgs
    {
        public ScrollEnabledChangedEventArgs(bool _ancienne_valeur, bool _nouvelle_valeur)
        {
            ancienne_valeur = _ancienne_valeur;
            nouvelle_valeur = _nouvelle_valeur;
        }
        public bool ancienne_valeur { get; set; }
        public bool nouvelle_valeur { get; set; }
    }

    public class AZScrollView : ScrollView
    {
        public AZScrollView():base()
        {

        }
        / *
        public static readonly BindableProperty ScrollEnabledProperty = BindableProperty.Create(nameof(AZScrollView), typeof(bool), typeof(AZScrollView), default(bool), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnScrollEnabledChanged);
        public bool ScrollEnabled;
        {
            get { return (bool)GetValue(ScrollEnabledProperty); }
            set { SetValue(ScrollEnabledProperty, value); }
        }
        public event EventHandler<ScrollEnabledChangedEventArgs> ScrollEnabledChanged;
        * /

        public static readonly BindableProperty ScrollingEnabledProperty = BindableProperty.Create<AZScrollView, bool>(p => p.ScrollingEnabled, true);
        public bool ScrollingEnabled
        {
            get { return (bool)GetValue(ScrollingEnabledProperty); }
            set { SetValue(ScrollingEnabledProperty, value); }
        }
    }
    */
}
