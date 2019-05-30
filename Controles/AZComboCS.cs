using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    public class ItemCbo
    {
        public int id { get; set; }
        public string lib { get; set; }
    }
    public class CboLibChangedEventArgs : EventArgs
    {
        public CboLibChangedEventArgs(string _ancienne_valeur, string _nouvelle_valeur)
        {
            ancienne_valeur = _ancienne_valeur;
            nouvelle_valeur = _nouvelle_valeur;
        }
        public string ancienne_valeur { get; set; }
        public string nouvelle_valeur { get; set; }
    }
    public class CboIdChangedEventArgs : EventArgs
    {
        public CboIdChangedEventArgs(int? _ancienne_valeur, int? _nouvelle_valeur)
        {
            ancienne_valeur = _ancienne_valeur;
            nouvelle_valeur = _nouvelle_valeur;
        }
        public int? ancienne_valeur { get; set; }
        public int? nouvelle_valeur { get; set; }
    }
    public class CboLargeurChangedEventArgs : EventArgs
    {
        public CboLargeurChangedEventArgs(double? _ancienne_valeur, double? _nouvelle_valeur)
        {
            ancienne_valeur = _ancienne_valeur;
            nouvelle_valeur = _nouvelle_valeur;
        }
        public double? ancienne_valeur { get; set; }
        public double? nouvelle_valeur { get; set; }
    }
    public class AZComboUtil
    {
        private string nom_col_nb_ligs;
        private string nom_col_id;
        private string nom_col_lib;
        public bool meme_liste;
        public bool liste_complete;
        public bool liste_initialisee;
        public int nb_total_items;

        public AZComboUtil(string _nom_col_nb_ligs, string _nom_col_id, string _nom_col_lib)
        {
            nom_col_nb_ligs = _nom_col_nb_ligs;
            nom_col_id = _nom_col_id;
            nom_col_lib = _nom_col_lib;
        }
        public string PreparerReq(string req)
        {
            string req_preparee = req;
            if (req_preparee.Contains("@top"))
            {
                req_preparee = req_preparee.Replace("@top", "top 200 count(*) over() as " + nom_col_nb_ligs + ",");
                if (req_preparee.EndsWith("order by 2"))
                {
                    req_preparee = req_preparee.Replace("order by 2", "order by 3");
                }
            }
            if (req_preparee.Contains("@id"))
            {
                req_preparee = req_preparee.Replace("@id", nom_col_id);
            }
            if (req_preparee.Contains("@lib"))
            {
                req_preparee = req_preparee.Replace("@lib", nom_col_lib);
            }
            return req_preparee;
        }
        public async Task<List<ItemCbo>> InitialiserListe(string req, string filtre_id, string filtre_lib, int? val_filtre_id, string val_filtre_lib)
        {
            List<ItemCbo> liste = new List<ItemCbo>();
            meme_liste = true;
            liste_complete = false;
            //            Sablier(true);
            meme_liste = false;
            req = PreparerReq(req);
            liste_complete = true;
            if (val_filtre_lib != null && val_filtre_lib.Length > 0)
            {
                req = req.Replace("1=1", "1=1" + filtre_lib.Replace("@valeur", val_filtre_lib));
                liste_complete = false;
            }
            if (val_filtre_id.HasValue)
            {
                req = req.Replace("1=1", "1=1" + filtre_id.Replace("@valeur", val_filtre_id.Value.ToString()));
                liste_complete = false;
            }
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            DataTable dt = await ab.LireTable(req);
            if (dt != null)
            {
                int nb_ligs = dt.Rows.Count;
                int nb_total = nb_ligs;
                if (nb_ligs > 0)
                {
                    if (dt.Columns.Contains(nom_col_nb_ligs))
                    {
                        nb_total = Convert.ToInt32(dt.Rows[0][nom_col_nb_ligs]);
                    }
                }
                if (nb_ligs < nb_total)
                {
                    liste_complete = false;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    ItemCbo it = new ItemCbo();
                    it.id = Convert.ToInt32(dr[nom_col_id].ToString());
                    it.lib = dr[nom_col_lib].ToString();
                    liste.Add(it);
                }
                liste_initialisee = true;
                nb_total_items = nb_total;
            }
            /*
            else
            {
                AfficherErreur("Aucune valeur disponible");
            }
            Sablier(false);
            */
            return liste;
        }
        public Page MaPage(Element el)
        {
            bool trouve = false;
            bool fini = false;
            Element ma_page = el.Parent;
            while (!fini)
            {
                if (ma_page is Page)
                {
                    trouve = true;
                    fini = true;
                }
                else if (ma_page == null)
                    fini = true;
                else
                {
                    ma_page = ma_page.Parent;
                }
            }
            if (!trouve)
            {
                ma_page = null;
            }
            return (Page)ma_page;
        }
        public ListView MaListView(Element el)
        {
            bool trouve = false;
            bool fini = false;
            while (!fini)
            {
                if (el is ListView)
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
            return (ListView)el;
        }
        public void Sablier(bool afficher, Element el)
        {
            //            ListView sv = MaListView();
            AZComboUtil util = new AZComboUtil(nom_col_nb_ligs, nom_col_id, nom_col_lib);
            ListView lv = util.MaListView(el);
            if (lv != null)
            {
                if (afficher)
                    lv.BeginRefresh();
                else
                    lv.EndRefresh();
            }
            else
            {
                Page ma_page = util.MaPage(el);
                ma_page.IsBusy = afficher;
            }
        }
    }
    /*
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AZCombo : Xamarin.Forms.Grid
    {
        private const string nom_col_id = "i";
        private const string nom_col_lib = "l";
        private const string nom_col_nb_ligs = "n";
        private bool liste_complete;
        private bool liste_initialisee;
        private string titre;
        private string base_req;
        private string base_filtre_lib;
        private string base_filtre_id;
        private string metier_req;
        private string metier_filtre_lib;
        private string metier_filtre_id;
        private string courant_req;
        private string courant_filtre_lib;
        private string courant_filtre_id;
        private string courant_val_filtre_lib;
        private int? courant_val_filtre_id;
        private string debut_focus;
        private ItemCbo SelectedItem;
        private List<ItemCbo> ItemsSource;
        private int nb_total_items;
        public double taille_texte { get; set; }

        public AZCombo()
        {
            / *
            object bd = BindingContext;
            BindingContext = this;
            taille_texte = 30.0;
            InitializeComponent();
            BindingContext = bd;
            * /
            //            texte.Text = Texte;
            InitializeComponent();
            texte.TextChanged += ChangementDeCboLib;
            texte.Focused += texte_Focused;
            texte.Unfocused += texte_Unfocused;
            bouton.Clicked += ClicageDuBouton;
            liste_complete = false;
            liste_initialisee = false;
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(AZCombo), default(double), Xamarin.Forms.BindingMode.OneWay);
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly BindableProperty TitreProperty = BindableProperty.Create(nameof(Titre), typeof(string), typeof(AZCombo), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Titre
        {
            get { return (string)GetValue(TitreProperty); }
            set { SetValue(TitreProperty, value); }
        }
        public static readonly BindableProperty BaseReqProperty = BindableProperty.Create(nameof(BaseReq), typeof(string), typeof(AZCombo), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string BaseReq
        {
            get { return (string)GetValue(BaseReqProperty); }
            set { SetValue(BaseReqProperty, value); }
        }
        public static readonly BindableProperty BaseFiltreLibProperty = BindableProperty.Create(nameof(BaseFiltreLib), typeof(string), typeof(AZCombo), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string BaseFiltreLib
        {
            get { return (string)GetValue(BaseFiltreLibProperty); }
            set { SetValue(BaseFiltreLibProperty, value); }
        }
        public static readonly BindableProperty BaseFiltreIdProperty = BindableProperty.Create(nameof(BaseFiltreId), typeof(string), typeof(AZCombo), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string BaseFiltreId
        {
            get { return (string)GetValue(BaseFiltreIdProperty); }
            set { SetValue(BaseFiltreIdProperty, value); }
        }
        public static readonly BindableProperty CboLibProperty = BindableProperty.Create(nameof(CboLib), typeof(string), typeof(AZCombo), default(string), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnCboLibChanged);
        public string CboLib
        {
            get { return (string)GetValue(CboLibProperty); }
            set { SetValue(CboLibProperty, value); }
            / *
            get { return texte.Text; }
            set { this.texte.Text = value; }
            * /
        }
        public event EventHandler<CboLibChangedEventArgs> CboLibChanged;
        public static readonly BindableProperty CboIdProperty = BindableProperty.Create(nameof(CboId), typeof(int?), typeof(AZCombo), default(int), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnCboIdChanged);
        public int? CboId
        {
            get { return (int?)GetValue(CboIdProperty); }
            set { SetValue(CboIdProperty, value); }
        }
        public event EventHandler<CboIdChangedEventArgs> CboIdChanged;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == BaseReqProperty.PropertyName)
            {
                base_req = BaseReq;
            }
            else if (propertyName == BaseFiltreLibProperty.PropertyName)
            {
                base_filtre_lib = BaseFiltreLib;
            }
            else if (propertyName == BaseFiltreIdProperty.PropertyName)
            {
                base_filtre_id = BaseFiltreId;
            }
            else if (propertyName == CboIdProperty.PropertyName)
            {
            }
            else if (propertyName == CboLibProperty.PropertyName)
            {
                texte.Text = CboLib;
            }
            else if (propertyName == TitreProperty.PropertyName)
            {
                titre = Titre;
            }
            else if (propertyName == FontSizeProperty.PropertyName)
            {
                texte.FontSize = FontSize;
            }
        }

        static void OnCboLibChanged(object bindable, object oldValue, object newValue)
        {
            var combo = (AZCombo)bindable;
            combo.CboLibChanged?.Invoke(bindable, new CboLibChangedEventArgs((string)oldValue, (string)newValue));
        }

        static void OnCboIdChanged(object bindable, object oldValue, object newValue)
        {
            var combo = (AZCombo)bindable;
            combo.CboIdChanged?.Invoke(bindable, new CboIdChangedEventArgs((int?)oldValue, (int?)newValue));
        }
        / *
        public void Init(string req, string filtre_lib, string filtre_id)
        {
            base_req = req;
            base_filtre_lib = filtre_lib;
            base_filtre_id = filtre_id;
        }
        * /
        private void ChangementDeCboLib(object sender, TextChangedEventArgs e)
        {
            CboLib = e.NewTextValue;
        }
        private async Task<bool> InitialiserListe(string val_filtre_lib, int? val_filtre_id)
        {
            string req = base_req;
            string filtre_lib = base_filtre_lib;
            string filtre_id = base_filtre_id;
            if (metier_req != null && metier_req.Length > 0)
            {
                req = metier_req;
                filtre_lib = metier_filtre_lib;
                filtre_id = metier_filtre_id;
            }
            if (courant_req == req && courant_filtre_lib == filtre_lib && courant_filtre_id == filtre_id && courant_val_filtre_lib == val_filtre_lib && courant_val_filtre_id == val_filtre_id)
                return true;
            Sablier(true);
            courant_req = req;
            courant_filtre_lib = filtre_lib;
            courant_filtre_id = filtre_id;
            courant_val_filtre_lib = val_filtre_lib;
            courant_val_filtre_id = val_filtre_id;
            var util = new AZComboUtil(nom_col_nb_ligs, nom_col_id, nom_col_lib);
            / *
            req = util.PreparerReq(req);
            liste_complete = true;
            if (val_filtre_lib != null && val_filtre_lib.Length > 0)
            {
                req = req.Replace("1=1", "1=1" + filtre_lib.Replace("@valeur", val_filtre_lib));
                liste_complete = false;
            }
            if (val_filtre_id.HasValue)
            {
                req = req.Replace("1=1", "1=1" + filtre_id.Replace("@valeur", val_filtre_id.Value.ToString()));
                liste_complete = false;
            }
            AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
            DataTable dt = await ab.LireTable(req);
            if (dt != null)
            {
                int nb_ligs = dt.Rows.Count;
                int nb_total = nb_ligs;
                if (nb_ligs > 0)
                {
                    if (dt.Columns.Contains(nom_col_nb_ligs))
                    {
                        nb_total = Convert.ToInt32(dt.Rows[0][nom_col_nb_ligs]);
                    }
                }
                if (nb_ligs < nb_total)
                {
                    liste_complete = false;
                }
                ItemsSource = new List<ItemCbo>();
                foreach (DataRow dr in dt.Rows)
                {
                    ItemCbo it = new ItemCbo();
                    it.id = Convert.ToInt32(dr[nom_col_id].ToString());
                    it.lib = dr[nom_col_lib].ToString();
                    ItemsSource.Add(it);
                }
                liste_initialisee = true;
                nb_total_items = nb_total;
            }
            * /
            ItemsSource = await util.InitialiserListe(req, filtre_lib, filtre_id, val_filtre_lib, val_filtre_id);
            if (ItemsSource == null)
            {
                AfficherErreur("Aucune valeur disponible");
                liste_initialisee = false;
                liste_complete = false;
                nb_total_items = 0;
            }
            else
            {
                liste_initialisee = util.liste_initialisee;
                liste_complete = util.liste_complete;
                nb_total_items = util.nb_total_items;
            }
            Sablier(false);
            return true;
        }
        private async void ClicageDuBouton(object sender, EventArgs e)
        {
            bool ret = await InitialiserListe(texte.Text, null);
            int nb_libs = ItemsSource.Count;
            string[] liste_lib = new string[nb_libs];
            for (int i = 0; i < nb_libs; i++)
            {
                liste_lib.SetValue(ItemsSource[i].lib, i);
            }
            SelectedItem = null;
            CboId = null;
            / *
            bool trouve = false;
            bool fini = false;
            Element ma_page = this.Parent;
            while (!fini)
            {
                if (ma_page is Page)
                {
                    trouve = true;
                    fini = true;
                }
                else if (ma_page == null)
                    fini = true;
                else
                {
                    ma_page = ma_page.Parent;
                }
            }
            if (trouve)
            {
                * /
                Element ma_page = MaPage();
                if (ma_page != null)
                {
                    string titre_tmp = titre;
                    if (nb_total_items > nb_libs)
                    {
                        titre_tmp += " (" + nb_libs.ToString() + "/" + nb_total_items.ToString() + ")";
                    }
                    string annuler = "cancel";
                    string ret_clic = await ((Page)ma_page).DisplayActionSheet(titre_tmp, annuler, null, liste_lib);
                    if (ret_clic != annuler)
                    {
                        texte.Text = ret_clic;
                        for (int i = 0; i < nb_libs; i++)
                        {
                            if (ret_clic == ItemsSource[i].lib)
                            {
                                SelectedItem = ItemsSource[i];
                                CboId = SelectedItem.id;
                            }
                        }
                    }
                }
            }

            private Page MaPage()
            {
                bool trouve = false;
                bool fini = false;
                Element ma_page = this.Parent;
                while (!fini)
                {
                    if (ma_page is Page)
                    {
                        trouve = true;
                        fini = true;
                    }
                    else if (ma_page == null)
                        fini = true;
                    else
                    {
                        ma_page = ma_page.Parent;
                    }
                }
                if (!trouve)
                {
                    ma_page = null;
                }
                return (Page)ma_page;
            }

            private ListView MaListView()
            {
                bool trouve = false;
                bool fini = false;
                Element el = this.Parent;
                while (!fini)
                {
                    if (el is ListView)
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
                return (ListView)el;
            }

            private void Sablier(bool afficher)
            {
                ListView sv = MaListView();
                if (sv != null)
                {
                    if (afficher)
                        sv.BeginRefresh();
                    else
                        sv.EndRefresh();
                }
                else
                {
                    Page ma_page = MaPage();
                    ma_page.IsBusy = afficher;
                }
            }

            private async void AfficherErreur(string msg)
            {
                bool trouve = false;
                bool fini = false;
                Element ma_page = this.Parent;
                while (!fini)
                {
                    if (ma_page is Page)
                    {
                        trouve = true;
                        fini = true;
                    }
                    else if (ma_page == null)
                        fini = true;
                    else
                    {
                        ma_page = ma_page.Parent;
                    }
                }
                if (trouve)
                {
                    await ((Page)ma_page).DisplayAlert("Erreur", msg, "Cancel");
                }
            }

            private void texte_Unfocused(object sender, FocusEventArgs e)
            {
                //            throw new NotImplementedException();
                string fin_focus = texte.Text;
                if (fin_focus != debut_focus)
                {
                    ChangementDeLib(fin_focus);
                }
            }

            private void texte_Focused(object sender, FocusEventArgs e)
            {
                debut_focus = texte.Text;
            }

            public void PersonnaliserReqMetier(string req, string filtre_lib, string filtre_id)
            {
                metier_req = req;
                metier_filtre_lib = filtre_lib;
                metier_filtre_id = filtre_id;
            }

            public async void ChangementDeLib(string nouveau_texte)
            {
                if (!liste_initialisee)
                {
                    bool liste_complete = await InitialiserListe(null, null);
                }
                bool texte_correct = false;
                for (int etape = 0; etape < 2; etape++)
                {
                    foreach (ItemCbo item in this.ItemsSource)
                    {
                        if (nouveau_texte == item.lib)
                        {
                            texte_correct = true;
                            this.SelectedItem = item;
                        }
                    }
                    if (!texte_correct && etape == 0)
                    {
                        bool liste_complete = await InitialiserListe(nouveau_texte, null);
                    }
                }
                texte.Text = nouveau_texte;
                texte.TextColor = texte_correct ? Color.Black : Color.Red;
                if (texte_correct == false)
                    SelectedItem = null;
            }

            public async void ChangementDeCboId(int? nouvel_id)
            {
                bool initialisation_complete_faite = liste_complete;
                if (!liste_initialisee)
                {
                    bool liste_complete = await InitialiserListe("", nouvel_id);
                    initialisation_complete_faite = true;
                }
                bool texte_correct = false;
                for (int etape = 0; etape < 2 && texte_correct == false; etape++)
                {
                    foreach (ItemCbo item in this.ItemsSource)
                    {
                        if (nouvel_id == item.id)
                        {
                            texte_correct = true;
                            this.SelectedItem = item;
                            texte.Text = item.lib;
                        }
                    }
                    if (!texte_correct && etape == 0 && !initialisation_complete_faite)
                    {
                        // la liste etait deja initialisee mais ne contenait pas le id recherche
                        // peut-etre cette liste etait-elle tronquee
                        // => on le reteste
                        bool liste_complete = await InitialiserListe(null, nouvel_id);
                    }
                }
                if (!texte_correct)
                {
                    texte.Text = "[id=" + nouvel_id + "]";
                    //                this.texte_affiche.Text = this.CboTexte;
                }
                texte.TextColor = texte_correct ? Color.Black : Color.Red;
                if (texte_correct == false)
                    SelectedItem = null;
            }
        }
    */
    //    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AZComboCS : Xamarin.Forms.Grid
    {
        private const string nom_col_id = "i";
        private const string nom_col_lib = "l";
        private const string nom_col_nb_ligs = "n";
        private bool liste_complete;
        private bool liste_initialisee;
        public string titre;
        public string base_req;
        public string base_filtre_lib;
        public string base_filtre_id;
        public string metier_req;
        public string metier_filtre_lib;
        public string metier_filtre_id;
        private string courant_req;
        private string courant_filtre_lib;
        private string courant_filtre_id;
        private string courant_val_filtre_lib;
        private int? courant_val_filtre_id;
        private string debut_focus;
        private ItemCbo SelectedItem;
        private List<ItemCbo> ItemsSource;
        private int nb_total_items;
        public double taille_texte { get; set; }
        public Entry texte { get; set; }
        public Button bouton { get; set; }
        private const double hauteur_textes = 15.0;
        private const double hauteur_textes_boutons = 10.0;
        private double largeur_bouton = 30.0;
        //        private int? id;

        public static readonly BindableProperty CboLibProperty = BindableProperty.Create(nameof(CboLib), typeof(string), typeof(AZComboCS), default(string), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnCboLibChanged);
        public string CboLib
        {
            get { return (string)GetValue(CboLibProperty); }
            set { SetValue(CboLibProperty, value); }
            /*
            get { return texte.Text; }
            set { this.texte.Text = value; }
            */
        }
        public event EventHandler<CboLibChangedEventArgs> CboLibChanged;

        public static readonly BindableProperty CboIdProperty = BindableProperty.Create(nameof(CboId), typeof(int?), typeof(AZComboCS), default(int?), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnCboIdChanged);
        public int? CboId
        {
            get { return (int?)GetValue(CboIdProperty); }
            set { SetValue(CboIdProperty, value); }
        }
        public event EventHandler<CboIdChangedEventArgs> CboIdChanged;

        public static readonly BindableProperty CboLargeurProperty = BindableProperty.Create(nameof(CboLargeur), typeof(double?), typeof(AZComboCS), default(double?), Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnCboLargeurChanged);
        public double? CboLargeur
        {
            get { return (double)GetValue(CboLargeurProperty); }
            set { SetValue(CboLargeurProperty, value); }
        }
        public event EventHandler<CboLargeurChangedEventArgs> CboLargeurChanged;

        public AZComboCS()
        {
            /*
            double largeur = this.Width;
            double largeur_texte = largeur - largeur_bouton;
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_texte, GridUnitType.Absolute) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_bouton, GridUnitType.Absolute) });
            */
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30.0, GridUnitType.Absolute) });
            this.ColumnSpacing = 0.0;
            this.texte = new Entry();
            this.texte.TextChanged += ChangementDeCboLib;
            this.texte.Focused += texte_Focused;
            this.texte.Unfocused += texte_Unfocused;
            this.texte.Margin = 0.0;
            this.texte.FontSize = hauteur_textes;
            this.Children.Add(this.texte);
            Grid.SetColumn(this.texte, 0);
            this.bouton = new Button();
            this.bouton.Text = "V";
            this.bouton.Padding = 0.0;
            this.bouton.Margin = 0.0;
            this.bouton.FontSize = taille_texte > 0.0 ? taille_texte : hauteur_textes_boutons;
            this.bouton.Clicked += ClicageDuBouton;
            this.Children.Add(this.bouton);
            Grid.SetColumn(this.bouton, 1);
            liste_complete = false;
            liste_initialisee = false;
            nb_total_items = 0;
        }
        //        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        protected async override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == CboIdProperty.PropertyName)
            {
                bool trouve = false;
                if (ItemsSource != null && CboId.HasValue)
                {
                    foreach (ItemCbo ic in ItemsSource)
                    {
                        if (CboId.Value == ic.id)
                        {
                            texte.Text = ic.lib;
                            texte.TextColor = Color.Black;
                            trouve = true;
                        }
                    }
                }
                if (!trouve)
                {
                    /*
                    InitialiserListe("", CboId);
                    foreach (ItemCbo ic in ItemsSource)
                    {
                        if (CboId.Value == ic.id)
                        {
                            texte.Text = ic.lib;
                        }
                    }
                    */
                    bool ret = await ChangementDeCboId(CboId);
                }
            }
            else if (propertyName == CboLibProperty.PropertyName)
            {
                texte.Text = CboLib;
            }
            else if (propertyName == CboLargeurProperty.PropertyName)
            {
                this.ColumnDefinitions.Clear();
                double largeur_texte = CboLargeur.Value - largeur_bouton;
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_texte, GridUnitType.Absolute) });
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_bouton, GridUnitType.Absolute) });
            }
            else if (propertyName=="Renderer")
            {

            }
        }
        static void OnCboLibChanged(object bindable, object oldValue, object newValue)
        {
            var combo = (AZComboCS)bindable;
            combo.CboLibChanged?.Invoke(bindable, new CboLibChangedEventArgs((string)oldValue, (string)newValue));
        }
        static void OnCboIdChanged(object bindable, object oldValue, object newValue)
        {
            var combo = (AZComboCS)bindable;
            combo.CboIdChanged?.Invoke(bindable, new CboIdChangedEventArgs((int?)oldValue, (int?)newValue));
        }
        static void OnCboLargeurChanged(object bindable, object oldValue, object newValue)
        {
            var combo = (AZComboCS)bindable;
            combo.CboLargeurChanged?.Invoke(bindable, new CboLargeurChangedEventArgs((double?)oldValue, (double?)newValue));
        }
        private void ChangementDeCboLib(object sender, TextChangedEventArgs e)
        {
            CboLib = e.NewTextValue;
        }
        private async Task<bool> InitialiserListe(int? val_filtre_id, string val_filtre_lib)
        {
            string req = base_req;
            string filtre_lib = base_filtre_lib;
            string filtre_id = base_filtre_id;
            if (metier_req != null && metier_req.Length > 0)
            {
                req = metier_req;
                filtre_lib = metier_filtre_lib;
                filtre_id = metier_filtre_id;
            }
            if (courant_req == req && courant_filtre_lib == filtre_lib && courant_filtre_id == filtre_id && courant_val_filtre_lib == val_filtre_lib && courant_val_filtre_id == val_filtre_id)
                return true;
            var util = new AZComboUtil(nom_col_nb_ligs, nom_col_id, nom_col_lib);
            courant_req = req;
            courant_filtre_lib = filtre_lib;
            courant_filtre_id = filtre_id;
            courant_val_filtre_lib = val_filtre_lib;
            courant_val_filtre_id = val_filtre_id;
            ItemsSource = await util.InitialiserListe(req, filtre_id, filtre_lib, val_filtre_id, val_filtre_lib);
            if (ItemsSource == null)
            {
                await AfficherErreur("Aucune valeur disponible");
                liste_complete = false;
                liste_initialisee = false;
                nb_total_items = 0;
            }
            else
            {
                liste_complete = util.liste_complete;
                liste_initialisee = util.liste_initialisee;
                nb_total_items = util.nb_total_items;
            }
            return true;
        }
        private async void ClicageDuBouton(object sender, EventArgs e)
        {
            var util = new AZComboUtil(nom_col_nb_ligs, nom_col_id, nom_col_lib);
            util.Sablier(true, this);
            try
            {
                bool ret = await InitialiserListe(null, texte.Text);
                int nb_libs = ItemsSource != null ? ItemsSource.Count : 0;
                string[] liste_lib = new string[nb_libs];
                for (int i = 0; i < nb_libs; i++)
                {
                    liste_lib.SetValue(ItemsSource[i].lib, i);
                }
                SelectedItem = null;
                CboId = null;
                Element ma_page = util.MaPage(this);
                if (ma_page != null)
                {
                    string titre_tmp = titre;
                    if (nb_total_items > nb_libs)
                    {
                        titre_tmp += " (" + nb_libs.ToString() + "/" + nb_total_items.ToString() + ")";
                    }
                    string annuler = "cancel";
                    string ret_clic = await ((Page)ma_page).DisplayActionSheet(titre_tmp, annuler, null, liste_lib);
                    if (ret_clic != annuler)
                    {
                        texte.Text = ret_clic;
                        for (int i = 0; i < nb_libs; i++)
                        {
                            if (ret_clic == ItemsSource[i].lib)
                            {
                                SelectedItem = ItemsSource[i];
                                CboId = SelectedItem.id;
                                texte.TextColor = Color.Black;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AfficherException(ex);
            }
            finally
            {
                util.Sablier(false, this);
            }
        }
        private async void AfficherException(Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg += " (" + ex.InnerException.Message + ")";
            }
            msg += ex.StackTrace;
            await AfficherErreur(msg);
        }
        private async Task<bool> AfficherErreur(string msg)
        {
            AZComboUtil util = new AZComboUtil(nom_col_nb_ligs, nom_col_id, nom_col_lib);
            Page ma_page = util.MaPage(this);
            if (ma_page != null)
                await ((Page)ma_page).DisplayAlert("Erreur", msg, "Cancel");
            return true;
        }
        private void texte_Unfocused(object sender, FocusEventArgs e)
        {
            //            throw new NotImplementedException();
            string fin_focus = texte.Text;
            if (fin_focus != debut_focus)
            {
                ChangementDeLib(fin_focus);
            }
        }
        private void texte_Focused(object sender, FocusEventArgs e)
        {
            debut_focus = texte.Text;
        }
        public void PersonnaliserReqMetier(string req, string filtre_lib, string filtre_id)
        {
            metier_req = req;
            metier_filtre_lib = filtre_lib;
            metier_filtre_id = filtre_id;
        }
        private async void ChangementDeLib(string nouveau_texte)
        {
            if (!liste_initialisee)
            {
                bool liste_complete = await InitialiserListe(null, null);
            }
            bool texte_correct = false;
            for (int etape = 0; etape < 2; etape++)
            {
                foreach (ItemCbo item in this.ItemsSource)
                {
                    if (nouveau_texte == item.lib)
                    {
                        texte_correct = true;
                        this.SelectedItem = item;
                    }
                }
                if (!texte_correct && etape == 0)
                {
                    bool liste_complete = await InitialiserListe(null, nouveau_texte);
                }
            }
            texte.Text = nouveau_texte;
            texte.TextColor = texte_correct ? Color.Black : Color.Red;
            if (texte_correct == false)
                SelectedItem = null;
        }
        private async Task<bool> ChangementDeCboId(int? nouvel_id)
        {
            bool ret = false;
            bool initialisation_complete_faite = liste_complete;
            if (!liste_initialisee)
            {
                bool liste_complete = await InitialiserListe(nouvel_id, "");
                initialisation_complete_faite = true;
            }
            bool texte_correct = false;
            for (int etape = 0; etape < 2 && texte_correct == false; etape++)
            {
                foreach (ItemCbo item in this.ItemsSource)
                {
                    if (nouvel_id == item.id)
                    {
                        texte_correct = true;
                        this.SelectedItem = item;
                        texte.Text = item.lib;
                    }
                }
                if (!texte_correct && etape == 0 && !initialisation_complete_faite)
                {
                    // la liste etait deja initialisee mais ne contenait pas le id recherche
                    // peut-etre cette liste etait-elle tronquee
                    // => on le reteste
                    bool liste_complete = await InitialiserListe(nouvel_id, null);
                }
            }
            if (!texte_correct)
            {
                texte.Text = "[id=" + nouvel_id + "]";
                //                this.texte_affiche.Text = this.CboTexte;
            }
            texte.TextColor = texte_correct ? Color.Black : Color.Red;
            if (texte_correct == false)
                SelectedItem = null;
            ret = texte_correct;
            return ret;
        }
    }
}