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
    public partial class AZEcranComplexe : AZEcranRecherche
    {
        private List<AZOnglet> m_onglets;
        private string m_onglet_actif = "";
        public AZEcranComplexe() : base()
        {
            InitializeComponent();
        }
        protected override async Task<bool> Init1()
        {
            bool ret = await base.Init1();
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            bool debut = true;
            m_onglets = new List<AZOnglet>();
            bool sauver = false;
            foreach (string nom_tab in m_lst_nom_tab)
            {
                AZTypeOnglet to = debut ? AZTypeOnglet.Formulaire : AZTypeOnglet.Grille;
                AZOnglet o = new AZOnglet(this, to, nom_tab);
                this.recherche_nb.Text = "init1: " + nom_tab;
                string nom_param = "@" + m_nom_cle_primaire;
                //                string complement = nom_tab == m_lst_nom_tab[0] ? "" : m_lst_nom_tab[0] + "__";
                string complement = m_lst_nom_tab[0] + "__";
                string nom_proc = "AZ" + complement + nom_tab + "Select";
                string req_lire = "exec " + nom_proc + " " + nom_param;
                string req_maj = "";
                string proc_maj = "AZ" + complement + nom_tab + "Maj";
                o.bd.proc_maj = proc_maj;
                if (m_proc_avec_user)
                    req_lire += ",'a'";
                string desc_tab = nom_tab;
                string sql = "select id_AZonglet,entete,req_lire,req_maj from AZonglet where id_AZecr=" + Formater(m_id_AZecr) + " and nom_table=" + Formater(nom_tab);
                DataTable dt_AZonglet = await ab.LireTable(sql);
                if (dt_AZonglet.Rows.Count == 0)
                {
                    sauver = true;
                    sql = req_lire.Replace(nom_param, "-1");
                    bool a = await o.bd.GenererChamps(sql);
                    o.req_lire = req_lire;
                    req_maj = "exec " + proc_maj;
                    string sep = " ";
                    foreach (AZChamp c in o.bd.lc)
                    {
                        if (c.maj)
                        {
                            req_maj += sep + "@" + c.NomChampBD();
                            sep = ",";
                        }
                    }
                }
                else
                {
                    int id_AZonglet = Convert.ToInt32(dt_AZonglet.Rows[0]["id_AZonglet"].ToString());
                    string entete = dt_AZonglet.Rows[0]["entete"].ToString();
                    o.entete = entete;
                    req_lire = dt_AZonglet.Rows[0]["req_lire"].ToString();
                    req_maj = dt_AZonglet.Rows[0]["req_maj"].ToString();
                    sql = "select c.id_AZtype_champ,c.entete,c.nom_champ,c.lg_champ,c.lg_champ_ecr,c.maj,c.oblig,c.visible,c.cbo_nom_tab_ref,c.cbo_req,c.cbo_filtre_lib,c.cbo_filtre_id from AZchamp_onglet co inner join AZchamp c on co.id_AZchamp=c.id_AZchamp where co.id_AZonglet=" + Formater(id_AZonglet) + " order by co.num_champ";
                    DataTable dt_champs = await ab.LireTable(sql);
                    foreach (DataRow dr in dt_champs.Rows)
                    {
                        AZTypeDeChamp tc = (AZTypeDeChamp)Convert.ToInt32(dr["id_AZtype_champ"].ToString());
                        entete = dr["entete"].ToString();
                        if (entete == "Début")
                        {
                            int a = 0;
                        }
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
                        AZChamp ch = new AZChamp(tc, entete, nom_champ, lg_champ, lg_champ_ecr, cbo_nom_tab_ref, cbo_req, cbo_filtre_id, cbo_filtre_lib, maj_champ, oblig, visible);
                        o.bd.lc.Add(ch);
                    }
                }
                o.req_lire = req_lire;
                o.req_maj = req_maj;
                m_onglets.Add(o);
                debut = false;
            }
            if (sauver)
            {
                ret = await MemoriserDefinitionEcran();
            }
            ret = true;
            return ret;
        }
        protected override bool InitIHM()
        {
            bool ret = base.InitIHM();
            LabelStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter{Property=Label.FontSizeProperty,Value=20}
                }
            };
            gspl.IsVisible = true;
            foreach (AZOnglet o in m_onglets)
            {
                o.InitIHM();
                this.recherche_nb.Text = "init2: " + o.bd.nom_table_bloc;
                o.btn.m_bouton.Clicked += btnonglet_Clicked;
                o.btn.m_bouton.ClassId = o.nom_onglet;
                btnonglets.Children.Add(o.btn);
                Grid.SetRow(o.bd.sv, 1);
                onglets.Children.Add(o.bd.sv);
            }
            gspl.m_p = this;
            ret = true;
            return ret;
        }
        private async void ActiverChampsOngletZero(bool activer)
        {
            if (activer == false && m_onglet_actif == m_onglets[0].nom_onglet)
            {
                await AfficherDetail();
            }
            foreach (AZChamp c in m_onglets[0].bd.lc)
            {
                if (c.visible && c.champ_saisie != null)
                    c.champ_saisie.IsEnabled = activer;
            }
        }
        private void ReinitialiserBoutons()
        {
            foreach (AZOnglet o in m_onglets)
                o.btn.m_bouton.Text = o.bd.header;
            m_objet_courant_modifie = false;
        }
        private void EffacerOnglets()
        {
            foreach (AZOnglet o in m_onglets)
            {
                o.bd.sv.IsVisible = false;
            }
        }
        protected async void btnrecherche_Clicked(object sender, EventArgs e)
        {
            try
            {
                m_onglet_actif = "";
                await Rechercher();
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
        }
        protected void btnvider_crit_Clicked(object sender, EventArgs e)
        {
            m_criteres_recherche.dg.ItemsSource = null;
            GenererCriteres();
            m_criteres_recherche.dg.ItemsSource = m_dtcriteres.Rows;
        }
        private AZOnglet TrouverOnglet()
        {
            AZOnglet onglet = null;
            foreach (AZOnglet o in m_onglets)
            {
                if (o.nom_onglet == m_onglet_actif)
                    onglet = o;
            }
            return onglet;
        }
        //        private async Task<bool> AfficherDetail()
        protected override async Task<bool> AfficherDetail()
        {
            bool ret = await base.AfficherDetail();
            EffacerMessage();
            EffacerOnglets();
            if (m_id_courant > 0 && m_onglet_actif.Length > 0)
            {
                AZOnglet o = TrouverOnglet();
                string nom_btn = o.btn.m_bouton.Text;
                o.bd.sv.IsVisible = true;
                string sql_tmp = o.bd.req_lire.Replace("@" + m_nom_cle_primaire, m_id_courant.ToString());
                AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                o.bd.dt = await ab.LireTable(sql_tmp);
                o.bd.dt.TableName = o.bd.nom_table_bloc;
                o.bd.dt_suppr = o.bd.dt.Clone();
                if (m_onglets[0].nom_onglet == m_onglet_actif)
                {
                    // detail principal
                    if (o.bd.dt.Rows.Count == 0)
                    {
                        DataRow dr = o.bd.dt.NewRow();
                        o.bd.dt.Rows.Add(dr);
                    }
                    DataRow dronglet = o.bd.dt.Rows[0];
                    foreach (AZChamp c in o.bd.lc)
                    {
                        if (c.visible)
                        {
                            switch (c.type)
                            {
                                case AZTypeDeChamp.Booleen:
                                    ((Switch)c.champ_saisie).IsToggled = false;
                                    if (dronglet[c.NomChampBD()].ToString().Length > 0)
                                    {
                                        ((Switch)c.champ_saisie).IsToggled = ConvertirBool(dronglet[c.NomChampBD()].ToString());
                                    }
                                    break;
                                case AZTypeDeChamp.ClePrimaire:
                                case AZTypeDeChamp.ClePrimairePrincipale:
                                    break;
                                case AZTypeDeChamp.Combobox:
                                    string test = c.NomChampBD() + "WITH";
                                    bool trouve = false;
                                    foreach (AZChamp c_test in o.bd.lc)
                                    {
                                        if (c_test.NomChampBD() == test)
                                            trouve = true;
                                    }
                                    if (trouve)
                                    {
                                        string val_string = dronglet[c.NomChampBD() + "WITH"].ToString();
                                        ((AZComboCS)c.champ_saisie).CboLib = val_string;
                                    }
                                    else
                                    {
                                        if (dronglet[c.NomChampBD()].ToString().Length > 0)
                                        {
                                            int? val_int = Convert.ToInt32(dronglet[c.NomChampBD()].ToString());
                                            ((AZComboCS)c.champ_saisie).CboId = val_int;
                                        }
                                    }
                                    break;
                                case AZTypeDeChamp.Entier:
                                    ((Entry)c.champ_saisie).Text = dronglet[c.NomChampBD()].ToString();
                                    break;
                                case AZTypeDeChamp.Date:
                                    if (dronglet[c.NomChampBD()].ToString().Length > 0)
                                        ((DatePicker)c.champ_saisie).Date = Convert.ToDateTime(dronglet[c.NomChampBD()].ToString());
                                    break;
                                case AZTypeDeChamp.Double:
                                    ((Entry)c.champ_saisie).Text = dronglet[c.NomChampBD()].ToString();
                                    break;
                                case AZTypeDeChamp.Guid:
                                    break;
                                case AZTypeDeChamp.Texte:
                                    if (!c.NomChampBD().EndsWith("WITH"))
                                        ((Entry)c.champ_saisie).Text = dronglet[c.NomChampBD()].ToString();
                                    break;
                                default:
                                    await AfficherMessage("type de champ inconnu dans l'onglet " + o.bd.header);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // autres onglets
                    o.bd.dg.ItemsSource = null;
                    o.bd.dg.ItemsSource = o.bd.dt.Rows;
                }
                o.btn.m_bouton.Text = nom_btn;
            }
            return true;
        }
        protected override async Task<string> Sauver()
        {
            string ret = await base.Sauver();
            string separateur = "&";
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            AZOnglet o = TrouverOnglet();
            if (m_onglets[0].nom_onglet == m_onglet_actif)
            {
                // detail principal
                DataRow dronglet = o.bd.dt.Rows[0];
                bool val_bd1 = false;
                bool val_bd2 = false;
                bool modifie = false;
                foreach (AZChamp c in o.bd.lc)
                {
                    if (c.visible)
                    {
                        switch (c.type)
                        {
                            case AZTypeDeChamp.Booleen:
                                val_bd1 = false;
                                if (dronglet[c.NomChampBD()].ToString().Length > 0)
                                    val_bd1 = Convert.ToBoolean(dronglet[c.NomChampBD()].ToString());
                                val_bd2 = ((Switch)c.champ_saisie).IsToggled;
                                if (val_bd1 != val_bd2)
                                {
                                    dronglet[c.NomChampBD()] = val_bd2;
                                    modifie = true;
                                }
                                break;
                            case AZTypeDeChamp.ClePrimaire:
                            case AZTypeDeChamp.ClePrimairePrincipale:
                                break;
                            case AZTypeDeChamp.Combobox:
                                if (dronglet[c.NomChampBD() + "WITH"].ToString() != ((AZComboCS)c.champ_saisie).CboLib)
                                {
                                    dronglet[c.NomChampBD()] = ((AZComboCS)c.champ_saisie).CboId;
                                    modifie = true;
                                }
                                break;
                            case AZTypeDeChamp.Entier:
                                if (dronglet[c.NomChampBD()].ToString() != ((Entry)c.champ_saisie).Text)
                                {
                                    dronglet[c.NomChampBD()] = Convert.ToInt32(((Entry)c.champ_saisie).Text);
                                    modifie = true;
                                }
                                break;
                            case AZTypeDeChamp.Date:
                                if (dronglet[c.NomChampBD()].ToString().Length > 0)
                                {
                                    if (Convert.ToDateTime(dronglet[c.NomChampBD()].ToString()) != ((DatePicker)c.champ_saisie).Date)
                                    {
                                        dronglet[c.NomChampBD()] = Convert.ToDateTime(((DatePicker)c.champ_saisie).Date);
                                        modifie = true;
                                    }
                                }
                                break;
                            case AZTypeDeChamp.Double:
                                if (dronglet[c.NomChampBD()].ToString() != ((Entry)c.champ_saisie).Text)
                                {
                                    dronglet[c.NomChampBD()] = Convert.ToDouble(((Entry)c.champ_saisie).Text);
                                    modifie = true;
                                }
                                break;
                            case AZTypeDeChamp.Guid:
                                break;
                            case AZTypeDeChamp.Texte:
                                if (dronglet[c.NomChampBD()].ToString() != ((Entry)c.champ_saisie).Text)
                                {
                                    dronglet[c.NomChampBD()] = ((Entry)c.champ_saisie).Text;
                                    modifie = true;
                                }
                                break;
                            default:
                                await AfficherMessage("type de champ inconnu dans l'onglet " + o.bd.header);
                                break;
                        }
                    }
                }
                if (modifie)
                {
                    o.bd.dt.Rows[0]["etat"] = "U";
                    DataTable dt = o.bd.dt.Copy();
                    ret = await ab.EcrireTable(o.bd.req_lire + separateur + o.bd.req_maj, dt);
                    o.btn.m_bouton.Text = o.bd.header;
                }
            }
            else
            {
                DataTable dt = o.bd.dt.Clone();
                foreach (DataRow dronglet in o.bd.dt.Rows)
                {
                    bool faire = true;
                    switch (dronglet.RowState)
                    {
                        case DataRowState.Added:
                            dronglet["etat"] = "I";
                            break;
                        case DataRowState.Deleted:
                            dronglet.RejectChanges();
                            dronglet["etat"] = "D";
                            break;
                        case DataRowState.Modified:
                            dronglet["etat"] = "U";
                            break;
                        default:
                            faire = false;
                            break;
                    }
                    if (faire)
                    {
                        dt.ImportRow(dronglet);
                    }
                }
                foreach (DataRow dronglet in o.bd.dt_suppr.Rows)
                {
                    //                        dronglet.Delete();
                    dronglet["tat"] = "D";
                    dt.ImportRow(dronglet);
                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn col = dt.Columns[i];
                        string nom_col = col.ColumnName;
                        bool garder = false;
                        for (int j = 0; j < o.bd.lc.Count && garder == false; j++)
                        {
                            AZChamp c = o.bd.lc[j];
                            string nom_champ = c.NomChampBD();
                            if (nom_col == nom_champ)
                                garder = c.maj;
                        }
                        if (!garder)
                        {
                            dt.Columns.RemoveAt(i--);
                        }
                    }
                    ret = await ab.EcrireTable(o.bd.req_lire + separateur + o.bd.req_maj, dt);
                    o.btn.m_bouton.Text = o.bd.header;
                }
                else
                    ret = "OK";
                o.bd.dt_suppr.Clear();
            }
            if (ret != "OK")
            {
                await AfficherMessage(ret);
            }
            else
            {
                bool ret_tmp = await AfficherDetail();
            }
            return ret;
        }
        protected async void btnsauver_Clicked(object sender, EventArgs e)
        {
            Attente(true);
            try
            {
                await Sauver();
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
        protected override async Task<string> Creer()
        {
            string ret = await base.Creer();
            AZOnglet o = TrouverOnglet();
            if (m_onglets[0].nom_onglet == m_onglet_actif)
            {
                // detail principal
                m_id_courant = -1;
                bool ret_tmp = await AfficherDetail();
            }
            else
            {
                o.bd.dg.ItemsSource = null;
                DataRow dronglet = o.bd.dt.NewRow();
                o.bd.dt.Rows.Add(dronglet);
                //                        drcab_form["etat"] = "I";
                dronglet[m_nom_cle_primaire] = m_id_courant;
                o.bd.dg.ItemsSource = o.bd.dt.Rows;

            }
            return ret;
        }
        protected async void btncreer_Clicked(object sender, EventArgs e)
        {
            Attente(true);
            try
            {
                await Creer();
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
        protected override async Task<string> Supprimer()
        {
            string ret = await base.Supprimer();
            AZOnglet o = TrouverOnglet();
            if (m_onglets[0].nom_onglet == m_onglet_actif)
            {
                // detail principal
            }
            else
            {
                object select = o.bd.dg.SelectedItem;
                if (select != null)
                {
                    o.bd.dg.ItemsSource = null;
                    DataRow dronglet = (DataRow)select;
                    o.bd.dt_suppr.ImportRow(dronglet);
                    int idx = o.bd.dt.Rows.IndexOf(dronglet);
                    o.bd.dt.Rows.RemoveAt(idx);
                    o.bd.dg.ItemsSource = o.bd.dt.Rows;
                }
            }
            return ret;
        }
        protected async void btnsupprimer_Clicked(object sender, EventArgs e)
        {
            Attente(true);
            try
            {
                string ret = await Supprimer();
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
        protected async void btnonglet_Clicked(object sender, EventArgs e)
        {
            Attente(true);
            try
            {
                Button btn = (Button)sender;
                string nom_onglet = btn.ClassId;
                if (nom_onglet == m_onglet_actif)
                    return;
                AZOnglet o;
                if (m_onglet_actif.Length > 0)
                {
                    o = TrouverOnglet();
                    o.btn.Relacher();
                }
                m_onglet_actif = nom_onglet;
                o = TrouverOnglet();
                o.btn.Appuyer();
                await AfficherDetail();
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
        private void ToucherOngletCourant(bool toucher)
        {
            AZOnglet o = TrouverOnglet();
            if (o != null)
            {
                AZBoutonOnglet bouton = o.btn;
                if (bouton != null)
                {
                    Button btn = o.btn.m_bouton;
                    if (btn != null)
                    {
                        string nom_btn = btn.Text;
                        bool deja_touche = nom_btn.EndsWith("*");
                        if (toucher)
                        {
                            if (!deja_touche)
                                btn.Text += "*";
                            m_objet_courant_modifie = true;
                        }
                        else
                        {
                            if (deja_touche)
                                btn.Text = btn.Text.Substring(0, btn.Text.Length - 1);
                        }
                    }
                }
            }
        }
        protected override async Task<bool> MemoriserDefinitionEcran()
        {
            bool ret = await base.MemoriserDefinitionEcran();
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            if (m_onglets != null)
            {
                for (int num_onglet = 0; num_onglet < m_onglets.Count; num_onglet++)
                {
                    AZOnglet o = m_onglets[num_onglet];
                    string sql = "select count(*) from AZonglet where id_AZecr=" + Formater(m_id_AZecr) + " and nom_table=" + Formater(o.bd.nom_table_bloc);
                    int? nb = await ab.LireEntier(sql);
                    if (nb == 0)
                    {
                        this.recherche_nb.Text = "onglet: " + o.bd.header;
                        int id_type_onglet = Convert.ToInt32(o.type_onglet);
                        sql = "exec AZongletCreer ";
                        sql += Formater(m_id_AZecr.ToString());
                        sql += "," + Formater(id_type_onglet);
                        sql += "," + Formater(num_onglet);
                        sql += "," + Formater(o.entete);
                        sql += "," + Formater(o.bd.nom_table_bloc);
                        sql += "," + Formater(o.bd.req_lire);
                        sql += "," + Formater(o.bd.req_maj);
                        sql += "," + Formater(o.bd.proc_maj);
                    }
                    int? id_AZonglet = await ab.LireEntier(sql);
                    for (int num_champ = 0; num_champ < o.bd.lc.Count; num_champ++)
                    {
                        AZChamp c = o.bd.lc[num_champ];
                        if (o.bd.header == "cab_lg" && c.header == "date_deroulage")
                        {
                            int a = 0;
                        }
                        sql = "select count(*) from AZchamp c inner join AZchamp_onglet co on c.id_AZchamp=co.id_AZchamp inner join AZonglet o on co.id_AZonglet=o.id_AZonglet where o.id_AZecr=" + Formater(m_id_AZecr) + " and o.nom_table=" + Formater(o.bd.nom_table_bloc) + " and c.nom_champ=" + Formater(c.NomChampBD());
                        nb = await ab.LireEntier(sql);
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
                            sql = "exec AZchamp_ongletCreer ";
                            sql += Formater(id_AZonglet.Value);
                            sql += "," + Formater(num_champ);
                            sql += "," + Formater(id_AZchamp.Value);
                            await ab.ExecSql(sql);
                        }
                        else
                        {
                            sql = "select c.id_AZchamp from AZchamp c inner join AZchamp_onglet co on c.id_AZchamp=co.id_AZchamp inner join AZonglet o on co.id_AZonglet=o.id_AZonglet where o.id_AZecr=" + Formater(m_id_AZecr) + " and o.nom_table=" + Formater(o.bd.nom_table_bloc) + " and c.nom_champ=" + Formater(c.NomChampBD());
                            int? id_AZchamp = await ab.LireEntier(sql);
                            sql = "update AZchamp set lg_champ_ecr=" + Formater(c.lg_champ_ecran) + " where id_AZchamp=" + Formater(id_AZchamp);
                            await ab.ExecSql(sql);
                        }
                    }
                }
            }
            return ret;
        }
        protected async void btnsauver_ecran_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool ret = await MemoriserDefinitionEcran();
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
        }
    }
}

