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
    public partial class AZEcran : AZContentPage
    {
        //        private ContentPage m_p;
        protected string m_nom_serveur = "";
        protected string m_nom_bd = "";
        protected string m_nom_ecran;
        protected string m_lib_ecran;
        protected string m_nom_cle_primaire;
        protected List<string> m_lst_nom_tab;
        protected int m_mode_generation;
        protected bool m_proc_avec_user;
        protected string m_nom_champ_etat;
        protected int m_lg_min_champ_ecran;
        protected int m_lg_max_champ_ecran;
        protected string m_nom_table_principale;
        protected bool m_init_fait = false;
        protected DataTable m_dtattente;
        protected const double m_taille_combo = 30.0;
        protected bool m_objet_courant_modifie = false;
        protected const string m_nb_ligs = "nb_lig";
        protected double m_taille_textes;
        protected int m_id_AZecr;
        /*
        protected bool m_avec_redim = true;
        protected const int m_nb_max_champs_formulaire = 999;
        protected const int m_nb_max_champs_grille = 999;
        protected const bool m_avec_grid_splitter = false;
        protected const int m_largeur_grid_splitter = 20;
        protected AZBlocDonnees m_criteres_recherche;
        protected List<AZChampCritere> m_lc_criteres;
        protected List<AZOnglet> m_onglets;
        protected string m_sql_recherche;

        protected DataTable m_dtcriteres;
        protected string m_onglet_actif = "";
        protected int m_id_courant = 0;
        protected const int m_nb_max_lignes_recherche = 200;

        public List<AZChampCritere> lc_criteres { get { return m_lc_criteres; } set { m_lc_criteres = value; } }
        public DataTable dt_criteres { get { return m_dtcriteres; } }
        */
        public string nom_table_principale { get { return m_nom_table_principale; } set { m_nom_table_principale = value; } }
        public double taille_textes { get { return m_taille_textes; } }
        //        public ContentPage p { get { return m_p; } }
        public Style LabelStyle;

        public AZEcran()
        {

        }
        public virtual void Init(string _nom_serveur, string _nom_bd, int _mode_generation, string _nom_ecran, List<string> _lst_nom_tab, bool _proc_avec_user, string _nom_champ_etat, int _lg_min_champ_ecran, int _lg_max_champ_ecran, double taille_textes)
        {
            m_nom_serveur = _nom_serveur;
            m_nom_bd = _nom_bd;
            m_mode_generation = _mode_generation;
            m_nom_ecran = _nom_ecran;
            m_lst_nom_tab = _lst_nom_tab;
            //ABCD            m_sql_recherche = "exec AZ" + m_lst_nom_tab[0] + "__recherche 0 @criteres";
            m_proc_avec_user = _proc_avec_user;
            m_nom_champ_etat = _nom_champ_etat;
            m_lg_min_champ_ecran = _lg_min_champ_ecran;
            m_lg_max_champ_ecran = _lg_max_champ_ecran;
            m_nom_table_principale = _lst_nom_tab[0];
            //            m_avec_redim = _avec_redim;
            m_taille_textes = taille_textes;
        }
        protected void RechercheNb(string msg)
        {
            //            ((Label)this.FindByName("recherche_nb")).Text = msg;
            AZLabel el = (AZLabel)this.ChercherParNom("recherche_nb");
            if (el is AZLabel)
                el.Text = msg;
        }
        /*
        public virtual void Init(string _nom_serveur, string _nom_bd, int _mode_generation, string _nom_ecran, List<string> _lst_nom_tab, bool _proc_avec_user, string _nom_champ_etat, int _lg_min_champ_ecran, int _lg_max_champ_ecran, bool _avec_redim)
        {
            m_nom_serveur = _nom_serveur;
            m_nom_bd = _nom_bd;
            m_mode_generation = _mode_generation;
            m_nom_ecran = _nom_ecran;
            m_lst_nom_tab = _lst_nom_tab;
            //ABCD            m_sql_recherche = "exec AZ" + m_lst_nom_tab[0] + "__recherche 0 @criteres";
            m_proc_avec_user = _proc_avec_user;
            m_nom_champ_etat = _nom_champ_etat;
            m_lg_min_champ_ecran = _lg_min_champ_ecran;
            m_lg_max_champ_ecran = _lg_max_champ_ecran;
            m_nom_table_principale = _lst_nom_tab[0];
            m_avec_redim = _avec_redim;
        }
        */
        protected virtual async Task<bool> Init1()
        {
            bool ret = false;
            m_nom_cle_primaire = "id_" + m_lst_nom_tab[0];
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            string sql = "select count(*) from AZecr where nom_ecr=" + Formater(this.m_nom_ecran);
            int? nb_ecran = await ab.LireEntier(sql);
            if (nb_ecran.Value > 0)
            {
                //                lcr = new List<AZChamp>();
                sql = "select id_AZecr,lib_ecr from AZecr where nom_ecr=" + Formater(this.m_nom_ecran);
                DataTable dt_ecr = await ab.LireTable(sql);
                m_id_AZecr = Convert.ToInt32(dt_ecr.Rows[0]["id_AZecr"].ToString());
                m_lib_ecran = dt_ecr.Rows[0]["lib_ecr"].ToString();
            }
            ret = true;
            return ret;
        }
        protected virtual bool InitIHM()
        {
            bool ret = false;
            RechercheNb("init2");
            //                ret = await ab.Attendre();
            m_dtattente = new DataTable();
            m_dtattente.TableName = "attente";
            DataColumn dc;
            dc = new DataColumn("erreur", typeof(string));
            m_dtattente.Columns.Add(dc);
            DataRow dr = m_dtattente.NewRow();
            dr["erreur"] = " ";
            m_dtattente.Rows.Add(dr);
            ret = true;
            return ret;
        }
        public void AfficherTrace(string msg)
        {
            if (m_dtattente != null)
            {
                if (m_dtattente.Rows.Count > 0)
                {
                    m_dtattente.Rows[0]["erreur"] = DateTime.Now.ToString() + ": " + msg;
                    ListView dgattente = (ListView)this.FindByName("dgattente");
                    if (dgattente != null)
                    {
                        dgattente.ItemsSource = null;
                        dgattente.ItemsSource = m_dtattente.Rows;
                    }
                }
            }
        }
        protected async Task<bool> AfficherMessage(string msg)
        {
            bool trouve = false;
            bool fini = false;
            Element ma_page = this;
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
            AfficherTrace(msg);
            return true;
        }
        protected async Task<bool> AfficherException(Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg += " (" + ex.InnerException.Message + ")";
            }
            msg += ex.StackTrace;
            return await AfficherMessage(msg);
        }
        protected void EffacerMessage()
        {
            m_dtattente.Rows[0]["erreur"] = "";
            ListView dgattente = (ListView)this.FindByName("dgattente");
            if (dgattente != null)
            {
                dgattente.ItemsSource = null;
                dgattente.ItemsSource = m_dtattente.Rows;
            }
        }
        protected string ConvertirDateTimePourSql(DateTime dtt)
        {
            string ret = "";
            ret = dtt.Year.ToString().PadLeft(4, '0') + "-" + dtt.Month.ToString().PadLeft(2, '0') + "-" + dtt.Day.ToString().PadLeft(2, '0');
            if (dtt.Hour > 0 || dtt.Minute > 0 || dtt.Second > 0 || dtt.Millisecond > 0)
            {
                ret += " " + dtt.Hour.ToString().PadLeft(2, '0') + ":" + dtt.Minute.ToString().PadLeft(2, '0') + ":" + dtt.Second.ToString().PadLeft(2, '0') + "." + dtt.Millisecond.ToString().PadLeft(2, '0');
            }
            return ret;
        }
        protected bool ConvertirBool(string ch)
        {
            bool val = false;
            if (ch != null && ch.Length > 0)
            {
                val = Convert.ToBoolean(ch);
            }
            return val;
        }
        protected int? ConvertirInt32(string ch)
        {
            int? val = null;
            if (ch != null && ch.Length > 0)
            {
                val = Convert.ToInt32(ch);
            }
            return val;
        }
        public void Attente(bool debut)
        {
            ListView dgattente = (ListView)this.FindByName("dgattente");
            if (dgattente != null)
            {
                if (debut)
                    dgattente.BeginRefresh();
                else
                    dgattente.EndRefresh();
            }
        }
        protected async Task<bool> DemandeSauvegarde()
        {
            string titre = "Voulez-vous sauvegarder ?";
            string annuler = "Cancel";
            string[] ouinon = { "oui", "non" };
            string ret_clic = await this.DisplayActionSheet(titre, annuler, null, ouinon);
            bool ret = false;
            if (ret_clic != annuler)
                ret = (ret_clic == "oui");
            return ret;
        }
        protected void RendreBoutonVisible(string nom_bouton)
        {
            AZButton btn = (AZButton)this.FindByName(nom_bouton);
            if (btn != null)
            {
                ((AZButton)this.FindByName(nom_bouton)).Visible = true;
            }
        }
        protected virtual async Task<string> Sauver()
        {
            string ret = "";
            Attente(true);
            try
            {
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
            finally
            {
                Attente(false);
            }
            return ret;
        }
        protected virtual async Task<string> Creer()
        {
            string ret = "OK";
            Attente(true);
            try
            {
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
            finally
            {
                Attente(false);
            }
            return ret;
        }
        //        protected void btnsupprimer_Clicked(object sender, EventArgs e)
        protected virtual async Task<string> Supprimer()
        {
            string ret = "OK";
            Attente(true);
            try
            {
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
            finally
            {
                Attente(false);
            }
            return ret;
        }
        private async Task<DataRow> DonnerDataRow(object obj)
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
                await AfficherException(ex);
            }
            return r;
        }
        /*
        private void ModifEntry(string nom_champ, object sender)
        {
            string val = "";
            if (sender is Entry)
            {
                Entry c = (Entry)sender;
                val = c.Text;
            }
            else if (sender is AZComboCS)
            {
                AZComboCS c = (AZComboCS)sender;
                val = c.CboLib;
            }
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
        }
        private void ModifSwitch(string nom_champ, object sender)
        {
            bool val = false;
            if (sender is Switch)
            {
                Switch c = (Switch)sender;
                val = c.IsToggled;
            }
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
        }
        private void ModifDate(string nom_champ, object sender)
        {
            DateTime val = DateTime.MinValue;
            if (sender is DatePicker)
            {
                DatePicker c = (DatePicker)sender;
                val = c.Date;
            }
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
*/
        private GridLength ModifierGridLength(GridLength gl, bool reduire)
        {
            GridLength nouvelle_gl;
            if (gl.IsAbsolute)
            {
                double delta = reduire ? -10.0 : 10.0;
                double nouvelle_valeur = gl.Value + delta;
                if (nouvelle_valeur <= 1.0)
                    nouvelle_valeur = 1.0;
                nouvelle_gl = new GridLength(nouvelle_valeur);
            }
            else
            {
                nouvelle_gl = gl;
            }
            return nouvelle_gl;
        }
        private void DeplacerFrontiereHorizontale(bool vers_le_haut)
        {
            Grid LayoutRoot = ((Grid)this.FindByName("LayoutRoot"));
            double delta = vers_le_haut ? 10.0 : -10.0;
            GridLength haut1 = ModifierGridLength(LayoutRoot.RowDefinitions[3].Height, vers_le_haut);
            GridLength haut2 = ModifierGridLength(LayoutRoot.RowDefinitions[6].Height, !vers_le_haut);
            LayoutRoot.RowDefinitions[3].Height = haut1;
            LayoutRoot.RowDefinitions[6].Height = haut2;
        }
        protected void btnhaut_Clicked(object sender, EventArgs e)
        {
            DeplacerFrontiereHorizontale(true);
        }
        protected void btnbas_Clicked(object sender, EventArgs e)
        {
            DeplacerFrontiereHorizontale(false);
        }
        /*
        private async void btnvoir_plan_Clicked(object sender, EventArgs e)
        {
            try
            {
                string sql = "select doc from plan_acad where id_plan_acad=22";
                AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                byte[] contenu = await ab.LireBlob(sql);
                await Navigation.PushAsync(new VoirImagePage(contenu));
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }

        private async void btndefinir_plan_Clicked(object sender, EventArgs e)
        {
            try
            {
                Stream stream = await DependencyService.Get<IAccesAuxPhotos>().DonnerStreamVersPhotoAsync();
                AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                await ab.DefinirServeur("sbmrs10007");
                await ab.DefinirBd("bcr");
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                byte[] doc = ms.ToArray();
                bool ret = await ab.EcrireBlob("update plan_acad set doc=@blob where id_plan_acad=22", doc);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        
        private void SwToggled(object sender, ToggledEventArgs e)
        {
            try
            {
                Switch sw = (Switch)sender;
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
                Entry en = (Entry)sender;
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
                DatePicker dp = (DatePicker)sender;
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
        

        private ListView MaListViewFille(Element el)
        {
            ListView lv = null;
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
                Grid gr = (Grid)el;
                trouve = false;
                foreach (Element f in gr.Children)
                {
                    if (f is ListView)
                    {
                        trouve = true;
                        lv = (ListView)f;
                    }
                }
            }
            return lv;
        }
        
        private void BtnMoinsClicked(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                bool vers_la_gauche = true;
                DeplacerFrontiereVerticale(btn, vers_la_gauche);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }

        private void BtnPlusClicked(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                bool vers_la_gauche = false;
                DeplacerFrontiereVerticale(btn, vers_la_gauche);
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
        }
        
        private void DeplacerFrontiereVerticale(Button btn, bool vers_la_gauche)
        {
            int delta_largeur = vers_la_gauche ? -10 : 10;
            int largeur_mini = 50;
            Grid gr = MaGridGrandMere(btn);
            if (gr != null)
            {
                int num_col_btn = Convert.ToInt32(btn.ClassId);
                if (gr.ClassId == "grcriteres_entete")
                {
                    int num_col = 0;
                    foreach (AZChamp c in lc_criteres)
                    {
                        if (c.visible)
                        {
                            if (num_col_btn == num_col)
                            {
                                c.largeur_champ_ecran += delta_largeur;
                                if (c.largeur_champ_ecran < largeur_mini)
                                    c.largeur_champ_ecran = largeur_mini;
                            }
                            num_col++;
                        }
                    }
                    num_col = 0;
                    foreach (AZChamp c in lc_recherche)
                    {
                        if (c.visible)
                        {
                            if (num_col_btn == num_col)
                            {
                                c.largeur_champ_ecran += delta_largeur;
                            }
                            num_col++;
                        }
                    }
                    CreerGrilleCriteresEtRecherche();
                }
                else
                {
                    foreach (AZOnglet o in lo)
                    {
                        if (gr.ClassId == o.NomDataGridPourGrille())
                        {
                            int num_col = 0;
                            foreach (AZChamp c in o.bd.lc)
                            {
                                if (c.visible)
                                {
                                    if (num_col_btn == num_col)
                                    {
                                        c.largeur_champ_ecran += delta_largeur;
                                        if (c.largeur_champ_ecran < largeur_mini)
                                            c.largeur_champ_ecran = largeur_mini;
                                    }
                                    num_col++;
                                }
                            }
                            CreerGrilleOnglet(o);
                        }
                    }
                }
            }
        }
        public void DeplacerFrontiereVerticale(Grid gr, int num_col_grid_splitter, double delta_x)
        {
            if (gr != null)
            {
                int largeur_mini = 50;
                int num_col_a_modifier = num_col_grid_splitter - 1;
                int i_delta_x = (int)(delta_x * 5.0);
                if (gr.ClassId == "grcriteres_entete")
                {
                    int num_col = 0;
                    foreach (AZChamp c in m_criteres_recherche.lc_criteres)
                    {
                        if (c.visible)
                        {
                            if (num_col > 0)
                                num_col++;  // pour le grdisplitter
                            if (num_col_a_modifier == num_col)
                            {
                                c.largeur_champ_ecran += i_delta_x;
                                if (c.largeur_champ_ecran < largeur_mini)
                                    c.largeur_champ_ecran = largeur_mini;
                            }
                            num_col++;
                        }
                    }
                    num_col = 0;
                    foreach (AZChamp c in m_criteres_recherche.lc)
                    {
                        if (c.visible)
                        {
                            if (num_col > 0)
                                num_col++;  // pour le gridsplitter
                            if (num_col_a_modifier == num_col)
                            {
                                c.largeur_champ_ecran += i_delta_x;
                            }
                            num_col++;
                        }
                    }
                    //                    CreerGrilleCriteresEtRecherche();
                    m_criteres_recherche.InitIHM();
                }
                else
                {
                    foreach (AZOnglet o in m_onglets)
                    {
                        if (gr.ClassId == o.NomDataGridPourGrille())
                        {
                            int num_col = 0;
                            foreach (AZChamp c in o.bd.lc)
                            {
                                if (c.visible)
                                {
                                    if (num_col > 0)
                                        num_col++;  // pour le gridsplitter
                                    if (num_col_a_modifier == num_col)
                                    {
                                        c.largeur_champ_ecran += i_delta_x;
                                        if (c.largeur_champ_ecran < largeur_mini)
                                            c.largeur_champ_ecran = largeur_mini;
                                    }
                                    num_col++;
                                }
                            }
                            //                            CreerGrilleOnglet(o);
                            o.bd.InitIHM();
                        }
                    }
                }
            }
        }
        */
        protected static string Formater(string val)
        {
            string ret = "'" + val.Replace("'", "''") + "'";
            return ret;
        }
        protected static string Formater(int val)
        {
            string ret = val.ToString();
            return ret;
        }
        protected static string Formater(int? val)
        {
            string ret = val.HasValue ? val.Value.ToString() : "";
            return ret;
        }
        protected static string Formater(bool val)
        {
            string ret = val ? "1" : "0";
            return ret;
        }
        protected virtual async Task<bool> MemoriserDefinitionEcran()
        {
            RechercheNb("MemoriserDefinitionEcran");
            bool ret = false;
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            string sql = "select count(*) from AZecr where nom_ecr=" + Formater(m_nom_ecran);
            int? nb = await ab.LireEntier(sql);
            if (nb == 0)
            {
                sql = "insert into AZecr(nom_ecr) values (" + Formater(m_nom_ecran) + ")";
                await ab.ExecSql(sql);
            }
            sql = "select id_AZecr from AZecr where nom_ecr=" + Formater(m_nom_ecran);
            int? id_ecr = await ab.LireEntier(sql);
            m_id_AZecr = id_ecr.Value;
            foreach (string nom_tab in m_lst_nom_tab)
            {
                sql = "select count(*) from AZtab where id_AZecr=" + m_id_AZecr.ToString() + " and nom_tab=" + Formater(nom_tab);
                nb = await ab.LireEntier(sql);
                if (nb == 0)
                {
                    sql = "select isnull(max(num_tab),0) from AZtab where id_AZecr=" + m_id_AZecr.ToString();
                    int? num_tab = await ab.LireEntier(sql);
                    num_tab++;
                    sql = "insert into AZtab (id_AZecr,num_tab,nom_tab) values (" + id_ecr.Value.ToString() + "," + num_tab.ToString() + ",'" + nom_tab + "')";
                    ret = await ab.ExecSql(sql);
                }
            }
            return ret;
        }
        protected async void btnsortie_ecran_Clicked(object sender, EventArgs e)
        {
            try
            {
                var action = await this.DisplayActionSheet("Voulez-vous fermer l'écran ?", "Cancel", null, "Oui", "Non");
                if (action == "Oui")
                    await this.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
        }
    }
}