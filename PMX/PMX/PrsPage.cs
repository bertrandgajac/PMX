using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Controles;
using Xamarin.Forms;

namespace PMX
{
    class PrsPage : AZEcranComplexe
    {
        public PrsPage() : base()
        {
            string nom_serveur = "BERTRAND-PC\\BD_MSSQL";
            string nom_bd = "bcr";
            nom_bd = "gestion_pm";
            int mode_generation = 3;
            int lg_min_champ_ecran = 50;
            int lg_max_champ_ecran = 150;
            bool avec_redim = true;
            //            string nom_classe_ecran = "bcr";
            //            string nom_cle_primaire = "id_chassis";
            /*
                        List<string> lst_nom_tab = new List<string>();
                        lst_nom_tab.Add("chassis");
                        lst_nom_tab.Add("platine");
            */
            bool proc_avec_user = false;
            string nom_champ_etat = "Mode";
            /*
            List<string> lst_nom_tab = new List<string>() { "chassis", "platine" };
            string sql_recherche = "select @top * from v_chassis_recherche where 1=1 @critere";
            */
            //            List<string> lst_nom_tab = new List<string>() { "cab", "cab_form", "cab_comm", "cab_lg", "cab_itin_terme", "cab_itin_terme_d", "cab_itin_synt", "cab_itin", "cab_hist", "cab_fonct", "mcc", "cab_vol_feu_mcc", "cab_of", "cab_itin_terme_of" };
            List<string> lst_nom_tab = new List<string>() { "prs", "trv", "prs_off", "prs_doc" };
            //            string sql_recherche = "select @top T.id_prs,T.nom_prs,T.prenom_prs,T.id_loge,l.nom_loge as id_logeWITH,T.id_deg_bl,d.nom_deg as id_deg_blWITH,T.ad_elec,T.id_etat_prs,e.etat_prs as id_etat_prsWITH FROM prs T inner join loge l on T.id_loge=l.id_loge inner join deg d on T.id_deg_bl=d.id_deg inner join etat_prs e on T.id_etat_prs=e.id_etat_prs WHERE 1=1 @critere ORDER BY nom_prs,prenom_prs";

            string nom_ecran = "Membres";
            Init(nom_serveur, nom_bd, mode_generation, nom_ecran, lst_nom_tab, proc_avec_user, nom_champ_etat, lg_min_champ_ecran, lg_max_champ_ecran, avec_redim);
            //        MainPage = new NavigationPage(new Controles.AZEcranComplexe(nom_serveur,nom_bd,mode_generation,lst_nom_tab,sql_recherche,proc_avec_user,nom_champ_etat,lg_min_champ_ecran,lg_max_champ_ecran));
            /*
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            await ab.DefinirServeur(nom_serveur);
            await ab.DefinirBd(nom_bd);
            await Navigation.PushAsync(new PrsPage(nom_serveur, nom_bd, mode_generation, nom_ecran, lst_nom_tab, sql_recherche, proc_avec_user, nom_champ_etat, lg_min_champ_ecran, lg_max_champ_ecran, avec_redim));
            */
        }
        /*
        public override async Task<bool> Rechercher()
        {
            if (!((App)Application.Current).LoginFait())
            {
                await Navigation.PushAsync(new DebutPage());
            }
            else
            {
                bool ret = await base.Rechercher();
            }
            return true;
        }
        */
        /*
        public PrsPage(string _nom_serveur, string _nom_bd, int _mode_generation, string _nom_ecran, List<string> _lst_nom_tab, string _sql_recherche, bool _proc_avec_user, string _nom_champ_etat, int _lg_min_champ_ecran, int _lg_max_champ_ecran, bool _avec_redim)
        : base(_nom_serveur, _nom_bd, _mode_generation, _nom_ecran, _lst_nom_tab, _sql_recherche, _proc_avec_user, _nom_champ_etat, _lg_min_champ_ecran, _lg_max_champ_ecran, _avec_redim)
        {

        }
        */
        /*
        protected override string PreparerCriteres(DataRow dr)
        {
            string crit_sql = base.PreparerCriteres(dr);
            if(dr.Table.Columns.Contains("actif"))
            {
                string str_actif = dr["actif"].ToString();
                if (str_actif.Length > 0)
                {
                    bool actif = Convert.ToBoolean(str_actif);
                    if (actif)
                    {
                        crit_sql = crit_sql.Replace(" and actif = 1", " and id_etat_prs=1");
                    }
                    else
                    {
                        crit_sql = crit_sql.Replace(" and actif = 0", "");
                    }
                }
                else
                {
                    crit_sql = crit_sql.Replace(" and actif = 0", "");
                }
            }
            return crit_sql;
        }
        private async void Login()
        {
            if (!((App)Application.Current).LoginFait())
            {
                await Navigation.PushAsync(new DebutPage());
            }
        }
        */
    }
}
