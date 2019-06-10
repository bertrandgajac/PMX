using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    public class AZEcranRecherche : AZEcran
    {
        protected AZBlocDonnees m_criteres_recherche;
        protected List<AZChampCritere> m_lc_criteres;
        protected string m_sql_recherche;
        protected string m_proc_recherche;

        protected DataTable m_dtcriteres;
        protected const int m_nb_max_lignes_recherche = 200;
        protected int m_id_courant = 0;
        protected AZListView m_dg_criteres;
        protected string m_nom_grid_criteres_recherche = "LayoutRoot";
        protected int m_num_row_criteres_recherche = 2;

        public List<AZChampCritere> lc_criteres { get { return m_lc_criteres; } set { m_lc_criteres = value; } }
        public DataTable dt_criteres { get { return m_dtcriteres; } }
        public AZListView dg_criteres { get { return m_dg_criteres; } set { m_dg_criteres = value; } }
        public string nom_grid_criteres_recherche { get { return m_nom_grid_criteres_recherche; } set { m_nom_grid_criteres_recherche = value; } }
        public int num_row_criteres_recherche { get { return m_num_row_criteres_recherche; } set { m_num_row_criteres_recherche = value; } }

        public AZEcranRecherche() : base()
        {

        }
        public override void Init(string _nom_serveur, string _nom_bd, int _mode_generation, string _nom_ecran, List<string> _lst_nom_tab, bool _proc_avec_user, string _nom_champ_etat, int _lg_min_champ_ecran, int _lg_max_champ_ecran, double _taille_textes)
        {
            base.Init(_nom_serveur, _nom_bd, _mode_generation, _nom_ecran, _lst_nom_tab, _proc_avec_user, _nom_champ_etat, _lg_min_champ_ecran, _lg_max_champ_ecran, _taille_textes);
            m_proc_recherche = "AZ" + m_lst_nom_tab[0] + "__recherche";
            m_sql_recherche = "exec " + m_proc_recherche + " 0 @criteres";
        }
        /*
        public override void Init(string _nom_serveur, string _nom_bd, int _mode_generation, string _nom_ecran, List<string> _lst_nom_tab, bool _proc_avec_user, string _nom_champ_etat, int _lg_min_champ_ecran, int _lg_max_champ_ecran, bool _avec_redim)
        {
            base.Init(_nom_serveur, _nom_bd, _mode_generation, _nom_ecran, _lst_nom_tab, _proc_avec_user, _nom_champ_etat, _lg_min_champ_ecran, _lg_max_champ_ecran, _avec_redim);
            m_proc_recherche = "AZ" + m_lst_nom_tab[0] + "__recherche";
            m_sql_recherche = "exec " + m_proc_recherche + " 0 @criteres";
        }
        */
        protected virtual string RemplacerTopPourRequeteRecherche(int max_lignes, string nb_ligs)
        {
            string ch = "top " + max_lignes.ToString() + " count(*) over() as " + nb_ligs + ",";
            return ch;
        }
        protected override async Task<bool> Init1()
        {
            bool ret = await base.Init1();
            m_criteres_recherche = new AZBlocDonnees(this, (AZOnglet)null, AZTypeBlocDonnees.CriteresRecherche, "recherche");
            m_lc_criteres = new List<AZChampCritere>();
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            string sql = "select count(*) from AZchamp_crit where id_AZecr=" + Formater(this.m_id_AZecr);
            int? nb_crit = await ab.LireEntier(sql);
            if (nb_crit.Value > 0)
            {
                sql = "select c.id_AZtype_champ,c.entete,c.nom_champ,c.lg_champ,c.lg_champ_ecr,c.maj,c.oblig,c.visible,c.dans_grille,c.cbo_nom_tab_ref,c.cbo_req,c.cbo_filtre_lib,c.cbo_filtre_id,cc.clause_sql from AZchamp_crit cc inner join AZchamp c on cc.id_AZchamp=c.id_AZchamp where cc.id_AZecr=" + Formater(m_id_AZecr) + " order by cc.num_champ";
                DataTable dt_champs_crit = await ab.LireTable(sql);
                foreach (DataRow dr in dt_champs_crit.Rows)
                {
                    AZTypeDeChamp tc = (AZTypeDeChamp)Convert.ToInt32(dr["id_AZtype_champ"].ToString());
                    string entete = dr["entete"].ToString();
                    string nom_champ = dr["nom_champ"].ToString();
                    int lg_champ = Convert.ToInt32(dr["lg_champ"].ToString());
                    int lg_champ_ecr = Convert.ToInt32(dr["lg_champ_ecr"].ToString());
                    bool maj_champ = Convert.ToBoolean(dr["maj"].ToString());
                    bool oblig = Convert.ToBoolean(dr["oblig"].ToString());
                    bool visible = Convert.ToBoolean(dr["visible"].ToString());
                    string cbo_nom_tab_ref = dr["cbo_nom_tab_ref"].ToString();
                    string cbo_req = dr["cbo_req"].ToString();
                    string cbo_filtre_lib = dr["cbo_filtre_lib"].ToString();
                    string cbo_filtre_id = dr["cbo_filtre_id"].ToString();
                    string clause_sql = dr["clause_sql"].ToString();
//                    AZChamp ch = new AZChamp(m_criteres_recherche, tc, entete, nom_champ, lg_champ, lg_champ_ecr, cbo_nom_tab_ref, cbo_req, cbo_filtre_id, cbo_filtre_lib, maj_champ, oblig, visible);
//                    m_criteres_recherche.lc.Add(ch);
                    AZChampCritere chc = new AZChampCritere(m_criteres_recherche, tc, entete, nom_champ, lg_champ, lg_champ_ecr, cbo_nom_tab_ref, cbo_req, cbo_filtre_id, cbo_filtre_lib, maj_champ, visible, clause_sql);
                    m_lc_criteres.Add(chc);
                }
                sql = "select c.id_AZtype_champ,c.entete,c.nom_champ,c.lg_champ,c.lg_champ_ecr,c.maj,c.oblig,c.visible,c.dans_grille,c.cbo_nom_tab_ref,c.cbo_req,c.cbo_filtre_lib,c.cbo_filtre_id from AZchamp_rech cr inner join AZchamp c on cr.id_AZchamp=c.id_AZchamp where cr.id_AZecr=" + Formater(m_id_AZecr) + " order by cr.num_champ";
                DataTable dt_champs_rech = await ab.LireTable(sql);
                foreach (DataRow dr in dt_champs_rech.Rows)
                {
                    AZTypeDeChamp tc = (AZTypeDeChamp)Convert.ToInt32(dr["id_AZtype_champ"].ToString());
                    string entete = dr["entete"].ToString();
                    string nom_champ = dr["nom_champ"].ToString();
                    int lg_champ = Convert.ToInt32(dr["lg_champ"].ToString());
                    int lg_champ_ecr = Convert.ToInt32(dr["lg_champ_ecr"].ToString());
                    bool maj_champ = Convert.ToBoolean(dr["maj"].ToString());
                    bool oblig = Convert.ToBoolean(dr["oblig"].ToString());
                    bool visible = Convert.ToBoolean(dr["visible"].ToString());
                    string cbo_nom_tab_ref = dr["cbo_nom_tab_ref"].ToString();
                    string cbo_req = dr["cbo_req"].ToString();
                    string cbo_filtre_lib = dr["cbo_filtre_lib"].ToString();
                    string cbo_filtre_id = dr["cbo_filtre_id"].ToString();
                    AZChamp ch = new AZChamp(m_criteres_recherche, tc, entete, nom_champ, lg_champ, lg_champ_ecr, cbo_nom_tab_ref, cbo_req, cbo_filtre_id, cbo_filtre_lib, maj_champ, oblig, visible);
                    m_criteres_recherche.lc.Add(ch);
                }
                //                ret = await MemoriserDefinitionEcran();
            }
            else
            {
                sql = m_sql_recherche.Replace("@top", RemplacerTopPourRequeteRecherche(1, m_nb_ligs)).Replace("@criteres", "");
                bool gen_ch_crit = await m_criteres_recherche.GenererChamps(sql);
                m_lc_criteres = new List<AZChampCritere>();
                foreach (AZChamp c in m_criteres_recherche.lc)
                {
                    string clause_sql = ",@" + c.nom_champ + "=@valeur";
                    sql = "select count(*) from INFORMATION_SCHEMA.PARAMETERS where SPECIFIC_NAME='" + m_proc_recherche + "' and PARAMETER_NAME='@" + c.NomChampBD() + "'";
                    int? nb_maj = await ab.LireEntier(sql);
                    bool maj = nb_maj.Value > 0;
                    AZChampCritere cc = new AZChampCritere(m_criteres_recherche, c.type, c.header, c.nom_champ, c.lg_champ, c.lg_champ_ecran, c.nom_tab_ref_pour_cbo, c.base_req, c.base_filtre_id, c.base_filtre_lib, maj, c.visible, clause_sql);
                    m_lc_criteres.Add(cc);
                }                //                lcr = await GenererChamps("recherche", sql);
                ret = await MemoriserDefinitionEcran();
            }
            return ret;
        }
        protected override bool InitIHM()
        {
            bool ret = base.InitIHM();
            m_dtcriteres = new DataTable();
            m_dtcriteres.TableName = "criteres";
            ((ListView)this.FindByName("dgattente")).ItemsSource = m_dtattente.Rows;
            m_criteres_recherche.nom_table_bloc = "criteres_recherche";
            GenererCriteres();
            m_criteres_recherche.InitIHM();
            m_criteres_recherche.sv.Visible = true;
            m_criteres_recherche.dg.ItemsSource = null;
            m_criteres_recherche.dg.ItemSelected += dgrecherche_ItemSelected;
            Grid.SetRow(m_criteres_recherche.sv, m_num_row_criteres_recherche);
            Grid layoutroot = (Grid)this.FindByName(m_nom_grid_criteres_recherche);
            layoutroot.Children.Add(m_criteres_recherche.sv);
            return ret;
        }
        protected virtual string PreparerCriteres(DataRow dr)
        {
            string crit_sql = "";
            foreach (AZChampCritere c in m_lc_criteres)
            {
                if (c.visible)
                {
                    switch (c.type)
                    {
                        case AZTypeDeChamp.Booleen:
                            if (dr[c.NomChampBD()] != null && dr[c.NomChampBD()].ToString().Length > 0)
                            {
                                bool val_bool = Convert.ToBoolean(dr[c.NomChampBD()].ToString());
                                /*
                                if (val_bool)
                                    crit_sql += ",@" + c.NomChampBD() + " = 1";
                                */
                                crit_sql += ",@" + c.NomChampBD() + "=" + (val_bool ? "1" : "0");
                                //                                crit_sql += " and " + c.NomChampBD() + " = 1";
                            }
                            break;
                        case AZTypeDeChamp.ClePrimaire:
                        case AZTypeDeChamp.ClePrimairePrincipale:
                            break;
                        case AZTypeDeChamp.Combobox:
                            // on ne teste que sur le champ WITH
                            if (dr[c.NomChampBD()] != null && dr[c.NomChampBD()].ToString().Length > 0)
                            {
                                int val_cbo = Convert.ToInt32(dr[c.NomChampBD()].ToString());
                                crit_sql += ",@" + c.NomChampBD() + "=" + val_cbo;
                            }
                            break;
                        case AZTypeDeChamp.Date:
                            if (dr[c.NomChampBD()] != null && dr[c.NomChampBD()].ToString().Length > 0)
                            {
                                DateTime val_date = Convert.ToDateTime(dr[c.NomChampBD()].ToString());
                                if (val_date != DateTime.MinValue)
                                    crit_sql += ",@" + c.NomChampBD() + " > '" + ConvertirDateTimePourSql(val_date) + "'";
                            }
                            break;
                        case AZTypeDeChamp.Double:
                            if (dr[c.NomChampBD()] != null && dr[c.NomChampBD()].ToString().Length > 0)
                            {
                                double val_double = Convert.ToDouble(dr[c.NomChampBD()].ToString());
                                if (val_double > 0.0)
                                    crit_sql += ",@" + c.NomChampBD() + " = " + Convert.ToDouble(val_double.ToString());
                            }
                            break;
                        case AZTypeDeChamp.Entier:
                            if (dr[c.NomChampBD()] != null && dr[c.NomChampBD()].ToString().Length > 0)
                            {
                                int val_int = Convert.ToInt32(dr[c.NomChampBD()].ToString());
                                if (val_int > 0)
                                    crit_sql += ",@" + c.NomChampBD() + " = " + Convert.ToInt32(val_int.ToString());
                            }
                            break;
                        case AZTypeDeChamp.Texte:
                            string val_string = dr[c.NomChampBD()].ToString();
                            if (val_string != null && val_string.Length > 0)
                                crit_sql += ",@" + c.NomChampBD() + "= " + Formater(val_string.ToString());
                            break;
                    }
                }
            }
            return crit_sql;
        }
        public virtual async Task<bool> Rechercher()
        {
            bool ret = false;
            Attente(true);
            try
            {
                if (!m_init_fait)
                {
                    //                AfficherMessage("pas passé");
                    //                    ((Button)this.FindByName("btnrecherche")).IsEnabled = false;
                    ((AZButton)this.ChercherParNom("btnrecherche")).IsEnabled = false;
                    ret = await Init1();
                    if (ret)
                    {
                        ret = InitIHM();
                        if (ret)
                        {
                            m_init_fait = true;
                            //                        Rechercher();
                            //((Button)this.FindByName("btnrecherche")).Text = "Rechercher";
                            //((Button)this.FindByName("btnrecherche")).IsEnabled = true;
                            ((AZButton)this.ChercherParNom("btnrecherche")).Text = "Rechercher";
                            ((AZButton)this.ChercherParNom("btnrecherche")).IsEnabled = true;
                            RendreBoutonVisible("btnvider_crit");
                            RendreBoutonVisible("btncreer");
                            RendreBoutonVisible("btnsauver");
                            RendreBoutonVisible("btnsupprimer");
                            RendreBoutonVisible("btnsauver_ecran");
                            RendreBoutonVisible("btnsortie_ecran");
                            RendreBoutonVisible("btnhaut");
                            RendreBoutonVisible("btnbas");
                            //ABCD                            m_onglet_actif = "";
                            RechercheNb("");
                        }
                    }
                }
                else
                {
                    bool faire = true;
                    if (m_objet_courant_modifie)
                    {
                        ret = await DemandeSauvegarde();
                        if (ret)
                            faire = false;
                    }
                    if (faire)
                    {
                        EffacerMessage();
                        //ABCD                        ReinitialiserBoutons();
                        AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                        this.Title = m_lib_ecran;
                        int max_lignes = m_nb_max_lignes_recherche;
                        //                        string sql = "select @top 0 as sel,id_cab,id_fct,id_fctWITH,nom_cab,ind_cab_etat,id_voie,id_voieWITH,id_coul,id_coulWITH,id_car_type_cab,id_car_type_cabWITH,id_statut_cab,id_statut_cabWITH,id_statut_cab_itin,id_statut_cab_itinWITH from v_cab_recherche where 1=1 @critere";
                        string sql = m_sql_recherche;
                        string crit_sql = "";
                        var enumerateur = dg_criteres.ItemsSource.GetEnumerator();
                        enumerateur.MoveNext();
                        object criteres = enumerateur.Current;
                        if (criteres != null)
                        {
                            DataRow dr = (DataRow)criteres;
                            crit_sql = PreparerCriteres(dr);
                        }
                        string sql_val = sql.Replace("@top", RemplacerTopPourRequeteRecherche(max_lignes, m_nb_ligs));
                        sql_val = sql_val.Replace("@criteres", crit_sql);
                        m_criteres_recherche.dt = await ab.LireTable(sql_val);
                        m_criteres_recherche.dt.TableName = "recherche";
                        int nb_lignes = m_criteres_recherche.dt.Rows.Count;
                        if (nb_lignes > 0)
                        {
                            int nb_max_lignes = Convert.ToInt32(m_criteres_recherche.dt.Rows[0][m_nb_ligs]);
                            m_criteres_recherche.dg.ItemsSource = m_criteres_recherche.dt.Rows;
                            m_criteres_recherche.dg.BackgroundColor = nb_lignes < nb_max_lignes ? Color.Yellow : Color.White;
                            //ABCD                            EffacerOnglets();
                            string str_nb_lignes = nb_lignes.ToString();
                            if (m_criteres_recherche.dt.Columns.Contains(m_nb_ligs))
                            {
                                str_nb_lignes += "/" + m_criteres_recherche.dt.Rows[0][m_nb_ligs].ToString();
                            }
                            RechercheNb(str_nb_lignes);
                        }
                        else
                        {
                            m_criteres_recherche.dg.ItemsSource = null;
                            RechercheNb("");
                        }
                        m_id_courant = -1;
                        //ABCD                        ActiverChampsOngletZero(false);
                    }
                }
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
            finally
            {
                //                ((Button)this.FindByName("btnrecherche")).IsEnabled = true;
                ((AZButton)this.ChercherParNom("btnrecherche")).IsEnabled = true;
                Attente(false);
            }
            return ret;
        }
        protected void GenererCriteres()
        {
            m_dtcriteres = new DataTable();
            m_dtcriteres.TableName = "criteres";
            DataColumn dc;
            foreach (AZChamp c in m_lc_criteres)
            {
                dc = new DataColumn(c.NomChampBD(), c.TypeCsharp());
                m_dtcriteres.Columns.Add(dc);
            }
            DataRow dr = m_dtcriteres.NewRow();
            m_dtcriteres.Rows.Add(dr);
        }
        protected void ViderCriteres()
        {
            m_criteres_recherche.dg.ItemsSource = null;
            GenererCriteres();
            m_criteres_recherche.dg.ItemsSource = m_dtcriteres.Rows;
        }
        protected async void dgrecherche_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Attente(true);
            try
            {
                object objet_sel = m_criteres_recherche.dg.SelectedItem;
                if (objet_sel != null)
                {
                    object val = null;
                    DataRow dr = (DataRow)objet_sel;
                    if (dr.Table.Columns.Contains(m_nom_cle_primaire))
                        val = dr[m_nom_cle_primaire];
                    else if (dr.Table.Columns.Contains("SelectId"))
                        val = dr["SelectId"];
                    else
                        throw new Exception("La table recherche ne contient pas de colonne identifiant l'objet courant");
                    bool faire = true;
                    if (m_objet_courant_modifie && m_id_courant != (int)val)
                    {
                        bool ret = await DemandeSauvegarde();
                        if (ret)
                            faire = false;
                    }
                    if (faire)
                    {
                        //ABCD                        ReinitialiserBoutons();
                        m_id_courant = (int)val;
                        await AfficherDetail();
                        //ABCD                        ActiverChampsOngletZero(true);
                    }
                }
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
            finally
            {
                Attente(false);
            }
        }
        protected virtual async Task<bool> AfficherDetail()
        {
            EffacerMessage();
            return true;
        }
        protected async override Task<bool> MemoriserDefinitionEcran()
        {
            bool ret = await base.MemoriserDefinitionEcran();
            RechercheNb("criteres");
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            /*
            string sql = "select id_AZecr from AZecr where nom_ecr=" + Formater(m_nom_ecran);
            int? id_ecr = await ab.LireEntier(sql);
            */
            for (int num_champ = 0; num_champ < m_lc_criteres.Count; num_champ++)
            {
                AZChampCritere c = m_lc_criteres[num_champ];
                string sql = "select count(*) from AZchamp c inner join AZchamp_crit cc on c.id_AZchamp=cc.id_AZchamp where cc.id_AZecr=" + Formater(m_id_AZecr) + " and c.nom_champ=" + Formater(c.NomChampBD());
                int? nb = await ab.LireEntier(sql);
                if (nb == 0)
                {
                    int id_type_champ = Convert.ToInt32(c.type);
                    sql = "exec AZchampCreer ";
                    sql += Formater(id_type_champ);
                    sql += "," + Formater(c.header);
                    sql += "," + Formater(c.NomChampBD());
                    sql += "," + Formater(c.lg_champ);
                    sql += "," + Formater(c.lg_champ_ecran);
                    sql += "," + Formater(c.maj);
                    sql += "," + Formater(c.oblig);
                    sql += "," + Formater(c.visible);
                    sql += "," + Formater(c.nom_tab_ref_pour_cbo);
                    sql += "," + Formater(c.base_req);
                    sql += "," + Formater(c.base_filtre_lib);
                    sql += "," + Formater(c.base_filtre_id);
                    int? id_AZchamp = await ab.LireEntier(sql);
                    sql = "exec AZchamp_critCreer ";
                    sql += Formater(m_id_AZecr);
                    sql += "," + Formater(num_champ);
                    sql += "," + Formater(id_AZchamp.Value);
                    sql += "," + Formater(c.clause_sql);
                    await ab.ExecSql(sql);
                }
                else
                {
                    sql = "select c.id_AZchamp from AZchamp c inner join AZchamp_crit cc on c.id_AZchamp=cc.id_AZchamp where cc.id_AZecr=" + Formater(m_id_AZecr) + " and c.nom_champ=" + Formater(c.NomChampBD());
                    int? id_AZchamp = await ab.LireEntier(sql);
                    sql = "update AZchamp set lg_champ_ecr=" + Formater(c.lg_champ_ecran) + " where id_AZchamp=" + Formater(id_AZchamp);
                    await ab.ExecSql(sql);
                }
            }
            RechercheNb("recherche");
            for (int num_champ = 0; num_champ < m_criteres_recherche.lc.Count; num_champ++)
            {
                AZChamp c = m_criteres_recherche.lc[num_champ];
                string sql = "select count(*) from AZchamp c inner join AZchamp_rech cr on c.id_AZchamp=cr.id_AZchamp where cr.id_AZecr=" + Formater(m_id_AZecr) + " and c.nom_champ=" + Formater(c.NomChampBD());
                int? nb = await ab.LireEntier(sql);
                if (nb == 0)
                {
                    int id_type_champ = Convert.ToInt32(c.type);
                    sql = "exec AZchampCreer ";
                    sql += Formater(id_type_champ);
                    sql += "," + Formater(c.header);
                    sql += "," + Formater(c.NomChampBD());
                    sql += "," + Formater(c.lg_champ);
                    sql += "," + Formater(c.lg_champ_ecran);
                    sql += "," + Formater(c.maj);
                    sql += "," + Formater(c.oblig);
                    sql += "," + Formater(c.visible);
                    sql += "," + Formater(c.nom_tab_ref_pour_cbo);
                    sql += "," + Formater(c.base_req);
                    sql += "," + Formater(c.base_filtre_lib);
                    sql += "," + Formater(c.base_filtre_id);
                    int? id_AZchamp = await ab.LireEntier(sql);
                    sql = "exec AZchamp_rechCreer ";
                    sql += Formater(m_id_AZecr);
                    sql += "," + Formater(num_champ);
                    sql += "," + Formater(id_AZchamp.Value);
                    await ab.ExecSql(sql);
                }
                else
                {
                    sql = "select c.id_AZchamp from AZchamp c inner join AZchamp_rech cr on c.id_AZchamp=cr.id_AZchamp where cr.id_AZecr=" + Formater(m_id_AZecr) + " and c.nom_champ=" + Formater(c.NomChampBD());
                    int? id_AZchamp = await ab.LireEntier(sql);
                    sql = "update AZchamp set lg_champ_ecr=" + Formater(c.lg_champ_ecran) + " where id_AZchamp=" + Formater(id_AZchamp);
                    await ab.ExecSql(sql);
                }
            }
            return ret;
        }
    }
}

