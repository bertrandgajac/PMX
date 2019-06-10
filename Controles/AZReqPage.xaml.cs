using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AZReqPage : AZEcranRecherche
    {
        private AZBoutonOnglet m_btnstd;
        private AZBoutonOnglet m_btnspec;
        private AZBoutonOnglet m_btnsauver_ecran;
        private List<ReqCrit> m_liste_crit;
        private string m_req_sql;

        public AZReqPage() : base()
        {
            InitializeComponent();
        }
        private async void Btninit_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool ret = await Init1();
                if (ret)
                {
                    nom_grid_criteres_recherche = "tbstd";
                    num_row_criteres_recherche = 1;
                    ret = InitIHM();
                    if (ret)
                    {
                        m_init_fait = true;
                        btninit.IsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await AfficherException(ex);
            }
        }
        protected override bool InitIHM()
        {
            bool ret = base.InitIHM();
            tbstd.IsVisible = false;
            //            bool ret = base.InitIHM();
            m_btnstd = new AZBoutonOnglet("Standard");
            m_btnstd.m_bouton.Clicked += Btnstd_Clicked;
            slbtn.Children.Add(m_btnstd);
            m_btnspec = new AZBoutonOnglet("Spécifique");
            m_btnspec.m_bouton.Clicked += Btnspec_Clicked;
            slbtn.Children.Add(m_btnspec);
            m_btnsauver_ecran = new AZBoutonOnglet("+");
            m_btnsauver_ecran.m_bouton.Clicked += Btnsauver_ecran_Clicked;
            slbtn.Children.Add(m_btnsauver_ecran);
            /*
            LabelStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter{Property=Label.FontSizeProperty,Value=20}
                }
            };
            */
            return true;
        }
        private void Btnstd_Clicked(object sender, EventArgs e)
        {
            this.tbstd.IsVisible = true;
            this.tbspec.IsVisible = false;
            m_btnstd.Appuyer();
            m_btnspec.Relacher();
        }
        private void Btnspec_Clicked(object sender, EventArgs e)
        {
            this.tbstd.IsVisible = false;
            this.tbspec.IsVisible = true;
            m_btnstd.Relacher();
            m_btnspec.Appuyer();
        }
        private async void Btnsauver_ecran_Clicked(object sender, EventArgs e)
        {
            bool ret = await MemoriserDefinitionEcran();
        }
        protected async void btnrecherche_Clicked(object sender, EventArgs e)
        {
            try
            {
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
        //        private async Task<bool> AfficherDetail()
        protected override async Task<bool> AfficherDetail()
        {
            //            bool ret = await base.AfficherDetail();
            EffacerMessage();
            DataRow dr = (DataRow)m_criteres_recherche.dg.SelectedItem;
            m_req_sql = dr["req_sql"].ToString();
            string sql = "exec AZreq__req_critSelect " + m_id_courant.ToString();
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            DataTable dt_crit = await ab.LireTable(sql);
            int num_lig = 0;
            m_liste_crit = new List<ReqCrit>();
            double taille_textes = 20.0;
            foreach (DataRow dr_crit in dt_crit.Rows)
            {
                grcriteres_req.RowDefinitions.Add(new RowDefinition() { Height = grcriteres_req.DonnerGridLength(40, AZGridUnitType.Absolute) });
                int id_req_crit = Convert.ToInt32(dr_crit["id_req_crit"].ToString());
                int id_type_req_crit = Convert.ToInt32(dr_crit["id_type_req_crit"].ToString());
                string nom_type_req_crit = dr_crit["id_type_req_critWITH"].ToString();
                string nom_req_crit = dr_crit["nom_req_crit"].ToString();
                int num_req_crit = Convert.ToInt32(dr_crit["num_req_crit"].ToString());
                string req_crit_sql = dr_crit["req_crit_sql"].ToString();
                string nom_tab_si_combo = dr_crit["nom_tab_si_combo"].ToString();
                ReqCrit rc = new ReqCrit(id_req_crit, id_type_req_crit, num_req_crit, nom_req_crit, req_crit_sql, nom_tab_si_combo);
                m_liste_crit.Add(rc);
                AZLabel lblnom_crit = new AZLabel();
                Grid.SetColumn(lblnom_crit, 0);
                Grid.SetRow(lblnom_crit, num_lig);
                lblnom_crit.Text = nom_req_crit;
                grcriteres_req.Children.Add(lblnom_crit);
                switch (nom_type_req_crit.ToLower())
                {
                    case "booleen":
                        AZSwitch sw = new AZSwitch();
                        Grid.SetColumn(sw, 1);
                        Grid.SetRow(sw, num_lig);
                        sw.ClassId = num_req_crit.ToString();
                        grcriteres_req.Children.Add(sw);
                        break;
                    case "combo":
                        string titre = num_req_crit.ToString();
                        string nom_tab = dr_crit["nom_tab_si_combo"].ToString();
                        string base_req = "select id_" + nom_tab + " as @id,dbo.fct_rep('" + nom_tab + "',id_" + nom_tab + ") as @lib from " + nom_tab + " order by 2";
                        string base_filtre_lib = " and dbo.fct_rep('" + nom_tab + "',id_" + nom_tab + ") like '@valeur'";
                        string base_filtre_id = " and id_" + nom_tab + "=@valeur";
                        AZComboCS cbo = new AZComboCS((AZChamp)null, titre, base_req, base_filtre_lib, base_filtre_id);
                        Grid.SetColumn(cbo, 1);
                        Grid.SetRow(cbo, num_lig);
                        cbo.ClassId = num_req_crit.ToString();
                        //                        cbo.taille_texte = taille_textes;
                        grcriteres_req.Children.Add(cbo);
                        break;
                    case "double":
                    case "entier":
                    case "chaine":
                        AZEntry en = new AZEntry();
                        Grid.SetColumn(en, 1);
                        Grid.SetRow(en, num_lig);
                        en.ClassId = num_req_crit.ToString();
                        en.FontSize = taille_textes;
                        grcriteres_req.Children.Add(en);
                        break;
                    case "date":
                        AZDatePicker dp = new AZDatePicker();
                        Grid.SetColumn(dp, 1);
                        Grid.SetRow(dp, num_lig);
                        dp.ClassId = num_req_crit.ToString();
                        dp.FontSize = taille_textes;
                        grcriteres_req.Children.Add(dp);
                        break;
                }
                num_lig++;
            }
            return true;
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
        private async void Btnexec_req_Clicked(object sender, EventArgs e)
        {
            try
            {
                string chaine_criteres = "";
                int nb_crit = m_liste_crit.Count;
                int nb_fils_crit = grcriteres_req.Children.Count;
                for (int i = 0; i < nb_crit; i++)
                {
                    ReqCrit un_crit = m_liste_crit[i];
                    string val = null;
                    for (int j = 0; j < nb_fils_crit; j++)
                    {
                        View un_el = grcriteres_req.Children[j];
                        string Uid = un_el.ClassId;
                        if (Uid != null && Uid.Length > 0)
                        {
                            int num_crit = Convert.ToInt32(Uid);
                            if (num_crit == un_crit.m_num_req_crit)
                            {
                                if (un_el is AZSwitch)
                                {
                                    AZSwitch sw = (AZSwitch)un_el;
                                    val = sw.IsToggled ? "1" : "0";
                                }
                                else if (un_el is AZComboCS)
                                {
                                    AZComboCS cbo = (AZComboCS)un_el;
                                    int? id = cbo.CboId;
                                    if (id.HasValue)
                                    {
                                        val = id.Value.ToString();
                                    }
                                }
                                else if (un_el is AZEntry)
                                {
                                    AZEntry tb = (AZEntry)un_el;
                                    val = tb.Text;
                                }
                                else if (un_el is AZDatePicker)
                                {
                                    AZDatePicker dp = (AZDatePicker)un_el;
                                    val = dp.Date.ToString();
                                }
                                if (val != null)
                                {
                                    string req_crit_sql = un_crit.m_req_crit_sql.Replace("@valeur", val);
                                    chaine_criteres += req_crit_sql;
                                }
                            }
                        }
                    }
                }
                if (m_req_sql.EndsWith("@critere"))
                    m_req_sql += "s";
                string req_sql = m_req_sql.Replace("@criteres", chaine_criteres);
                bool ret = await AfficherResultat(req_sql, 0);
            }
            catch (System.Exception ex)
            {
                await AfficherException(ex);
            }
        }
        private async Task<bool> AfficherResultat(string sql, int num)
        {
            try
            {
                AZBlocDonnees bd = new AZBlocDonnees(this, (AZOnglet)null, AZTypeBlocDonnees.Grille, "requete");
                /*
                AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                DataTable dt_resultat = await ab.LireTable(sql); // ab.LireEnsembleDeTables(sql);
                bd.dt = dt_resultat;
                */
                await bd.GenererChamps(sql);
                bd.InitIHM();

                Grid grresultat = (Grid)bd.sv.Content;
                if (num == 0)
                    svreq_resultat.Content = grresultat;
                else
                    svreq_spec_resultat.Content = grresultat;
                //                bd.dg.ItemsSource = null;
                /*
                                if (num == 0)
                                {
                                    // onglet standard
                                    tbstd.IsVisible = false;
                                    Grid.SetRow(bd.sv, 5);
                                    tbstd.Children.Add(bd.sv);
                                    tbstd.IsVisible = true;
                                }
                                else
                                {
                                    // onglet spécifique
                                    Grid.SetRow(bd.sv, 2);
                                    tbspec.Children.Add(bd.sv);
                                }
                //                bd.dg.ItemsSource = dt_resultat.Rows;
                */
                /*
                Grid grresultat = (Grid)bd.sv.Content;
                if (num == 0)
                    svreq_resultat.Content = grresultat;
                else
                    svreq_spec_resultat.Content = grresultat;
                */
                /*
                int nb_tabs = resultat.Tables.Count;
                TabItem onglet = null;
                //                string xml = resultat.Donnees.GetXml();
                string nom_onglet = chaine_criteres.Replace(" ", "_").Replace("'", "_");
                TabControl tc = num == 0 ? this.tabs2 : this.tabs;
                for (int i = 0; i < nb_tabs; i++)
                {
                    int nb_onglets = 0;
                    int num_suffixe = 0;
                    / *
                    if (num == 0)
                    {
                        nb_onglets = this.tabs2.Items.Count;
                        foreach(TabItem it_tmp in this.tabs2.Items)
                        {
                            if (it_tmp.Name.StartsWith(nom_onglet))
                            {
                                num_suffixe++;
                            }
                        }
                    }
                    else
                    {
                        nb_onglets = this.tabs.Items.Count;
                        foreach (TabItem it_tmp in this.tabs.Items)
                        {
                            if (it_tmp.Name.StartsWith(nom_onglet))
                            {
                                num_suffixe++;
                            }
                        }
                    }
                    * /
                    nb_onglets = tc.Items.Count;
                    foreach (TabItem it_tmp in tc.Items)
                    {
                        if (it_tmp.Name.StartsWith(nom_onglet))
                        {
                            num_suffixe++;
                        }
                    }
                    string nom_tmp = nom_onglet;
                    if (num_suffixe > 0)
                    {
                        nom_tmp += "_" + num_suffixe.ToString();
                    }
                    int num_onglet = nb_onglets + 1;
                    resultat.Tables[i].TableName = num_onglet.ToString();
                    onglet = new TabItem();
                    //                    onglet.Header = num_onglet.ToString();
                    onglet.Header = nom_tmp;
                    onglet.Name = nom_tmp;
                    DataGrid dgres = new DataGrid();
                    dgres.ItemsSource = resultat.Tables[i].DefaultView;
                    dgres.CanUserAddRows = false;
                    dgres.CanUserDeleteRows = false;
                    onglet.Content = dgres;
                    onglet.AllowDrop = true;
                    if (num == 0)
                    {
                        this.tabs2.Items.Add(onglet);
                    }
                    else
                    {
                        this.tabs.Items.Add(onglet);
                    }
                    onglet.Visibility = System.Windows.Visibility.Visible;
                }
                if (onglet != null)
                {
                    if (num == 0)
                    {
                        this.tabs2.SelectedItem = onglet;
                    }
                    else
                    {
                        this.tabs.SelectedItem = onglet;
                    }
                }
                //                this.tabs.Items[nb_tabs-1].
                / *
                tab = acces_bd.LireTout(this.txtsql.Text);
                this.dgresultat.ItemsSource = tab.Donnees.DefaultView;
                 * /
                 */
            }
            catch (System.Exception ex)
            {
                await AfficherException(ex);
            }
            return true;
        }
        private async void Btnexec_req_spec_Clicked(object sender, EventArgs e)
        {
            try
            {
                string req_sql = this.txtsql_spec.Text;
                bool ret = await AfficherResultat(req_sql, 1);
            }
            catch (System.Exception ex)
            {
                await AfficherException(ex);
            }
        }
        /*ABCD
protected async void btnsortie_ecran_Clicked(object sender, EventArgs e)
{
    try
    {
        var action = await DisplayActionSheet("Voulez-vous fermer l'écran ?", "Cancel", null, "Oui", "Non");
        if (action == "Oui")
            await Navigation.PopAsync();
    }
    catch (Exception ex)
    {
        AfficherException(ex);
    }
}
*/
    }
    public class ReqCrit
    {
        public int m_id_req_crit;
        public int m_id_type_req_crit;
        public int m_num_req_crit;
        public string m_nom_req_crit;
        public string m_req_crit_sql;
        public string m_req_sql_si_combo;

        public ReqCrit(int id_req_crit, int id_type_req_crit, int num_req_crit, string nom_req_crit, string req_crit_sql, string req_sql_si_combo)
        {
            m_id_req_crit = id_req_crit;
            m_id_type_req_crit = id_type_req_crit;
            m_num_req_crit = num_req_crit;
            m_nom_req_crit = nom_req_crit;
            m_req_crit_sql = req_crit_sql;
            m_req_sql_si_combo = req_sql_si_combo;
        }
    }
}

