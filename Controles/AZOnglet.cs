using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    public enum AZTypeOnglet { UNDEFINED, Formulaire, Grille };
    public class AZOnglet
    {
        private AZEcran _p;
        private AZTypeOnglet _type_onglet;
        private AZBlocDonnees _bd;
        private AZBoutonOnglet _btn;
        private string _entete;
        private string _req_lire;
        private string _req_maj;
        private string _proc_maj;
        //        private bool _objet_courant_modifie = false;
        public AZOnglet(AZEcran p, AZTypeOnglet to, string nom_tab)
        {
            _p = p;
            _type_onglet = to;
            AZTypeBlocDonnees tbd = AZTypeBlocDonnees.UNDEFINED;
            if (type_onglet == AZTypeOnglet.Formulaire)
                tbd = AZTypeBlocDonnees.Formulaire;
            else if (type_onglet == AZTypeOnglet.Grille)
                tbd = AZTypeBlocDonnees.Grille;
            _bd = new AZBlocDonnees(_p, this, tbd, nom_tab);
            entete = nom_tab;
        }
        public AZEcran p { get { return _p; } set { _p = value; } }
        public AZTypeOnglet type_onglet { get { return _type_onglet; } set { _type_onglet = value; } }
        public AZBlocDonnees bd { get { return _bd; } set { _bd = value; } }
        public AZBoutonOnglet btn { get { return _btn; } set { _btn = value; } }
        public string entete { get { return _entete; } set { _entete = value; _bd.header = value; } }
        public string req_lire { get { return _req_lire; } set { _req_lire = value; _bd.req_lire = value; } }
        public string req_maj { get { return _req_maj; } set { _req_maj = value; _bd.req_maj = value; } }
        public string proc_maj { get { return _proc_maj; } set { _proc_maj = value; _bd.proc_maj = value; } }
        public string nom_onglet { get { return "tb" + _bd.nom_table_bloc; } }
        /*
        public string NomListePourGrille()
        {
            return "liste_" + _bd.nom_table_bloc;
        }
        public string NomDataGridPourGrille()
        {
            return "dg" + _bd.nom_table_bloc;
        }
        public string NomOnglet()
        {
            return "tb" + _bd.nom_table_bloc;
        }
        public string NomBooleenPourSauvegarde()
        {
            return "onglet_" + _bd.nom_table_bloc + "_sauve";
        }
        public string NomObjetCourant()
        {
            return _bd.nom_table_bloc + "_courant";
        }
        public string NomFonctionObjetModifie()
        {
            return _bd.nom_table_bloc + "Modifie";
        }
        public string NomBouton()
        {
            return "btn" + _bd.nom_table_bloc;
        }
        */
        public void InitIHM()
        {
            btn = new AZBoutonOnglet(_entete);
            btn.ClassId = nom_onglet;
            bd.InitIHM();
        }
        public void Toucher(bool toucher)
        {
            if (_btn != null)
            {
                Button btn_tmp = _btn.m_bouton;
                if (btn_tmp != null)
                {
                    string nom_btn = btn_tmp.Text;
                    bool deja_touche = nom_btn.EndsWith("*");
                    if (toucher)
                    {
                        if (!deja_touche)
                            btn_tmp.Text += "*";
                        //                        _objet_courant_modifie = true;
                    }
                    else
                    {
                        if (deja_touche)
                            btn_tmp.Text = btn_tmp.Text.Substring(0, btn_tmp.Text.Length - 1);
                    }
                }
            }
        }
    }
    public class AZBoutonOnglet : Xamarin.Forms.Grid
    {
        private const int largeur_trait = 2;
        public Button m_bouton;
        public BoxView m_gauche;
        public BoxView m_droite;
        public BoxView m_haut;
        public BoxView m_bas;
        public AZBoutonOnglet(string texte)
        {
            this.Padding = 0.0;
            this.Margin = 0.0;
            this.ColumnSpacing = 0.0;
            this.RowSpacing = 0.0;
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_trait, GridUnitType.Absolute) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_trait, GridUnitType.Absolute) });
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(largeur_trait, GridUnitType.Absolute) });
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(largeur_trait, GridUnitType.Absolute) });
            m_bouton = new Button();
            m_bouton.Text = texte;
            m_bouton.FontSize = 10.0;
            Grid.SetColumn(m_bouton, 1);
            Grid.SetRow(m_bouton, 1);
            m_gauche = new BoxView();
            Grid.SetColumn(m_gauche, 0);
            Grid.SetRow(m_gauche, 0);
            Grid.SetRowSpan(m_gauche, 3);
            m_gauche.BackgroundColor = Color.Black;
            m_droite = new BoxView();
            Grid.SetColumn(m_droite, 2);
            Grid.SetRow(m_droite, 0);
            Grid.SetRowSpan(m_droite, 3);
            m_droite.BackgroundColor = Color.Black;
            m_haut = new BoxView();
            Grid.SetColumn(m_haut, 1);
            Grid.SetRow(m_haut, 0);
            m_haut.BackgroundColor = Color.Black;
            m_bas = new BoxView();
            Grid.SetColumn(m_bas, 0);
            Grid.SetRow(m_bas, 2);
            Grid.SetColumnSpan(m_bas, 3);
            m_bas.BackgroundColor = Color.Black;
            this.Children.Add(m_bouton);
            this.Children.Add(m_gauche);
            this.Children.Add(m_droite);
            this.Children.Add(m_haut);
            this.Children.Add(m_bas);
            Relacher();
        }
        public void Appuyer()
        {
            m_bouton.BackgroundColor = Color.White;
            m_gauche.IsVisible = true;
            m_droite.IsVisible = true;
            m_haut.IsVisible = true;
            m_bas.IsVisible = false;
        }
        public void Relacher()
        {
            m_bouton.BackgroundColor = Color.LightGray;
            m_gauche.IsVisible = false;
            m_droite.IsVisible = false;
            m_haut.IsVisible = false;
            m_bas.IsVisible = true;
        }
    }
}