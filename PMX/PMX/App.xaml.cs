using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PMX
{
    public partial class App : Application
    {
        private int m_id_prs = 0;
        private double m_taille_textes = 15.0;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MenuPage());
        }
        public void SpecifierIdPrs(int id_prs)
        {
            if (id_prs > 0)
            {
                m_id_prs = id_prs;
            }
        }
        public int IdPrsUtilisateur { get { return m_id_prs; } }
        public bool LoginFait { get { return m_id_prs > 0; } }
        public double TailleTextes { get { return m_taille_textes; } }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
