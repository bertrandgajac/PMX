using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using Controles;

namespace PMX
{
    public class DebutPage : AZDebutPage
    {
        public DebutPage() : base()
        {
        }
        protected override void Init()
        {
            base.Init();
            cboid_prs.base_req = "select @top id_prs as @id,dbo.fct_rep('prs',id_prs) as @lib from prs where 1=1 and id_etat_prs in (select id_etat_prs from etat_prs where actif=1) order by 2";
        }
        protected override void MemoriserPrs()
        {
            base.MemoriserPrs();
            ((App)Application.Current).SpecifierIdPrs(m_id_prs);
        }
    }
}