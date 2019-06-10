using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Controles
{
    public interface IAZElement
    {
        object ChercherParNom(string nom);
    }
    public class AZBoxView : BoxView, IAZElement
    {
        public AZBoxView() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZButton : Button, IAZElement
    {
        public AZButton() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZColor
    {
        public Color coul;
        public AZColor(Color _coul)
        {
            coul = _coul;
        }
    }
    public class AZContentPage : ContentPage, IAZElement
    {
        public AZContentPage() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
    }
    public class AZDatePicker : DatePicker, IAZElement
    {
        public AZDatePicker() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZEntry : Entry, IAZElement
    {
        public AZEntry() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public enum AZGridUnitType { Absolute = 0, Star = 1, Auto = 2 }
    public class AZGrid : Grid, IAZElement
    {
        public AZGrid() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public GridLength DonnerGridLength(double lg, AZGridUnitType t)
        {
            GridUnitType t2 = t == AZGridUnitType.Absolute ? GridUnitType.Absolute : t == AZGridUnitType.Auto ? GridUnitType.Auto : GridUnitType.Star;
            GridLength gl = new GridLength(lg, t2);
            return gl;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZLabel : Label, IAZElement
    {
        public AZLabel() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZListView : ListView, IAZElement
    {
        public AZListView() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public enum AZScrollBarVisibility { Default = 0, Always = 1, Never = 2 }
    public class AZScrollView : ScrollView, IAZElement
    {
        public AZScrollView() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
    public class AZStackLayout : StackLayout, IAZElement
    {
        public AZStackLayout() : base()
        {
        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
    }
    public class AZSwitch : Switch, IAZElement
    {
        public AZSwitch() : base()
        {

        }
        public object ChercherParNom(string nom)
        {
            object o = FindByName(nom);
            return o;
        }
        public bool Visible { get { return IsVisible; } set { IsVisible = value; } }
    }
}