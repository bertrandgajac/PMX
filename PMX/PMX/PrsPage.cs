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
            bool proc_avec_user = false;
            string nom_champ_etat = "Mode";
            double taille_textes = 15.0;
            List<string> lst_nom_tab = new List<string>() { "prs", "trv", "prs_off", "prs_doc" };
            //            string sql_recherche = "select @top T.id_prs,T.nom_prs,T.prenom_prs,T.id_loge,l.nom_loge as id_logeWITH,T.id_deg_bl,d.nom_deg as id_deg_blWITH,T.ad_elec,T.id_etat_prs,e.etat_prs as id_etat_prsWITH FROM prs T inner join loge l on T.id_loge=l.id_loge inner join deg d on T.id_deg_bl=d.id_deg inner join etat_prs e on T.id_etat_prs=e.id_etat_prs WHERE 1=1 @critere ORDER BY nom_prs,prenom_prs";

            string nom_ecran = "Membres";
            Init(nom_serveur, nom_bd, mode_generation, nom_ecran, lst_nom_tab, proc_avec_user, nom_champ_etat, lg_min_champ_ecran, lg_max_champ_ecran, taille_textes);
            //        MainPage = new NavigationPage(new Controles.AZEcranComplexe(nom_serveur,nom_bd,mode_generation,lst_nom_tab,sql_recherche,proc_avec_user,nom_champ_etat,lg_min_champ_ecran,lg_max_champ_ecran));
        }
        public override string PreparerSqlPourComboboxDetail(AZComboCS cbo)
        {
            string sql = "";
            AZGrid g = (AZGrid)cbo.Parent;
            AZChamp champ = cbo.champ;
            string nom_onglet = champ.bloc_donnees.nom_table_bloc;
            switch(nom_onglet)
            {
                case "prs":
                    break;
                case "trv":
                    switch (champ.nom_champ)
                    {
                        case "id_tenue":
                            AZComboCS cboid_loge = (AZComboCS)g.Children[1];
                            int? id_loge = cboid_loge.CboId;
                            if (id_loge.HasValue)
                            {
                                sql = cbo.base_req.Replace("1=1", "1=1 and id_loge=" + id_loge.Value.ToString());
                            }
                            break;
                    }
                    break;
                case "prs_off":
                    switch (champ.nom_champ)
                    {
                        case "id_tenue_deb":
                            AZComboCS cboid_loge = (AZComboCS)g.Children[1];
                            int? id_loge = cboid_loge.CboId;
                            if(id_loge.HasValue)
                            {
                                sql = cbo.base_req.Replace("1=1", "1=1 and id_loge=" + id_loge.Value.ToString());
                            }
                            break;
                        case "id_tenue_fin":
                            AZComboCS cboid_tenue_deb = (AZComboCS)g.Children[3];
                            int? id_tenue_deb = cboid_tenue_deb.CboId;
                            if (id_tenue_deb.HasValue)
                            {
                                sql = cbo.base_req.Replace("1=1", "1=1 and id_tenue in (select t1.id_tenue from tenue t1 inner join tenue t2 on t1.id_loge=t2.id_loge and t1.date_tenue>t2.date_tenue and t2.id_tenue=" + id_tenue_deb.Value.ToString() + ")");
                            }
                            break;
                    }
                    break;
                case "prs_doc":
                    break;
            }
            return sql;
        }
    }
}
